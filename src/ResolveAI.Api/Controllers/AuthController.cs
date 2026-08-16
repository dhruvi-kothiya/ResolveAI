using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ResolveAI.Application.DTOs;
using ResolveAI.Application.Interfaces; 
using ResolveAI.Domain.Entities;

namespace ResolveAI.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly IJwtTokenGenerator _tokenGenerator; 

    
    public AuthController(UserManager<User> userManager, IJwtTokenGenerator tokenGenerator)
    {
        _userManager = userManager;
        _tokenGenerator = tokenGenerator;
    }
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        // 
        var user = new User
        {
            UserName = request.Email,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName
        };

        //
        var result = await _userManager.CreateAsync(user, request.Password);

        // 
        if (result.Succeeded)
        {
            // 
            await _userManager.AddToRoleAsync(user, "Employee");

            return Ok(new { Message = "User registered successfully as Employee!" });
        }

        // 
        return BadRequest(result.Errors);
    }
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user != null && await _userManager.CheckPasswordAsync(user, request.Password))
        {
            
            var token = _tokenGenerator.GenerateToken(user);
            return Ok(new { Token = token, Message = "Login Successful!" });
        }

        return Unauthorized("Invalid email or password.");
    }
}