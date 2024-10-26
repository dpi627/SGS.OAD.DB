using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGS.OAD.DB.API.Services.Interfaces;

namespace SGS.OAD.DB.API.Controllers;

[Route("[controller]")]
[ApiController]
public class AuthController(IJwtService jwt) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("login")]
    public IActionResult Login([FromBody] DTOs.LoginRequest request)
    {
        // 假設這裡做了一個簡單的用戶驗證
        if (request.Username == "admin" && request.Password == "0000")
        {
            var token = jwt.GenerateJwtToken(request.Username);
            return Ok(token);
        }

        return Unauthorized();
    }
}
