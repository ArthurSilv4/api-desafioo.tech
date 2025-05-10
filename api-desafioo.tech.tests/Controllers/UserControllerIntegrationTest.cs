using api_desafioo.tech.Context;
using api_desafioo.tech.Dtos.UserDtos;
using api_desafioo.tech.Models;
using Bogus;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using StackExchange.Redis;

namespace api_desafioo.tech.tests.Controllers
{

    public class UserControllerIntegrationTest : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly Faker _faker = new("pt_BR");
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly HttpClient _client;
        private readonly WebApplicationFactory<Program> _factory;
        private readonly SqliteConnection _connection;

        public UserControllerIntegrationTest(WebApplicationFactory<Program> factory, ITestOutputHelper testOutputHelper)
        {
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Testing");

            _testOutputHelper = testOutputHelper;

            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    // Remove o DbContext existente
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
                    if (descriptor != null) services.Remove(descriptor);

                    // Adiciona o DbContext com SQLite em memória
                    services.AddDbContext<AppDbContext>(options =>
                    {
                        options.UseSqlite(_connection);
                    });

                    // Configura o mock do IConnectionMultiplexer
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

                    services.AddSingleton(mockRedis.Object);

                    // Configura o banco de dados
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

        ~UserControllerIntegrationTest()
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
        [Fact]
        public async Task GetUser_ShouldReturnUser_WhenAuthenticated()
        {
            var token = await GetAuthenticationTokenAsync();
            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _client.GetAsync("/api/User");

            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadFromJsonAsync<UserDto>();

            content.Should().NotBeNull();
            content!.name.Should().Be("Test User");
            content.email.Should().Be("test@email.com");

            _testOutputHelper.WriteLine(JsonSerializer.Serialize(content, new JsonSerializerOptions { WriteIndented = true }));
        }
        
        [Fact]
        public async Task UpdateUserName_ShouldUpdateName_WhenRequestIsValid()
        {
            var token = await GetAuthenticationTokenAsync();
            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var request = new
            {
                newName = _faker.Person.FullName
            };

            var response = await _client.PutAsJsonAsync("/api/User/UpdateUserName", request);

            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();

            content.Should().Contain("Nome de usuário atualizado com sucesso");

            using (var scope = _factory.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var user = dbContext.Users.SingleOrDefault(u => u.Email == "test@email.com");
                user.Should().NotBeNull();
                user!.Name.Should().Be(request.newName);
            }
        }
        [Fact]
        public async Task UpdateDescription_ShouldUpdateDescription_WhenRequestIsValid()
        {
            var token = await GetAuthenticationTokenAsync();
            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var request = new
            {
                newDescription = _faker.Lorem.Paragraph()
            };

            var response = await _client.PutAsJsonAsync("/api/User/UpdateDescription", request);

            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();

            content.Should().Contain("Descrição atualizada com sucesso");

            using (var scope = _factory.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var user = dbContext.Users.SingleOrDefault(u => u.Email == "test@email.com");
                user.Should().NotBeNull();
                user!.Description.Should().Be(request.newDescription);
            }
        }
    }
}
