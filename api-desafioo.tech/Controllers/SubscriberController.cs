using api_desafioo.tech.Context;
using api_desafioo.tech.Models;
using api_desafioo.tech.Requests.SubscriberRequests;
using Microsoft.AspNetCore.Mvc;

namespace api_desafioo.tech.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SubscriberController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SubscriberController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> Subscriber([FromBody] SubscriberRequest request, CancellationToken ct)
        {
            var subscriber = new Subscriber(request.email);

            await _context.Subscribers.AddAsync(subscriber, ct);
            await _context.SaveChangesAsync(ct);

            return Ok(new {message = "Inscrição realizada com sucesso" });
        }
    }
}
