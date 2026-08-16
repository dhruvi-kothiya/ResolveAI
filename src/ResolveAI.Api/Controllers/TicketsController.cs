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
        // ૧. ticket nu generet (INC-XXXX)
        var ticketCount = _context.Tickets.Count() + 1;
        var ticketNumber = $"INC-{1000 + ticketCount}";

        // ૨.creat new ticket obj
        var ticket = new Ticket
        {
            Id = Guid.NewGuid(),
            TicketNumber = ticketNumber,
            Title = request.Title,
            Description = request.Description,
            Status = TicketStatus.New, // new ticket new
            Priority = TicketPriority.Medium, // Default 
            CreatedById = request.CreatedById,
            DepartmentId = request.DepartmentId,
            CreatedAt = DateTime.UtcNow
        };
        // Simulated AI Logic
        if (ticket.Description.Contains("VPN", StringComparison.OrdinalIgnoreCase) ||
        ticket.Description.Contains("Network", StringComparison.OrdinalIgnoreCase))
        {
            ticket.Priority = TicketPriority.High; // AI એ નક્કી કર્યું કે આ High છે
            ticket.IsAiProcessed = true;
        }

        _context.Tickets.Add(ticket);
        await _context.SaveChangesAsync();

        return Ok(new
        {
            Message = "Ticket Created Successfully!",
            TicketNumber = ticket.TicketNumber
        });

    }
    // 
    [HttpPatch("{id}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] TicketStatus newStatus)
    {
        var ticket = await _context.Tickets.FindAsync(id);
        if (ticket == null) return NotFound();

        ticket.Status = newStatus;

        if (newStatus == TicketStatus.Resolved)
        {
            ticket.ResolvedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();

        // આ રીતે લખશો તો જ Postman માં High અને True દેખાશે
        return Ok(new
        {
            Message = "Ticket Created Successfully!",
            TicketNumber = ticket.TicketNumber,
            Priority = ticket.Priority.ToString(), // "High" 
            AiProcessed = ticket.IsAiProcessed,     // true 
        });
    }
}