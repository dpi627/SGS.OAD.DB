﻿using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using SGS.OAD.DB.API.DTOs;
using SGS.OAD.DB.API.Services.DTOs;

namespace SGS.OAD.DB.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserInfoController(
        //SGSLims_chemContext context,
        Services.Interfaces.IUserInfoService userInfoService,
        IMapper mapper,
        ILogger<UserInfoController> logger
        ) : Controller
    {
        [HttpGet]
        public ActionResult<UserInfoEncryptResponse> Get([FromQuery] UserInfoEncryptRequest req)
        {
            try
            {
                logger.LogInformation("Request UserInfo with {@req}", req);
                var info = mapper.Map<UserInfoInfo>(req);
                var result = userInfoService.Get(info);

                if (result == null)
                {
                    logger.LogWarning("No UserInfo found for {@req}", req);
                    return NotFound();
                }

                var res = mapper.Map<UserInfoEncryptResponse>(result);
                logger.LogInformation("Response UserInfo with {@res}", res);
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
