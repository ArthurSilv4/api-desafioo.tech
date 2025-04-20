using api_desafioo.tech.Models;
using Bogus;
using FluentAssertions;
using Xunit.Abstractions;

namespace api_desafioo.tech.tests.Models
{
    [Trait("Category", "Models")]
    public class SubscriberTests
    {
        private readonly Faker _faker = new("pt_BR");
        private readonly ITestOutputHelper _testOutputHelper;

        public SubscriberTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void Subscriber_Constructor_ShouldCreateSubscriber_WithAllValidParameters()
        {
            var expectedEmail = _faker.Internet.Email();
            var subscriber = new Subscriber(expectedEmail);

            subscriber.Id.Should().NotBeEmpty();
            subscriber.Email.Should().Be(expectedEmail);
            subscriber.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMilliseconds(1000));

            _testOutputHelper.WriteLine($"Id: {subscriber.Id}");
            _testOutputHelper.WriteLine($"Email: {subscriber.Email}");
            _testOutputHelper.WriteLine($"CreatedAt: {subscriber.CreatedAt}");
        }
    }
}
