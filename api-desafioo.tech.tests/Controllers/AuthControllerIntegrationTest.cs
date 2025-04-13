using api_desafioo.tech.Context;
using api_desafioo.tech.Models;
using api_desafioo.tech.Requests.AuthRequests;
using Bogus;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit.Abstractions;

namespace api_desafioo.tech.tests.Controllers
{
    public class AuthControllerIntegrationTest : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly Faker _faker = new("pt_BR");
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly HttpClient _client;
        private readonly WebApplicationFactory<Program> _factory;
        private readonly SqliteConnection _connection;

        public AuthControllerIntegrationTest(WebApplicationFactory<Program> factory, ITestOutputHelper testOutputHelper)
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

                    var sp = services.BuildServiceProvider();
                    using var scope = sp.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    db.Database.EnsureCreated();

                    db.Users.Add(new User(
                        "Test User",
                        "teste@email.com",
                        BCrypt.Net.BCrypt.HashPassword("Password12345")
                    ));

                    db.SaveChanges();
                });
            });

            _client = _factory.CreateClient();
        }

        ~AuthControllerIntegrationTest()
        {
            _connection.Close();
        }

        [Fact]
        public async Task Login_ShouldReturnToken_WhenCredentialsAreValid()
        {
            var loginRequest = new LoginRequest
            (
                "teste@email.com",
                "Password12345"
            );

            var response = await _client.PostAsJsonAsync("/api/Auth/Login", loginRequest);

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadFromJsonAsync<JsonElement>();

            content.TryGetProperty("token", out _).Should().BeTrue();
            content.TryGetProperty("refreshToken", out _).Should().BeTrue();

            _testOutputHelper.WriteLine($"Status Code: {response.StatusCode}");
            _testOutputHelper.WriteLine($"Token: {content.GetProperty("token").GetString()}");
            _testOutputHelper.WriteLine($"RefreshToken: {content.GetProperty("refreshToken").GetString()}");
        }

        [Fact]
        public async Task Login_ShouldReturnError_WhenCredentialsAreInvalid()
        {
            var loginRequest = new LoginRequest
            (
                "invalid@email.com",
                "InvalidPassword"
            );

            var response = await _client.PostAsJsonAsync("/api/Auth/Login", loginRequest);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);

            _testOutputHelper.WriteLine($"Status Code: {response.StatusCode}");
        }

        [Fact]
        public async Task RefreshToken_ShouldReturnNewTokens_WhenRefreshTokenIsValid()
        {
            var loginRequest = new LoginRequest("teste@email.com", "Password12345");
            var loginResponse = await _client.PostAsJsonAsync("/api/Auth/Login", loginRequest);
            loginResponse.EnsureSuccessStatusCode();

            var loginContent = await loginResponse.Content.ReadFromJsonAsync<JsonElement>();
            var token = loginContent.GetProperty("token").GetString();
            var refreshToken = loginContent.GetProperty("refreshToken").GetString();

            var refreshTokenRequest = new RefreshTokenRequest
            (
                token,
                refreshToken
            );

            var response = await _client.PostAsJsonAsync("/api/Auth/RefreshToken", refreshTokenRequest);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadFromJsonAsync<JsonElement>();

            content.TryGetProperty("token", out _).Should().BeTrue();
            content.TryGetProperty("refreshToken", out _).Should().BeTrue();

            _testOutputHelper.WriteLine($"Status Code: {response.StatusCode}");
            _testOutputHelper.WriteLine($"Token: {content.GetProperty("token").GetString()}");
            _testOutputHelper.WriteLine($"RefreshToken: {content.GetProperty("refreshToken").GetString()}");
        }

        [Fact]
        public async Task RefreshToken_ShouldReturnUnauthorized_WhenRefreshTokenIsInvalid()
        {
            var loginRequest = new LoginRequest("teste@email.com", "Password12345");
            var loginResponse = await _client.PostAsJsonAsync("/api/Auth/Login", loginRequest);
            loginResponse.EnsureSuccessStatusCode();

            var loginContent = await loginResponse.Content.ReadFromJsonAsync<JsonElement>();
            var token = loginContent.GetProperty("token").GetString();

            var refreshTokenRequest = new RefreshTokenRequest
            (
                token,
                "invalid-refresh-token"
            );

            var response = await _client.PostAsJsonAsync("/api/Auth/RefreshToken", refreshTokenRequest);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);

            _testOutputHelper.WriteLine($"Status Code: {response.StatusCode}");
        }

        [Fact]
        public async Task RefreshToken_ShouldReturnUnauthorized_WhenUserDoesNotExist()
        {
            var loginRequest = new LoginRequest("teste@email.com", "Password12345");
            var loginResponse = await _client.PostAsJsonAsync("/api/Auth/Login", loginRequest);
            loginResponse.EnsureSuccessStatusCode();

            var loginContent = await loginResponse.Content.ReadFromJsonAsync<JsonElement>();
            var token = loginContent.GetProperty("token").GetString();
            var refreshToken = loginContent.GetProperty("refreshToken").GetString();

            var refreshTokenRequest = new RefreshTokenRequest
            (
                token,
                refreshToken
            );

            var sp = _factory.Services.CreateScope().ServiceProvider;
            var dbContext = sp.GetRequiredService<AppDbContext>();
            var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Email == "teste@email.com");
            dbContext.Users.Remove(user); 

            await dbContext.SaveChangesAsync();

            var response = await _client.PostAsJsonAsync("/api/Auth/RefreshToken", refreshTokenRequest);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);

            _testOutputHelper.WriteLine($"Status Code: {response.StatusCode}");
        }
    }
}
