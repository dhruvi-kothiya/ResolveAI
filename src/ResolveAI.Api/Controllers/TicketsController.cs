using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
    private readonly UserManager<User> _userManager;
    private readonly IAuditService _auditService;

    public TicketsController(
        ApplicationDbContext context,
        INotificationService notificationService,
        UserManager<User> userManager,
        IAuditService auditService)
    {
        _context = context;
        _notificationService = notificationService;
        _userManager = userManager;
        _auditService = auditService;
    }


    // =========================================================
    // CREATE TICKET
    // Only Employee can create ticket
    // =========================================================

    [Authorize(Roles = "Employee")]
    [HttpPost]
    public async Task<IActionResult> CreateTicket(
        [FromBody] CreateTicketRequest request)
    {
        // =====================================================
        // 1. CREATE TICKET
        // =====================================================

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


        // =====================================================
        // 3. SLA DUE DATE
        // =====================================================

        ticket.DueAt =
            ticket.Priority == TicketPriority.High
                ? DateTime.UtcNow.AddHours(4)
                : DateTime.UtcNow.AddHours(24);


        // =====================================================
        // 4. AUTOMATIC AGENT ASSIGNMENT
        // =====================================================

        var agents =
            await _userManager.GetUsersInRoleAsync("Agent");

        var assignedAgent =
            agents.FirstOrDefault();

        if (assignedAgent != null)
        {
            ticket.AssignedToId =
                assignedAgent.Id;

            ticket.Status =
                TicketStatus.Open;
        }


        // =====================================================
        // 5. SAVE TICKET
        // =====================================================

        _context.Tickets.Add(ticket);

        await _context.SaveChangesAsync();


        // =====================================================
        // 6. SEND NOTIFICATION TO EMPLOYEE
        // =====================================================

        await _notificationService.SendNotificationAsync(
            ticket.CreatedById,

            $"Your new ticket {ticket.TicketNumber} has been successfully created!",

            ticket.Id
        );


        // =====================================================
        // 7. AUDIT LOG
        // =====================================================

        await _auditService.LogAsync(
            ticket.CreatedById,
            "Created Ticket",
            "Tickets",
            ticket.TicketNumber
        );


        // =====================================================
        // 8. RESPONSE
        // =====================================================

        return Ok(new
        {
            Message =
                "Ticket, Notification & Audit Log Created!",

            TicketNumber =
                ticket.TicketNumber,

            Status =
                ticket.Status.ToString(),

            AssignedTo =
                assignedAgent?.UserName,

            Priority =
                ticket.Priority.ToString(),

            AiProcessed =
                ticket.IsAiProcessed,

            Deadline =
                ticket.DueAt
        });
    }


    // =========================================================
    // GET ALL TICKETS
    // Only Admin or Agent can see all tickets
    // =========================================================

    [Authorize(Roles = "Admin,Agent")]
    [HttpGet]
    public async Task<IActionResult> GetAllTickets(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        // Page number minimum 1
        if (page < 1)
        {
            page = 1;
        }

        // Page size minimum 1
        if (pageSize < 1)
        {
            pageSize = 10;
        }

        // Maximum 100 tickets per page
        if (pageSize > 100)
        {
            pageSize = 100;
        }

        var tickets = await _context.Tickets
            .OrderByDescending(t => t.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return Ok(new
        {
            Data = tickets,
            Page = page,
            PageSize = pageSize
        });
    }


    // =========================================================
    // UPDATE TICKET STATUS
    // =========================================================

    [HttpPatch("{id}/status")]
    public async Task<IActionResult> UpdateStatus(
        Guid id,
        [FromBody] TicketStatus newStatus)
    {
        var ticket =
            await _context.Tickets.FindAsync(id);

        if (ticket == null)
        {
            return NotFound();
        }


        // Status update
        ticket.Status = newStatus;


        // Resolved થાય ત્યારે ResolvedAt set કરો
        if (newStatus == TicketStatus.Resolved)
        {
            ticket.ResolvedAt =
                DateTime.UtcNow;
        }


        await _context.SaveChangesAsync();


        // =====================================================
        // RESPONSE
        // =====================================================

        return Ok(new
        {
            Message =
                "Ticket Status Updated Successfully!",

            TicketNumber =
                ticket.TicketNumber,

            Status =
                ticket.Status.ToString(),

            Priority =
                ticket.Priority.ToString(),

            AiProcessed =
                ticket.IsAiProcessed,

            Deadline =
                ticket.DueAt
        });
    }
}
