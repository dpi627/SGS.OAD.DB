using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SGS.OAD.DB.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class SecretController : ControllerBase
    {
        [HttpGet("protected")]
        public IActionResult GetProtectedData()
        {
            return Ok(new { Data = "This is protected data" });
        }

    }
}
