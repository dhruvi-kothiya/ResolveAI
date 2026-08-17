using Microsoft.AspNetCore.Mvc;
using ResolveAI.Application.DTOs;
using ResolveAI.Domain.Entities;
using ResolveAI.Domain.Enums;
using ResolveAI.Infrastructure.Persistence;

namespace ResolveAI.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TicketsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public TicketsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> CreateTicket([FromBody] CreateTicketRequest request)
    {
        // 1. Ticket number generate (INC-XXXX)
        var ticketCount = _context.Tickets.Count() + 1;
        var ticketNumber = $"INC-{1000 + ticketCount}";

        // 2. Create new ticket object
        var ticket = new Ticket
        {
            Id = Guid.NewGuid(),
            TicketNumber = ticketNumber,
            Title = request.Title,
            Description = request.Description,
            Status = TicketStatus.New,
            Priority = TicketPriority.Medium, // Default Medium
            CreatedById = request.CreatedById,
            DepartmentId = request.DepartmentId,
            CreatedAt = DateTime.UtcNow
        };

        // 3. AI Logic
        // VPN અથવા Network હોય તો Priority High
        if (ticket.Description.Contains("VPN", StringComparison.OrdinalIgnoreCase) ||
            ticket.Description.Contains("Network", StringComparison.OrdinalIgnoreCase))
        {
            ticket.Priority = TicketPriority.High;
            ticket.IsAiProcessed = true;
        }

        // 4. SLA Logic
        // High Priority = 4 hours
        // Medium/Other Priority = 24 hours
        ticket.DueAt = (ticket.Priority == TicketPriority.High)
                    ? DateTime.UtcNow.AddHours(4)
                    : DateTime.UtcNow.AddHours(24);

        // 5. Save ticket into database
        _context.Tickets.Add(ticket);
        await _context.SaveChangesAsync();

        // 6. Response
        return Ok(new
        {
            Message = "Ticket Created Successfully!",
            TicketNumber = ticket.TicketNumber,
            Priority = ticket.Priority.ToString(),
            AiProcessed = ticket.IsAiProcessed,
            Deadline = ticket.DueAt
        });
    }


    [HttpPatch("{id}/status")]
    public async Task<IActionResult> UpdateStatus(
        Guid id,
        [FromBody] TicketStatus newStatus)
    {
        var ticket = await _context.Tickets.FindAsync(id);

        if (ticket == null)
            return NotFound();

        ticket.Status = newStatus;

        if (newStatus == TicketStatus.Resolved)
        {
            ticket.ResolvedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();

        return Ok(new
        {
            Message = "Ticket Status Updated Successfully!",
            TicketNumber = ticket.TicketNumber,
            Priority = ticket.Priority.ToString(),
            AiProcessed = ticket.IsAiProcessed,
            Deadline = ticket.DueAt
        });
    }
}