using api_desafioo.tech.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace api_desafioo.tech.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PingController : ControllerBase
    {
        [HttpGet]
        public IActionResult Ping()
        {
            var dateTime = DateTimeHelper.GetBrasiliaTime();
            return Ok(dateTime);
        }
    }
}
