using api_desafioo.tech.Context;
using api_desafioo.tech.Dtos.UserDtos;
using api_desafioo.tech.Helpers;
using api_desafioo.tech.Models;
using api_desafioo.tech.Requests.UserRequests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using System.Security.Claims;

namespace api_desafioo.tech.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IEmail _email;
        private readonly IConnectionMultiplexer _redis;

        public UserController(AppDbContext context, IEmail email, IConnectionMultiplexer redis)
        {
            _context = context;
            _email = email;
            _redis = redis;
        }

        [HttpGet]
        [Authorize]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUser(CancellationToken ct)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized();
            }
            if (!Guid.TryParse(userIdClaim.Value, out var userId))
            {
                return BadRequest("ID de usuário inválido.");
            }
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Id == userId, ct);
            if (user == null)
            {
                return NotFound("Usuário não encontrado.");
            }
            var userDto = new UserDto(user.Name, user.Description, user.Email, user.Roles);
            return Ok(userDto);
        }

        [HttpPost("UserAdm")]
        public async Task<IActionResult> UserAdm([FromBody] CancellationToken ct)
        {
            
            
            
            
            var newUser = new User("Arthur", "arthurdesouza.silv@gmail.com", BCrypt.Net.BCrypt.HashPassword("S1s2s3s4s5@"));
            await _context.Users.AddAsync(newUser, ct);
            await _context.SaveChangesAsync(ct);
            var userDto = new UserDto(newUser.Name, newUser.Description, newUser.Email, newUser.Roles);
            return Ok(userDto);
        }

        [HttpPost("CreateNewUser")]
        [Authorize]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateNewUser([FromBody] CreateNewUserRequest request, CancellationToken ct)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            var userRoleClaims = User.FindAll(ClaimTypes.Role)
                .SelectMany(c => c.Value.Split(','))
                .ToArray();
            if (!userRoleClaims.Any(role => role.Equals("Admin", StringComparison.OrdinalIgnoreCase)))
            {
                return Forbid("Você não tem permissão para criar um novo usuário.");
            }

            var user = new User(request.name, request.email, BCrypt.Net.BCrypt.HashPassword(request.password));
            await _context.Users.AddAsync(user, ct);
            await _context.SaveChangesAsync(ct);

            var userDto = new UserDto(user.Name, user.Description, user.Email, user.Roles);

            return Ok(userDto);
        }

        [HttpPut("UpdateUserName")]
        [Authorize]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateUserName([FromBody] UpdateUserNameRequest request, CancellationToken ct)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            if (!Guid.TryParse(userIdClaim.Value, out var userId))
            {
                return BadRequest("ID de usuário inválido.");
            }

            var user = await _context.Users.SingleOrDefaultAsync(u => u.Id == userId, ct);
            if (user == null)
            {
                return NotFound("Usuário não encontrado.");
            }

            var challenges = await _context.Challenges.Where(c => c.AuthorId == userId).ToListAsync(ct);

            foreach (var challenge in challenges)
            {
                challenge.UpdateAuthorName(request.newName);
                _context.Challenges.Update(challenge);
            }

            user.UpdateName(request.newName);
            _context.Users.Update(user);

            await _context.SaveChangesAsync(ct);

            return Ok("Nome de usuário atualizado com sucesso em todos os desafios.");
        }

        [HttpPut("UpdateDescription")]
        [Authorize]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateDescription([FromBody] UpdateDescriptionRequest request, CancellationToken ct)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized();
            }
            if (!Guid.TryParse(userIdClaim.Value, out var userId))
            {
                return BadRequest("ID de usuário inválido.");
            }
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Id == userId, ct);
            if (user == null)
            {
                return NotFound("Usuário não encontrado.");
            }
            user.UpdateDescription(request.newDescription);
            _context.Users.Update(user);
            await _context.SaveChangesAsync(ct);
            return Ok("Descrição atualizada com sucesso.");
        }

        [HttpPost("SendConfirmationEmail")]
        [Authorize]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> SendConfirmationEmail(CancellationToken ct)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized();
            }
            if (!Guid.TryParse(userIdClaim.Value, out var userId))
            {
                return BadRequest("ID de usuário inválido.");
            }
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Id == userId, ct);
            if (user == null)
            {
                return NotFound("Usuário não encontrado.");
            }

            var code = GenerateEmailConfirmationCode.GenerateCode();

            var cacheKey = $"confirmation_code_{userId}";
            var cacheValue = code;
            var cacheExpiry = TimeSpan.FromMinutes(10);

            var db = _redis.GetDatabase();
            await db.StringSetAsync(cacheKey, cacheValue, cacheExpiry);

            bool sendEmail = _email.SendConfirmationEmail(user.Email, user.Name, code);

            var userDto = new UserDto(user.Name, user.Description, user.Email, user.Roles);

            return Ok(new { message = "Código enviado", userDto });
        }

        [HttpPut("UpdatePassword")]
        [Authorize]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdatePassword([FromBody] UpdatePasswordRequest request, CancellationToken ct)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            if (!Guid.TryParse(userIdClaim.Value, out var userId))
            {
                return BadRequest("ID de usuário inválido.");
            }

            var user = await _context.Users.SingleOrDefaultAsync(u => u.Id == userId, ct);
            if (user == null)
            {
                return NotFound("Usuário não encontrado.");
            }

            var db = _redis.GetDatabase();
            var cacheKey = $"confirmation_code_{userId}";
            var cachedCode = await db.StringGetAsync(cacheKey);
            if (string.IsNullOrEmpty(cachedCode) || request.code != cachedCode)
            {
                return BadRequest("Código de confirmação não encontrado ou expirado.");
            }

            if (!BCrypt.Net.BCrypt.Verify(request.oldPassword, user.Password))
            {
                return BadRequest("A senha antiga está incorreta.");
            }

            user.UpdatePassword(BCrypt.Net.BCrypt.HashPassword(request.newPassword));
            _context.Users.Update(user);
            await _context.SaveChangesAsync(ct);

            var userDto = new UserDto(user.Name, user.Description, user.Email, user.Roles);

            return Ok(new { message = "Senha atualizada com sucesso.", userDto });
        }
    }
}
