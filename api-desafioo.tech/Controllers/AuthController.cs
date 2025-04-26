using Microsoft.AspNetCore.Mvc;
using api_desafioo.tech.Context;
using Microsoft.EntityFrameworkCore;
using api_desafioo.tech.Services;
using api_desafioo.tech.Requests.AuthRequests;
using System.Security.Claims;
using api_desafioo.tech.Configurations;
using api_desafioo.tech.Dtos.AuthDtos;

namespace api_desafioo.tech.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly TokenService _tokenService;

        public AuthController(AppDbContext context, TokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }

        [HttpPost("Login")]
        [ProducesResponseType(typeof(LoginDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken ct)
        {
            var userEmail = request.email.ToLowerInvariant();

            var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == userEmail, ct);

            if (user == null || !BCrypt.Net.BCrypt.Verify(request.password, user.Password))
            {
                return Unauthorized();
            }

            var token = _tokenService.GenerateToken(user);
            var refreshToken = _tokenService.GenerateRefreshToken();
            user.SetRefreshToken(refreshToken, DateTime.UtcNow.AddDays(JwtConfig.RefreshTokenExpirationInDays));

            await _context.SaveChangesAsync(ct);

            var loginDto = new LoginDto(
                Token: token,
                RefreshToken: refreshToken
            );

            return Ok(loginDto);
        }

        [HttpPost("RefreshToken")]
        [ProducesResponseType(typeof(RefreshTokenDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request, CancellationToken ct)
        {
            var principal = _tokenService.GetPrincipalFromExpiredToken(request.Token);
            var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Id == Guid.Parse(userId), ct);
            if (user == null || user.RefreshToken != request.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                return Unauthorized();
            }

            var newToken = _tokenService.GenerateToken(user);
            var newRefreshToken = _tokenService.GenerateRefreshToken();
            user.SetRefreshToken(newRefreshToken, DateTime.UtcNow.AddDays(JwtConfig.RefreshTokenExpirationInDays));

            await _context.SaveChangesAsync(ct);

            var RefreshTokenDto = new RefreshTokenDto(
                NewToken: newToken,
                NewRefreshToken: newRefreshToken
            );

            return Ok(RefreshTokenDto);
        }
    }
}
