using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ResolveAI.Infrastructure.Persistence;

namespace ResolveAI.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class NotificationsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    public NotificationsController(ApplicationDbContext context) => _context = context;

    
    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetUserNotifications(Guid userId)
    {
        var notifications = await _context.Notifications
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();

        return Ok(notifications);
    }
}