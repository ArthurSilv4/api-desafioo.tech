using api_desafioo.tech.Configurations;
using api_desafioo.tech.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace api_desafioo.tech.Services
{
    public class TokenService
    {
        public string GenerateToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var privateKey = Encoding.ASCII.GetBytes(JwtConfig.PrivateKey);

            
            var credential = new SigningCredentials(
                new SymmetricSecurityKey(privateKey), 
                SecurityAlgorithms.HmacSha256Signature
            );

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = GenerateClaims(user),
                SigningCredentials = credential,
                Expires = DateTime.UtcNow.AddHours(JwtConfig.ExpirationInHours),
                Issuer = JwtConfig.Issuer,
                Audience = JwtConfig.Audience
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        public static ClaimsIdentity GenerateClaims(User user)
        {
            var ci = new ClaimsIdentity();

            ci.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
            foreach(var role in user.Roles)
            {
                ci.AddClaim(new Claim(ClaimTypes.Role, role));
            }

            return ci;  
        }
    }
}
