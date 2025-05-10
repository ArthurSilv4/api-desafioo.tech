
using api_desafioo.tech.Context;
using Bogus;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;

namespace api_desafioo.tech.tests.Controllers
{
    public class SubscriberControllerIntegrationTest : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly Faker _faker = new("pt_BR");
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly HttpClient _client;
        private readonly WebApplicationFactory<Program> _factory;
        private readonly SqliteConnection _connection;

        public SubscriberControllerIntegrationTest(WebApplicationFactory<Program> factory, ITestOutputHelper testOutputHelper)
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
                });
            });

            _client = _factory.CreateClient();
        }

        ~SubscriberControllerIntegrationTest()
        {
            _connection.Close();
        }

        [Fact]
        public async Task Subscriber_ShouldAddSubscriber_WhenRequestIsValid()
        {
            var request = new
            {
                email = _faker.Internet.Email()
            };

            var response = await _client.PostAsJsonAsync("/api/Subscriber", request);

            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadFromJsonAsync<JsonElement>();

            content.GetProperty("message").GetString().Should().Be("Inscrição realizada com sucesso");

            using (var scope = _factory.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var subscriber = dbContext.Subscribers.SingleOrDefault(s => s.Email == request.email);
                subscriber.Should().NotBeNull();
                subscriber!.Email.Should().Be(request.email);
            }

            _testOutputHelper.WriteLine(JsonSerializer.Serialize(content, new JsonSerializerOptions { WriteIndented = true }));
        }

        [Fact]
        public async Task Subscriber_ShouldReturnBadRequest_WhenEmailIsInvalid()
        {
            var request = new
            {
                email = "invalid-email"
            };

            var response = await _client.PostAsJsonAsync("/api/Subscriber", request);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        }
    }
}
