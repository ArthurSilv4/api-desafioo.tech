using api_desafioo.tech.Configurations;
using api_desafioo.tech.Context;
using api_desafioo.tech.Dto;
using api_desafioo.tech.Helpers;
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
        private readonly IEmail _email;

        public ChallengeController(AppDbContext context, IEmail email)
        {
            _context = context;
            _email = email;
        }

        [HttpGet("ListChallenge")]
        public async Task<IActionResult> ListChallenge(CancellationToken ct)
        {
            var challenges = await _context.Challenges.ToListAsync(ct);
            var challengeDtos = challenges.Select(c => new ChallengeDto(c.Id, c.Title, c.Description, c.Dificulty, c.Category, c.AuthorName, c.Links)).ToList();
            return Ok(challengeDtos);
        }

        [HttpPost("StartChallenge")]
        public async Task<IActionResult> StartChallenge(Guid challengeId, StartChallengeRequest request, CancellationToken ct)
        {
            var emailBody = string.Empty;
            bool sendEmail = false;

            var challenge = await _context.Challenges.FindAsync(new object[] { challengeId }, ct);
            if (challenge == null)
            {
                return NotFound("Desafio não encontrado.");
            }

            var existingParticipant = await _context.ChallengeParticipants
                .SingleOrDefaultAsync(p => p.Email == request.email, ct);
            if (existingParticipant != null)
            {
                existingParticipant.UpdateLastChallengeDate(DateTimeHelper.GetBrasiliaTime());
                _context.ChallengeParticipants.Update(existingParticipant);
                await _context.SaveChangesAsync(ct);

                emailBody = $"Olá {existingParticipant.Name}, você iniciou o desafio {challenge.Title}!";
                sendEmail = _email.SendEmail(existingParticipant.Email, "Desafio iniciado", emailBody);

                if (!sendEmail)
                {
                    return BadRequest("Falha ao enviar e-mail.");
                }

                var challengedto = new ChallengeDto(challenge.Id, challenge.Title, challenge.Description, challenge.Dificulty, challenge.Category, challenge.AuthorName, challenge.Links);
                var dto = new ChallengeParticipantDto(existingParticipant.Name, existingParticipant.Email, challengedto);

                return Ok(dto);
            }

            var newParticipant = new ChallengeParticipant(request.name, request.email, challengeId);

            await _context.ChallengeParticipants.AddAsync(newParticipant, ct);
            await _context.SaveChangesAsync(ct);

            emailBody = $"Olá {newParticipant.Name}, você iniciou o desafio {challenge.Title}!";
            sendEmail = _email.SendEmail(newParticipant.Email, "Desafio iniciado", emailBody);

            if (!sendEmail)
            {
                return BadRequest("Falha ao enviar e-mail.");
            }

            var challengeDto = new ChallengeDto(challenge.Id, challenge.Title, challenge.Description, challenge.Dificulty, challenge.Category, challenge.AuthorName, challenge.Links);
            var Dto = new ChallengeParticipantDto(newParticipant.Name, newParticipant.Email, challengeDto);

            return Ok(Dto);
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

        [HttpPut("UpdateChallenge")]
        [Authorize]
        public async Task<IActionResult> UpdateChallenge(Guid challengeId, UpdateChallengeRequest request, CancellationToken ct)
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
            var challenge = await _context.Challenges.FindAsync(new object[] { challengeId }, ct);
            if (challenge == null)
            {
                return NotFound("Desafio não encontrado.");
            }
            if (challenge.AuthorId != userId)
            {
                return Unauthorized();
            }
            challenge.Update(request.title, request.description, request.dificulty, request.category, request.links);
            _context.Challenges.Update(challenge);
            await _context.SaveChangesAsync(ct);
            var challengeDto = new ChallengeDto(challenge.Id, challenge.Title, challenge.Description, challenge.Dificulty, challenge.Category, challenge.AuthorName, challenge.Links);
            return Ok(new { message = "Desafio editado", challengeDto });
        }

        [HttpDelete("DeleteChallenge")]
        [Authorize]
        public async Task<IActionResult> DeleteChallenge(Guid challengeId, CancellationToken ct)
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
            var challenge = await _context.Challenges.FindAsync(new object[] { challengeId }, ct);
            if (challenge == null)
            {
                return NotFound("Desafio não encontrado.");
            }
            if (challenge.AuthorId != userId)
            {
                return Unauthorized();
            }
            _context.Challenges.Remove(challenge);
            await _context.SaveChangesAsync(ct);

            var challengeDto = new ChallengeDto(challenge.Id, challenge.Title, challenge.Description, challenge.Dificulty, challenge.Category, challenge.AuthorName, challenge.Links);

            return Ok(new { message = "Desafio excluido", challengeDto });
        }
    }
}
