using api_desafioo.tech.Helpers;
using api_desafioo.tech.Models;
using Bogus;
using FluentAssertions;
using Xunit.Abstractions;

namespace api_desafioo.tech.tests.Models
{
    [Trait("Category", "Models")]
    public class UserTests
    {
        private readonly Faker _faker = new("pt_BR");
        private readonly ITestOutputHelper _testOutputHelper;

        public UserTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void User_Constructor_ShouldCreateUser_WithAllValidParameters()
        {
            var expectedName = _faker.Name.FullName();
            var expectedEmail = _faker.Internet.Email();
            var expectedPassword = _faker.Internet.Password();

            var user = new User(expectedName, expectedEmail, expectedPassword);

            user.Id.Should().NotBeEmpty();
            user.Name.Should().Be(expectedName);
            user.Description.Should().BeEmpty();
            user.Email.Should().Be(expectedEmail);
            user.Password.Should().Be(expectedPassword);
            user.Roles.Should().BeEquivalentTo(new[] { "User" });
            user.IsActivated.Should().BeTrue();
            user.CreatedAt.Should().BeCloseTo(DateTimeHelper.GetBrasiliaTime(), TimeSpan.FromMilliseconds(1000));
            user.Challenges.Should().BeEmpty();
            user.RefreshToken.Should().BeEmpty();
            user.RefreshTokenExpiryTime.Should().Be(DateTime.MinValue);

            _testOutputHelper.WriteLine($"Id: {user.Id}");
            _testOutputHelper.WriteLine($"Name: {user.Name}");
            _testOutputHelper.WriteLine($"Description: {user.Description}");
            _testOutputHelper.WriteLine($"Email: {user.Email}");
            _testOutputHelper.WriteLine($"Password: {user.Password}");
            _testOutputHelper.WriteLine($"Roles: {string.Join(", ", user.Roles)}");
            _testOutputHelper.WriteLine($"IsActivated: {user.IsActivated}");
            _testOutputHelper.WriteLine($"CreatedAt: {user.CreatedAt}");
            _testOutputHelper.WriteLine($"RefreshToken: {user.RefreshToken}");
            _testOutputHelper.WriteLine($"RefreshTokenExpiryTime: {user.RefreshTokenExpiryTime}");
        }

        [Fact]
        public void User_UpdatePassword_ShouldUpdatePassword()
        {
            var expectedName = _faker.Name.FullName();
            var expectedEmail = _faker.Internet.Email();
            var expectedPassword = _faker.Internet.Password();
            var user = new User(expectedName, expectedEmail, expectedPassword);
            var newPassword = _faker.Internet.Password();

            user.UpdatePassword(newPassword);

            user.Password.Should().Be(newPassword);

            _testOutputHelper.WriteLine($"Password: {user.Password}");
        }

        [Fact]
        public void User_UpdateName_ShouldUpdateName()
        {
            var expectedName = _faker.Name.FullName();
            var expectedEmail = _faker.Internet.Email();
            var expectedPassword = _faker.Internet.Password();
            var user = new User(expectedName, expectedEmail, expectedPassword);
            var newName = _faker.Name.FullName();

            user.UpdateName(newName);

            user.Name.Should().Be(newName);

            _testOutputHelper.WriteLine($"Name: {user.Name}");
        }

        [Fact]
        public void User_UpdateDescription_ShouldUpdateDescription()
        {
            var expectedName = _faker.Name.FullName();
            var expectedEmail = _faker.Internet.Email();
            var expectedPassword = _faker.Internet.Password();
            var user = new User(expectedName, expectedEmail, expectedPassword);
            var newDescription = _faker.Lorem.Paragraph();

            user.UpdateDescription(newDescription);

            user.Description.Should().Be(newDescription);

            _testOutputHelper.WriteLine($"Description: {user.Description}");
        }

        [Fact]
        public void User_SetRefreshToken_ShouldSetRefreshToken()
        {
            var expectedName = _faker.Name.FullName();
            var expectedEmail = _faker.Internet.Email();
            var expectedPassword = _faker.Internet.Password();
            var user = new User(expectedName, expectedEmail, expectedPassword);
            var expectedRefreshToken = _faker.Random.AlphaNumeric(32);
            var expectedExpiryTime = DateTimeHelper.GetBrasiliaTime().AddDays(1);

            user.SetRefreshToken(expectedRefreshToken, expectedExpiryTime);

            user.RefreshToken.Should().Be(expectedRefreshToken);
            user.RefreshTokenExpiryTime.Should().Be(expectedExpiryTime);

            _testOutputHelper.WriteLine($"RefreshToken: {user.RefreshToken}");
            _testOutputHelper.WriteLine($"RefreshTokenExpiryTime: {user.RefreshTokenExpiryTime}");
        }
    }
}
