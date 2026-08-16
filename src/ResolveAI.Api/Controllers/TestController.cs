using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ResolveAI.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TestController : ControllerBase
{
    [HttpGet("secure-data")]
    [Authorize] // 
    public IActionResult GetSecureData()
    {
        //return Ok(new { Message = "Congratulations Dhruvi Your token is working and you are viewing secure data." });
        // 
        var userEmail = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;

        return Ok(new { Message = $"Congratulations {userEmail}! તમારો ટોકન કામ કરે છે." });
    }
}