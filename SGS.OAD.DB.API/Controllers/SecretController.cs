using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SGS.OAD.DB.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class SecretController : ControllerBase
    {
        [ApiController]
        [Route("api/[controller]")]
        [Authorize]
        public class SecureController : ControllerBase
        {
            [HttpGet("protected")]
            public IActionResult GetProtectedData()
            {
                return Ok(new { Data = "This is protected data" });
            }
        }
    }
}
