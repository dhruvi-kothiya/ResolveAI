using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ResolveAI.Application.DTOs;
using ResolveAI.Domain.Entities;
using ResolveAI.Infrastructure.Persistence;

namespace ResolveAI.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TeamsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public TeamsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> CreateTeam([FromBody] CreateTeamRequest request)
    {
        // 
        var departmentExists = await _context.Departments.AnyAsync(d => d.Id == request.DepartmentId);
        if (!departmentExists)
        {
            return BadRequest("Invalid Department ID.");
        }

        // create new team
        var team = new Team
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            DepartmentId = request.DepartmentId
        };

        _context.Teams.Add(team);
        await _context.SaveChangesAsync();

        return Ok(new { Message = "Team created successfully!", TeamId = team.Id });
    }
}