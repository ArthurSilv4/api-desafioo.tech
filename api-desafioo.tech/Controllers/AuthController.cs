using Microsoft.AspNetCore.Mvc;
using api_desafioo.tech.Context;
using Microsoft.EntityFrameworkCore;
using api_desafioo.tech.Services;
using api_desafioo.tech.Requests.AuthRequests;

namespace api_desafioo.tech.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AuthController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request, TokenService service, CancellationToken ct)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == request.email, ct);

            if (user == null || !BCrypt.Net.BCrypt.Verify(request.password, user.Password))
            {
                return Unauthorized();
            }

            var token = service.GenerateToken(user);

            return Ok(token);
        }      
    }
}
