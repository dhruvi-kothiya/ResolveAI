using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        // 1. Ticket object બનાવવો
        var ticket = new Ticket
        {
            Id = Guid.NewGuid(),
            TicketNumber = $"INC-{1000 + _context.Tickets.Count() + 1}",
            Title = request.Title,
            Description = request.Description,
            Status = TicketStatus.New,
            Priority = TicketPriority.Medium,
            CreatedById = request.CreatedById,
            DepartmentId = request.DepartmentId,
            CreatedAt = DateTime.UtcNow
        };

        // 2. AI Logic
        // VPN અથવા Network issue હોય તો High Priority
        if (ticket.Description.Contains("VPN", StringComparison.OrdinalIgnoreCase) ||
            ticket.Description.Contains("Network", StringComparison.OrdinalIgnoreCase))
        {
            ticket.Priority = TicketPriority.High;
            ticket.IsAiProcessed = true;
        }

        // 3. SLA Logic
        // High Priority = 4 hours
        // Medium Priority = 24 hours
        ticket.DueAt = (ticket.Priority == TicketPriority.High)
                    ? DateTime.UtcNow.AddHours(4)
                    : DateTime.UtcNow.AddHours(24);

        // 4. પહેલા ફક્ત Ticket save કરો
        _context.Tickets.Add(ticket);
        await _context.SaveChangesAsync();

        // 5. Ticket database માં save થઈ ગઈ છે
        // હવે Notification બનાવો
        var notification = new Notification
        {
            Id = Guid.NewGuid(),
            UserId = ticket.CreatedById,
            TicketId = ticket.Id,
            Message = $"તમારી નવી ટિકિટ {ticket.TicketNumber} સફળતાપૂર્વક બની ગઈ છે!",
            CreatedAt = DateTime.UtcNow,
            IsRead = false
        };

        // 6. Notification save કરો
        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync();

        // 7. Response
        return Ok(new
        {
            Message = "Ticket and Notification Created!",
            TicketNumber = ticket.TicketNumber,
            Priority = ticket.Priority.ToString(),
            AiProcessed = ticket.IsAiProcessed,
            Deadline = ticket.DueAt
        });
    }


    // Section 11 - બધી ટિકિટો જોવા માટે
    [HttpGet]
    public async Task<IActionResult> GetAllTickets()
    {
        var tickets = await _context.Tickets
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();

        return Ok(tickets);
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