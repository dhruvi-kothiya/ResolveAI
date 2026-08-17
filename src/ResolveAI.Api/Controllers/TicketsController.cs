using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using ResolveAI.Application.DTOs;
using ResolveAI.Application.Interfaces;
using ResolveAI.Domain.Entities;
using ResolveAI.Domain.Enums;
using ResolveAI.Infrastructure.Persistence;

namespace ResolveAI.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TicketsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly INotificationService _notificationService;

    public TicketsController(
        ApplicationDbContext context,
        INotificationService notificationService)
    {
        _context = context;
        _notificationService = notificationService;
    }


    // =========================================================
    // CREATE TICKET
    // =========================================================

    [HttpPost]
    public async Task<IActionResult> CreateTicket(
        [FromBody] CreateTicketRequest request)
    {
        // 1. Ticket create
        var ticket = new Ticket
        {
            Id = Guid.NewGuid(),

            TicketNumber =
                $"INC-{1000 + _context.Tickets.Count() + 1}",

            Title = request.Title,
            Description = request.Description,

            Status = TicketStatus.New,
            Priority = TicketPriority.Medium,

            CreatedById = request.CreatedById,
            DepartmentId = request.DepartmentId,

            CreatedAt = DateTime.UtcNow
        };


        // =====================================================
        // 2. AI & SLA LOGIC
        // =====================================================

        // VPN અથવા Network issue હોય તો High Priority

        if (ticket.Description.Contains(
                "VPN",
                StringComparison.OrdinalIgnoreCase)
            ||
            ticket.Description.Contains(
                "Network",
                StringComparison.OrdinalIgnoreCase))
        {
            ticket.Priority = TicketPriority.High;
            ticket.IsAiProcessed = true;
        }


        // High Priority = 4 Hours
        // Medium Priority = 24 Hours

        ticket.DueAt =
            ticket.Priority == TicketPriority.High
                ? DateTime.UtcNow.AddHours(4)
                : DateTime.UtcNow.AddHours(24);


        // =====================================================
        // 3. SAVE TICKET
        // =====================================================

        _context.Tickets.Add(ticket);

        await _context.SaveChangesAsync();


        // =====================================================
        // 4. SEND NOTIFICATION
        // =====================================================

        await _notificationService.SendNotificationAsync(
            ticket.CreatedById,

            $"Your new ticket {ticket.TicketNumber} has been successfully created!",

            ticket.Id
        );


        // =====================================================
        // 5. RESPONSE
        // =====================================================

        return Ok(new
        {
            Message = "Ticket and Notification Created!",

            TicketNumber = ticket.TicketNumber,

            Priority = ticket.Priority.ToString(),

            AiProcessed = ticket.IsAiProcessed,

            Deadline = ticket.DueAt
        });
    }


    // =========================================================
    // GET ALL TICKETS
    // =========================================================

    [HttpGet]
    public async Task<IActionResult> GetAllTickets()
    {
        var tickets = await _context.Tickets
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();

        return Ok(tickets);
    }


    // =========================================================
    // UPDATE TICKET STATUS
    // =========================================================

    [HttpPatch("{id}/status")]
    public async Task<IActionResult> UpdateStatus(
        Guid id,
        [FromBody] TicketStatus newStatus)
    {
        var ticket = await _context.Tickets.FindAsync(id);

        if (ticket == null)
        {
            return NotFound();
        }


        // Status update
        ticket.Status = newStatus;


        // Resolved થાય ત્યારે ResolvedAt set કરો
        if (newStatus == TicketStatus.Resolved)
        {
            ticket.ResolvedAt = DateTime.UtcNow;
        }


        await _context.SaveChangesAsync();


        // Response
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