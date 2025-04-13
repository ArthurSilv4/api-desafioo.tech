using api_desafioo.tech.Context;
using api_desafioo.tech.Dto;
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

        [Fact]
        public async Task ListChallenge_ShouldReturnChallengesFromDatabase_WhenNotInCache()
        {
            var expectedTitle = _faker.Lorem.Sentence(3);
            var expectedDescription = _faker.Lorem.Paragraph(5);
            var expectedDificulty = _faker.PickRandom(new[] { "Easy", "Medium", "Hard" });
            var expectedCategory = new[] { _faker.Lorem.Word(), _faker.Lorem.Word() };
            var expectedAuthor = new User(
                _faker.Person.FullName,
                _faker.Internet.Email(),
                _faker.Internet.Password()
            );
            var expectedLinks = new List<string> {
                    _faker.Internet.Url(),
                    _faker.Internet.Url()
                };

            var challenge = new Challenge(expectedTitle, expectedDescription, expectedDificulty,
                expectedCategory, expectedAuthor, expectedLinks);

            using (var scope = _factory.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                dbContext.Challenges.Add(challenge);
                dbContext.SaveChanges();
            }

            var response = await _client.GetAsync("/api/Challenge/ListChallenge");

            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadFromJsonAsync<List<ChallengeDto>>();

            content.Should().NotBeNull();
            content.Should().ContainSingle();
            content.First().title.Should().Be(expectedTitle);

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

            var challenge = new Challenge(
                _faker.Lorem.Sentence(3),
                _faker.Lorem.Paragraph(5),
                _faker.PickRandom(new[] { "Easy", "Medium", "Hard" }),
                new[] { _faker.Lorem.Word(), _faker.Lorem.Word() },
                author,
                new List<string> { _faker.Internet.Url(), _faker.Internet.Url() }
            );

            using (var scope = _factory.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                dbContext.Users.Add(author);
                dbContext.Challenges.Add(challenge);
                dbContext.SaveChanges();
            }

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

    }
}
