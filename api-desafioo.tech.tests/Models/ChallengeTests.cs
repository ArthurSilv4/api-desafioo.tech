using api_desafioo.tech.Models;
using Bogus;
using FluentAssertions;
using System.Reflection;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace api_desafioo.tech.tests.Models
{
    [Trait("Category", "Models")]
    public class ChallengeTests
    {
        private readonly Faker _faker = new("pt_BR");
        private readonly ITestOutputHelper _testOutputHelper;

        public ChallengeTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void Challenge_PrivateConstructor_ShouldNotBeAccessible()
        {
            var constructor = typeof(Challenge).GetConstructor(
               BindingFlags.Instance | BindingFlags.NonPublic,
               null, new Type[0], null);

            Assert.NotNull(constructor);
            var challenge = (Challenge)constructor.Invoke(null);

            Assert.Equal(Guid.Empty, challenge.Id);
            Assert.Null(challenge.Title);
            Assert.Null(challenge.Description);
            Assert.Null(challenge.Dificulty);
            Assert.Null(challenge.Category);
            Assert.Equal(0, challenge.Starts);
            Assert.Null(challenge.Links);
            Assert.Null(challenge.Author);
            Assert.Equal(Guid.Empty, challenge.AuthorId);
            Assert.Null(challenge.AuthorName);

            _testOutputHelper.WriteLine($"Id: {challenge.Id}");
            _testOutputHelper.WriteLine($"Title: {challenge.Title}");
            _testOutputHelper.WriteLine($"Description: {challenge.Description}");
            _testOutputHelper.WriteLine($"Dificulty: {challenge.Dificulty}");
            _testOutputHelper.WriteLine($"Category:  {challenge.Category}");
            _testOutputHelper.WriteLine($"Stars: {challenge.Starts}");
            _testOutputHelper.WriteLine($"Links: {challenge.Links}");
            _testOutputHelper.WriteLine($"Author: {challenge.Author}");
            _testOutputHelper.WriteLine($"AuthorId: {challenge.AuthorId}");
            _testOutputHelper.WriteLine($"AuthorName: {challenge.AuthorName}");
        }

        [Fact]
        public void Challenge_Constructor_ShouldCreateChallenge_WithAllValidParameters()
        {
            var expectedTitle = _faker.Lorem.Sentence(3);
            var expectedDescription = _faker.Lorem.Paragraph(5);
            var expectedDificulty = _faker.PickRandom(new[] { "Fácil", "Médio", "Difícil" });
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

            challenge.Id.Should().NotBeEmpty();
            challenge.Title.Should().Be(expectedTitle);
            challenge.Description.Should().Be(expectedDescription);
            challenge.Dificulty.Should().Be(expectedDificulty);
            challenge.Category.Should().BeEquivalentTo(expectedCategory);
            challenge.Author.Should().Be(expectedAuthor);
            challenge.AuthorId.Should().Be(expectedAuthor.Id);
            challenge.AuthorName.Should().Be(expectedAuthor.Name);
            challenge.Starts.Should().Be(0);
            challenge.Links.Should().BeEquivalentTo(expectedLinks);

            _testOutputHelper.WriteLine($"Id: {challenge.Id}");
            _testOutputHelper.WriteLine($"Title: {challenge.Title}");
            _testOutputHelper.WriteLine($"Description: {challenge.Description}");
            _testOutputHelper.WriteLine($"Dificulty: {challenge.Dificulty}");
            _testOutputHelper.WriteLine($"Category: {string.Join(", ", challenge.Category)}");
            _testOutputHelper.WriteLine($"Author: {challenge.Author.Name}");
            _testOutputHelper.WriteLine($"AuthorId: {challenge.AuthorId}");
            _testOutputHelper.WriteLine($"AuthorName: {challenge.AuthorName}");
            _testOutputHelper.WriteLine($"Stars: {challenge.Starts}");
            _testOutputHelper.WriteLine($"Links: {string.Join(", ", challenge.Links)}");
        }

        [Fact]
        public void Challenge_AddStar_ShouldIncrementStars()
        {
            var expectedTitle = _faker.Lorem.Sentence(3);
            var expectedDescription = _faker.Lorem.Paragraph(5);
            var expectedDificulty = _faker.PickRandom(new[] { "Fácil", "Médio", "Difícil" });
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

            var challenge = new Challenge(expectedTitle, expectedDescription, expectedDificulty, expectedCategory, expectedAuthor, expectedLinks);

            challenge.AddStar();
            challenge.Starts.Should().Be(1);
            challenge.AddStar();
            challenge.Starts.Should().Be(2);

            _testOutputHelper.WriteLine($"Stars: {challenge.Starts}");
        }

        [Fact]
        public void Challenge_Update_ShouldUpdateChallenge_WithAllValidParameters()
        {
            var expectedTitle = _faker.Lorem.Sentence(3);
            var expectedDescription = _faker.Lorem.Paragraph(5);
            var expectedDificulty = _faker.PickRandom(new[] { "Fácil", "Médio", "Difícil" });
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

            var challenge = new Challenge(expectedTitle, expectedDescription, expectedDificulty, expectedCategory, expectedAuthor, expectedLinks);

            var newTitle = _faker.Lorem.Sentence(3);
            var newDescription = _faker.Lorem.Paragraph(5);
            var newDificulty = _faker.PickRandom(new[] { "Fácil", "Médio", "Difícil" });
            var newCategory = new[] { _faker.Lorem.Word(), _faker.Lorem.Word() };
            var newLinks = new List<string> {
                _faker.Internet.Url(),
                _faker.Internet.Url()
            };

            challenge.Update(newTitle, newDescription, newDificulty, newCategory, newLinks);

            challenge.Title.Should().Be(newTitle);
            challenge.Description.Should().Be(newDescription);
            challenge.Dificulty.Should().Be(newDificulty);
            challenge.Category.Should().BeEquivalentTo(newCategory);
            challenge.Links.Should().BeEquivalentTo(newLinks);

            _testOutputHelper.WriteLine($"Title: {challenge.Title}");
            _testOutputHelper.WriteLine($"Description: {challenge.Description}");
            _testOutputHelper.WriteLine($"Dificulty: {challenge.Dificulty}");
            _testOutputHelper.WriteLine($"Category: {string.Join(", ", challenge.Category)}");
            _testOutputHelper.WriteLine($"Links: {string.Join(", ", challenge.Links)}");
        }
    }
}
