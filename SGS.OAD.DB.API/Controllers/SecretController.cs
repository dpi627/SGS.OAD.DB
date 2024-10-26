using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SGS.OAD.DB.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class SecretController : ControllerBase
    {
        [HttpGet("protected")]
        public IActionResult GetProtectedData()
        {
            return Ok(new { Data = "This is protected data" });
        }

    }
}
