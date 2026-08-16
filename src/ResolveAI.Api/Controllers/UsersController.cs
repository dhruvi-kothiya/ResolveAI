using Microsoft.AspNetCore.Mvc;
using ResolveAI.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ResolveAI.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public UsersController(ApplicationDbContext context)
    {
        _context = context;
    }

    // user join
    [HttpPost("assign-department")]
    public async Task<IActionResult> AssignDepartment(Guid userId, Guid departmentId)
    {
        var user = await _context.Users.FindAsync(userId);
        var dept = await _context.Departments.FindAsync(departmentId);

        if (user == null || dept == null) return BadRequest("User or Department not found.");

        user.DepartmentId = departmentId; // join
        await _context.SaveChangesAsync();

        return Ok(new { Message = $"User {user.Email} assigned to {dept.Name} department!" });
    }
}