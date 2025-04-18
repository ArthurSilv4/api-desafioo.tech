using api_desafioo.tech.Context;
using api_desafioo.tech.Dto;
using api_desafioo.tech.Helpers;
using api_desafioo.tech.Models;
using Bogus;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using StackExchange.Redis;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit.Abstractions;
using static System.Net.Mime.MediaTypeNames;

namespace api_desafioo.tech.tests.Controllers
{
    public class ChallengeControllerIntegrationTest : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly Faker _faker = new("pt_BR");
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly HttpClient _client;
        private readonly WebApplicationFactory<Program> _factory;
        private readonly SqliteConnection _connection;

        public ChallengeControllerIntegrationTest(WebApplicationFactory<Program> factory, ITestOutputHelper testOutputHelper)
        {
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Testing");

            _testOutputHelper = testOutputHelper;

            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
                    if (descriptor != null) services.Remove(descriptor);

                    services.AddDbContext<AppDbContext>(options =>
                    {
                        options.UseSqlite(_connection);
                    });

                    var mockRedis = new Mock<IConnectionMultiplexer>();
                    var mockDb = new Mock<IDatabase>();

                    mockDb.Setup(db => db.StringGetAsync(
                            It.IsAny<RedisKey>(),
                            It.IsAny<CommandFlags>()))
                        .ReturnsAsync(RedisValue.Null);

                    mockDb.Setup(db => db.StringSetAsync(
                            It.IsAny<RedisKey>(),
                            It.IsAny<RedisValue>(),
                            It.IsAny<TimeSpan?>(),
                            It.IsAny<When>(),
                            It.IsAny<CommandFlags>()))
                        .ReturnsAsync(true);

                    mockRedis.Setup(x => x.GetDatabase(It.IsAny<int>(), It.IsAny<object>()))
                        .Returns(mockDb.Object);

                    services.AddSingleton(mockRedis);
                    services.AddSingleton(mockRedis.Object);

                    var sp = services.BuildServiceProvider();
                    using var scope = sp.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    db.Database.EnsureCreated();

                    db.Users.Add(new User(
                        "Test User",
                        "test@email.com",
                        BCrypt.Net.BCrypt.HashPassword("Password12345")
                    ));

                    db.SaveChanges();
                });

            });

            _client = _factory.CreateClient();
        }

        ~ChallengeControllerIntegrationTest()
        {
            _connection.Close();
        }

        private async Task<string> GetAuthenticationTokenAsync()
        {
            var loginRequest = new
            {
                email = "test@email.com",
                password = "Password12345"
            };

            var response = await _client.PostAsJsonAsync("/api/Auth/Login", loginRequest);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadFromJsonAsync<JsonElement>();
            return content.GetProperty("token").GetString() ?? string.Empty;
        }

        private async Task<Challenge> CreateChallengeAsync()
        {
            var title = _faker.Lorem.Sentence(3);
            var description = _faker.Lorem.Paragraph(5);
            var difficulty = _faker.PickRandom(new[] { "Easy", "Medium", "Hard" });
            var categories = new[] { _faker.Lorem.Word(), _faker.Lorem.Word() };
            var author = new User(
                 _faker.Person.FullName,
                 _faker.Internet.Email(),
                 _faker.Internet.Password()
            );
            var links = new List<string>
            {
                _faker.Internet.Url(),
                _faker.Internet.Url()
            };

            var challenge = new Challenge(title, description, difficulty, categories, author, links);

            using (var scope = _factory.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                dbContext.Users.Add(author);
                dbContext.Challenges.Add(challenge);
                await dbContext.SaveChangesAsync();
            }

            return challenge;
        }

        [Fact]
        public async Task ListChallenge_ShouldReturnChallengesFromDatabase_WhenNotInCache()
        {
            await CreateChallengeAsync();

            var response = await _client.GetAsync("/api/Challenge/ListChallenge");

            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadFromJsonAsync<List<ChallengeDto>>();

            content.Should().NotBeNull();
            content.Should().ContainSingle();

            _testOutputHelper.WriteLine(JsonSerializer.Serialize(content, new JsonSerializerOptions { WriteIndented = true }));
        }

        [Fact]
        public async Task AuthorInformations_ShouldReturnAuthorNameAndDescription_WhenChallengeExists()
        {
            var author = new User(
                _faker.Person.FullName,
                _faker.Internet.Email(),
                _faker.Internet.Password()
            );

            var challenge = await CreateChallengeAsync();

            var response = await _client.GetAsync($"/api/Challenge/AuthorInformations?challengeId={challenge.Id}");

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();

            content.Should().NotBeNull();
            content!.Should().ContainKey("name");
            content!.Should().ContainKey("description");

            content["name"].Should().Be(author.Name);
            content["description"].Should().Be(author.Description);

            _testOutputHelper.WriteLine(JsonSerializer.Serialize(content, new JsonSerializerOptions { WriteIndented = true }));
        }

        [Fact]
        public async Task AuthorInformations_ShouldReturnNotFound_WhenChallengeDoesNotExist()
        {
            var response = await _client.GetAsync("/api/Challenge/AuthorInformations?challengeId=00000000-0000-0000-0000-000000000000");

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);

            var content = await response.Content.ReadAsStringAsync();

            content.Should().Contain("Desafio não encontrado.");

            _testOutputHelper.WriteLine(content);
        }

        [Fact]
        public async Task ListChallengeUser_ShouldReturnUserChallenges_WhenUserIsAuthenticated()
        {
            var token = await GetAuthenticationTokenAsync();

            using (var scope = _factory.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var user = dbContext.Users.SingleOrDefault(u => u.Email == "test@email.com");

                if (user != null)
                {
                    var challenge1 = new Challenge(
                        _faker.Lorem.Sentence(3),
                        _faker.Lorem.Paragraph(5),
                        _faker.PickRandom(new[] { "Easy", "Medium", "Hard" }),
                        new[] { _faker.Lorem.Word(), _faker.Lorem.Word() },
                        user,
                        new List<string> { _faker.Internet.Url(), _faker.Internet.Url() }
                    );

                    var challenge2 = new Challenge(
                        _faker.Lorem.Sentence(3),
                        _faker.Lorem.Paragraph(5),
                        _faker.PickRandom(new[] { "Easy", "Medium", "Hard" }),
                        new[] { _faker.Lorem.Word(), _faker.Lorem.Word() },
                        user,
                        new List<string> { _faker.Internet.Url(), _faker.Internet.Url() }
                    );

                    dbContext.Challenges.AddRange(challenge1, challenge2);
                    await dbContext.SaveChangesAsync();
                }
            }

            var client = _factory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await client.GetAsync("/api/Challenge/ListChallengeUser");

            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadFromJsonAsync<List<ChallengeDto>>();

            content.Should().NotBeNull();
            content.Should().HaveCount(2);

            _testOutputHelper.WriteLine(JsonSerializer.Serialize(content, new JsonSerializerOptions { WriteIndented = true }));
        }

    }
}
