using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using SGS.OAD.DB.API.DTOs;
using SGS.OAD.DB.API.Models;

namespace SGS.OAD.DB.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserInfoController(
        SGSLims_chemContext context,
        IMapper mapper,
        ILogger<UserInfoController> logger
        ) : Controller
    {
        [HttpGet]
        public ActionResult<UserInfoEncryptResponse> Get([FromQuery] UserInfoEncryptRequest req)
        {
            try
            {
                logger.LogInformation("Get UserInfo with {@req}", req);
                var result = context.SQLEncryptPasswords
                    .FirstOrDefault(p =>
                        p.PLanguage == req.PLanguage &&
                        p.ServerName == req.ServerName &&
                        p.DatabaseName == req.DatabaseName &&
                        p.DatabaseRole == req.DatabaseRole
                    );

                if (result == null)
                {
                    logger.LogWarning("No UserInfo found for {@req}", req);
                    return NotFound();
                }

                var res = mapper.Map<UserInfoEncryptResponse>(result);
                logger.LogInformation("Get UserInfo with {@res}", res);
                return Ok(res);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in Get UserInfo with {@req}", req);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
