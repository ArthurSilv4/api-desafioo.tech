using api_desafioo.tech.Context;
using api_desafioo.tech.Dto;
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
    public class ChallengeController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ChallengeController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("CreateNewChallenge")]
        [Authorize]
        public async Task<IActionResult> CreateNewChallenge([FromBody] CreateNewChallengeRequest request, CancellationToken ct)
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
            var challenge = new Challenge(request.title, request.description, request.dificulty, request.category, user, request.links);
            await _context.Challenges.AddAsync(challenge, ct);
            await _context.SaveChangesAsync(ct);

            var challengeDto = new ChallengeDto(challenge.Id, challenge.Title, challenge.Description, challenge.Dificulty, challenge.Category, challenge.AuthorName, challenge.Links);

            return Ok(challengeDto);
        }
    }
}
