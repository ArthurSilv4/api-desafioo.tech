using api_desafioo.tech.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace api_desafioo.tech.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PingController : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType(typeof(DateTime), StatusCodes.Status200OK)]
        public IActionResult Ping()
        {
            var dateTime = DateTime.UtcNow;
            return Ok(dateTime);
        }
    }
}
