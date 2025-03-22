using api_desafioo.tech.Configurations;
using api_desafioo.tech.Models;
using api_desafioo.tech.Services;
using Bogus;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using Xunit.Abstractions;

namespace api_desafioo.tech.tests.Services
{
    [Trait("Category", "Services")]
    public class TokenServiceTests
    {
        private readonly Faker _faker = new("pt_BR");
        private readonly ITestOutputHelper _testOutputHelper;

        public TokenServiceTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
            JwtConfig.Initialize(new ConfigurationBuilder().AddJsonFile("appsettings.Development.json").Build());
        }

        [Fact]
        public void TokenService_GenerateToken_ShouldReturnValidToken()
        {
            var expectedUser = _faker.Person.UserName;
            var user = new User(expectedUser, _faker.Person.Email, _faker.Internet.Password());

            var tokenService = new TokenService();
            var token = tokenService.GenerateToken(user);

            Assert.False(string.IsNullOrEmpty(token), "Token should not be null or empty");

            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);

            JwtConfig.Issuer.Should().Be(jwtToken.Issuer);
            JwtConfig.Audience.Should().Be(jwtToken.Audiences.First()); // Fix: Use Audiences.First() instead of Audience
            JwtConfig.ExpirationInHours.Should().Be((int)jwtToken.ValidTo.Subtract(jwtToken.ValidFrom).TotalHours);

            _testOutputHelper.WriteLine($"Token: {token}");
        }

        [Fact]
        public void TokenService_GenerateRefreshToken_ShouldReturnValidRefreshToken()
        {
            var tokenService = new TokenService();
            var refreshToken = tokenService.GenerateRefreshToken();

            Assert.False(string.IsNullOrEmpty(refreshToken), "RefreshToken should not be null or empty");

            _testOutputHelper.WriteLine($"RefreshToken: {refreshToken}");
        }
    }
}
