using api_desafioo.tech.Context;
using api_desafioo.tech.Models;
using api_desafioo.tech.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace api_desafioo.tech.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UserController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("CreateNewUser")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateNewUser([FromBody] CreateNewUserRequest request, CancellationToken ct)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            var userRoleClaim = User.FindFirst(ClaimTypes.Role);
            if (userRoleClaim == null || userRoleClaim.Value != "Admin")
            {
                return Forbid("Você não tem permissão para criar um novo usuário.");
            }

            var user = new User(request.name, request.email, BCrypt.Net.BCrypt.HashPassword(request.password));
            await _context.Users.AddAsync(user, ct);
            await _context.SaveChangesAsync(ct);

            return Ok("Usuário criado com sucesso.");
        }

        [HttpPut("UpdateUserName")]
        [Authorize]
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

            user.UpdateName(request.newName);
            _context.Users.Update(user);
            await _context.SaveChangesAsync(ct);
            return Ok("Nome de usuário atualizado com sucesso.");
        }

        [HttpPut("UpdatePassword")]
        [Authorize]
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

            if (!BCrypt.Net.BCrypt.Verify(request.oldPassword, user.Password))
            {
                return BadRequest("A senha antiga está incorreta.");
            }

            user.UpdatePassword(BCrypt.Net.BCrypt.HashPassword(request.newPassword));
            _context.Users.Update(user);
            await _context.SaveChangesAsync(ct);

            return Ok("Senha atualizada com sucesso.");
        }
    }
}
