using api_desafioo.tech.Services;
using Bogus;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit.Abstractions;

namespace api_desafioo.tech.Tests.Services
{
    [Trait("Category", "Services")]
    public class EmailServiceTests
    {
        private readonly Faker _faker = new("pt_BR");
        private readonly EmailService _emailService;
        private readonly Mock<ILogger<EmailService>> _loggerMock;
        private readonly ITestOutputHelper _testOutputHelper;

        public EmailServiceTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
            _loggerMock = new Mock<ILogger<EmailService>>();
            _emailService = new EmailService(_loggerMock.Object);
        }

        [Fact]
        public void EmailService_SendEmail_ShouldReturnFalse_WhenExceptionIsThrown()
        {
            var expectedTo = _faker.Person.Email;
            var expectedSubject = _faker.Lorem.Sentence();
            var expectedBody = _faker.Lorem.Paragraph();

            var result = _emailService.SendEmail(expectedTo, expectedSubject, expectedBody);

            result.Should().BeFalse();

            _testOutputHelper.WriteLine("SendEmail returned false due to an exception. \n");
        }

        [Fact]
        public void EmailService_SendChallengeStartedEmail_ShouldReturnFalse_WhenExceptionIsThrown()
        {
            var expectedEmail = _faker.Internet.Email();
            var expectedName = _faker.Name.FirstName();
            var expectedTitle = _faker.Lorem.Sentence();
            var expectedDescription = _faker.Lorem.Paragraph();
            var expectedDifficulty = _faker.PickRandom(new[] { "Fácil", "Médio", "Difícil" });
            string[] categories = { "C#", "Backend" };
            List<string> links = new() { _faker.Internet.Url() };

            var result = _emailService.SendChallengeStartedEmail(
                expectedEmail, expectedName, expectedTitle,
                expectedDescription, expectedDifficulty, categories, "Autor", links);

            result.Should().BeFalse();

            _testOutputHelper.WriteLine("SendChallengeStartedEmail returned false due to an exception. \n");
        }

        [Fact]
        public void EmailService_SendConfirmationEmail_ShouldReturnFalse_WhenExceptionIsThrown()
        {
            var expectedEmail = _faker.Internet.Email();
            var expectedName = _faker.Name.FirstName();
            var expectedCode = _faker.Random.AlphaNumeric(6);

            var result = _emailService.SendConfirmationEmail(expectedEmail, expectedName, expectedCode);

            result.Should().BeFalse();

            _testOutputHelper.WriteLine("SendConfirmationEmail returned false due to an exception.");
        }
    }
}
