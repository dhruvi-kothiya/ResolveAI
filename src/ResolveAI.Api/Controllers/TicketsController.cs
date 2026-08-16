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

        _context.Tickets.Add(ticket);
        await _context.SaveChangesAsync();

        return Ok(new
        {
            Message = "Ticket Created Successfully!",
            TicketNumber = ticket.TicketNumber
        });
    }
}