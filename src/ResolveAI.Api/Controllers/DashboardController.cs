using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ResolveAI.Application.DTOs;
using ResolveAI.Infrastructure.Persistence;

namespace ResolveAI.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DashboardController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public DashboardController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("stats")]
    public async Task<IActionResult> GetStats()
    {
        // SQL Server find num
        var stats = new DashboardStatsResponse
        {
            TotalTickets = await _context.Tickets.CountAsync(),

            OpenTickets = await _context.Tickets
                .CountAsync(t => t.Status == ResolveAI.Domain.Enums.TicketStatus.New ||
                                 t.Status == ResolveAI.Domain.Enums.TicketStatus.Open),

            ResolvedTickets = await _context.Tickets
                .CountAsync(t => t.Status == ResolveAI.Domain.Enums.TicketStatus.Resolved),

            HighPriorityTickets = await _context.Tickets
    .CountAsync(t => t.Priority == ResolveAI.Domain.Enums.TicketPriority.High),

            TotalDepartments = await _context.Departments.CountAsync()
        };

        return Ok(stats);
    }
}