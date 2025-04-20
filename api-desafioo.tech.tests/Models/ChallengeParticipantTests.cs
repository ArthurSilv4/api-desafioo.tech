using api_desafioo.tech.Helpers;
using api_desafioo.tech.Models;
using Bogus;
using FluentAssertions;
using Xunit.Abstractions;

namespace api_desafioo.tech.tests.Models
{
    [Trait("Category", "Models")]
    public class ChallengeParticipantTests
    {
        private readonly Faker _faker = new("pt_BR");
        private readonly ITestOutputHelper _testOutputHelper;

        public ChallengeParticipantTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void ChallengeParticipant_Constructor_ShouldCreateChallenge_WithAllValidParameters()
        {
            var expectedName = _faker.Name.FullName();
            var expectedEmail = _faker.Internet.Email();
            var expectedChallenge = new Challenge(
                _faker.Lorem.Sentence(),
                _faker.Lorem.Paragraph(),
                _faker.PickRandom(new[] { "Fácil", "Médio", "Difícil" }),
                new[] { _faker.Lorem.Word() },
                new User(_faker.Name.FullName(), _faker.Internet.Email(), _faker.Internet.Password()),
                new List<string> { _faker.Internet.Url() }
            );
            var expectedChallengeId = expectedChallenge.Id;

            var challengeParticipant = new ChallengeParticipant(expectedName, expectedEmail, expectedChallengeId);

            challengeParticipant.Id.Should().NotBeEmpty();
            challengeParticipant.Name.Should().Be(expectedName);
            challengeParticipant.Email.Should().Be(expectedEmail);
            challengeParticipant.FirstChallengeDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMilliseconds(1000));
            challengeParticipant.LastChallengeDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMilliseconds(1000));
            challengeParticipant.Challenges.Should().BeEmpty();
            challengeParticipant.ChallengeId.Should().Be(expectedChallengeId);

            _testOutputHelper.WriteLine($"Id: {challengeParticipant.Id}");
            _testOutputHelper.WriteLine($"Name: {challengeParticipant.Name}");
            _testOutputHelper.WriteLine($"Email: {challengeParticipant.Email}");
            _testOutputHelper.WriteLine($"FirstChallengeDate: {challengeParticipant.FirstChallengeDate}");
            _testOutputHelper.WriteLine($"LastChallengeDate: {challengeParticipant.LastChallengeDate}");
            _testOutputHelper.WriteLine($"ChallengeId: {challengeParticipant.ChallengeId}");
        }

        [Fact]
        public void ChallengeParticipant_UpdateLastChallengeDate_ShouldUpdateLastChallengeDate()
        {
            var expectedName = _faker.Name.FullName();
            var expectedEmail = _faker.Internet.Email();
            var expectedChallenge = new Challenge(
                _faker.Lorem.Sentence(),
                _faker.Lorem.Paragraph(),
                _faker.PickRandom(new[] { "Fácil", "Médio", "Difícil" }),
                new[] { _faker.Lorem.Word() },
                new User(_faker.Name.FullName(), _faker.Internet.Email(), _faker.Internet.Password()),
                new List<string> { _faker.Internet.Url() }
            );
            var expectedChallengeId = expectedChallenge.Id;
            var challengeParticipant = new ChallengeParticipant(expectedName, expectedEmail, expectedChallengeId);
            var newDate = DateTime.UtcNow.AddDays(1);

            challengeParticipant.UpdateLastChallengeDate(newDate);

            challengeParticipant.LastChallengeDate.Should().Be(newDate);

            _testOutputHelper.WriteLine($"LastChallengeDate: {challengeParticipant.LastChallengeDate}");
        }
    }

}
