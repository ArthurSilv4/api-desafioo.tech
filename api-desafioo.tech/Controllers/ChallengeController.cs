﻿using api_desafioo.tech.Configurations;
using api_desafioo.tech.Context;
using api_desafioo.tech.Dto;
using api_desafioo.tech.Helpers;
using api_desafioo.tech.Models;
using api_desafioo.tech.Requests.ChallengeRequests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;
using System.Security.Claims;
using System.Text.Json;

namespace api_desafioo.tech.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChallengeController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IEmail _email;
        private readonly IConnectionMultiplexer _cache;

        public ChallengeController(AppDbContext context, IEmail email, IConnectionMultiplexer cache)
        {
            _context = context;
            _email = email;
            _cache = cache;
        }

        [HttpGet("ListChallenge")]
        public async Task<IActionResult> ListChallenge(CancellationToken ct)
        {
            var cacheKey = "ListChallenge";
            var db = _cache.GetDatabase();
            var cachedChallenges = await db.StringGetAsync(cacheKey);

            if (!cachedChallenges.IsNullOrEmpty)
            {
                return Ok(JsonSerializer.Deserialize<List<ChallengeDto>>(cachedChallenges.ToString() ?? string.Empty));
            }

            var challenges = await _context.Challenges.ToListAsync(ct);
            var challengeDtos = challenges.Select(c => new ChallengeDto(c.Id, c.Title, c.Description, c.Dificulty, c.Category, c.AuthorName, c.Starts, c.Links)).ToList();

            var serializedChallenges = JsonSerializer.Serialize(challengeDtos);
            var cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10) 
            };

            await db.StringSetAsync(cacheKey, serializedChallenges, cacheOptions.AbsoluteExpirationRelativeToNow);

            return Ok(challengeDtos);
        }

        [HttpGet("AuthorInformations")]
        public async Task<IActionResult> AuthorInformations(Guid challengeId, CancellationToken ct)
        {
            var challenge = await _context.Challenges.FindAsync(new object[] { challengeId }, ct);
            if (challenge == null)
            {
                return NotFound("Desafio não encontrado.");
            }
            var author = await _context.Users.FindAsync(new object[] { challenge.AuthorId }, ct);
            if (author == null)
            {
                return NotFound("Autor não encontrado.");
            }
            var authorDto = new { author.Name, author.Description };
            return Ok(authorDto);
        }

        [HttpGet("ListChallengeUser")]
        [Authorize]
        public async Task<IActionResult> ListChallengeUser(CancellationToken ct)
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
            var challengeDtos = challenges.Select(c => new ChallengeDto(c.Id, c.Title, c.Description, c.Dificulty, c.Category, c.AuthorName, c.Starts, c.Links)).ToList();
            return Ok(challengeDtos);
        }

        [HttpGet("ChallengeId")]
        public async Task<IActionResult> GetChallenge(Guid challengeId, CancellationToken ct)
        {
            var challenge = await _context.Challenges.FindAsync(new object[] { challengeId }, ct);
            if (challenge == null)
            {
                return NotFound("Desafio não encontrado.");
            }
            var challengeDto = new ChallengeDto(challenge.Id, challenge.Title, challenge.Description, challenge.Dificulty, challenge.Category, challenge.AuthorName, challenge.Starts, challenge.Links);
            return Ok(challengeDto);
        }

        [HttpGet("ListAuthorsChallenge")]
        public async Task<IActionResult> AuthorsChallenge(CancellationToken ct)
        {
            var authors = await _context.Challenges.Select(c => c.AuthorName).Distinct().ToListAsync(ct);


            return Ok(authors);
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
                challenge.AddStar();
                await _context.SaveChangesAsync(ct);

                sendEmail = _email.SendChallengeStartedEmail(existingParticipant.Email, existingParticipant.Name, challenge.Title, challenge.Description, challenge.Dificulty, challenge.Category, challenge.AuthorName, challenge.Links);

                if (!sendEmail)
                {
                    return BadRequest("Falha ao enviar e-mail.");
                }

                var challengedto = new ChallengeDto(challenge.Id, challenge.Title, challenge.Description, challenge.Dificulty, challenge.Category, challenge.AuthorName, challenge.Starts, challenge.Links);
                var dto = new ChallengeParticipantDto(existingParticipant.Name, existingParticipant.Email, challengedto);

                return Ok(new { message = "Desafio iniciado", dto });
            }

            var newParticipant = new ChallengeParticipant(request.name, request.email, challengeId);

            await _context.ChallengeParticipants.AddAsync(newParticipant, ct);
            challenge.AddStar();
            await _context.SaveChangesAsync(ct);

            sendEmail = _email.SendChallengeStartedEmail(newParticipant.Email, newParticipant.Name, challenge.Title, challenge.Description, challenge.Dificulty, challenge.Category, challenge.AuthorName, challenge.Links);

            if (!sendEmail)
            {
                return BadRequest("Falha ao enviar e-mail.");
            }

            var challengeDto = new ChallengeDto(challenge.Id, challenge.Title, challenge.Description, challenge.Dificulty, challenge.Category, challenge.AuthorName, challenge.Starts, challenge.Links);
            var Dto = new ChallengeParticipantDto(newParticipant.Name, newParticipant.Email, challengeDto);


            return Ok(new { message = "Desafio iniciado", Dto });
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

            var challengeDto = new ChallengeDto(challenge.Id, challenge.Title, challenge.Description, challenge.Dificulty, challenge.Category, challenge.AuthorName, challenge.Starts, challenge.Links);

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
            challenge.Update(
                request.title ?? challenge.Title,
                request.description ?? challenge.Description,
                request.dificulty ?? challenge.Dificulty,
                request.category ?? challenge.Category,
                request.links ?? challenge.Links
            );
            _context.Challenges.Update(challenge);
            await _context.SaveChangesAsync(ct);
            var challengeDto = new ChallengeDto(challenge.Id, challenge.Title, challenge.Description, challenge.Dificulty, challenge.Category, challenge.AuthorName, challenge.Starts, challenge.Links);
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

            var challengeDto = new ChallengeDto(challenge.Id, challenge.Title, challenge.Description, challenge.Dificulty, challenge.Category, challenge.AuthorName, challenge.Starts, challenge.Links);

            return Ok(new { message = "Desafio excluido", challengeDto });
        }
    }
}
