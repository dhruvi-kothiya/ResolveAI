using ResolveAI.Domain.Enums;

namespace ResolveAI.Domain.Entities;

public class Ticket
{
    public Guid Id { get; set; }

    public string TicketNumber { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;


    // =========================================================
    // TICKET STATUS & PRIORITY
    // =========================================================

    public TicketStatus Status { get; set; } = TicketStatus.New;

    public TicketPriority Priority { get; set; } = TicketPriority.Medium;


    // =========================================================
    // AI
    // =========================================================

    public bool IsAiProcessed { get; set; } = false;

    public string? ResolutionSummary { get; set; }

    public DateTime? ResolvedAt { get; set; }


    // =========================================================
    // SLA / RESOLUTION TRACKING
    // =========================================================

    public DateTime? FirstResponseAt { get; set; }

    public DateTime? ClosedAt { get; set; }

    public string? ResolutionCode { get; set; }


    // =========================================================
    // CREATED
    // =========================================================

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;


    // =========================================================
    // CONNECTION
    // =========================================================

    // Employee who created the ticket
    public Guid CreatedById { get; set; }

    public User? CreatedBy { get; set; }


    // Department of the ticket
    public Guid DepartmentId { get; set; }

    public Department? Department { get; set; }


    // Agent assigned to the ticket
    public Guid? AssignedToId { get; set; }

    public User? AssignedTo { get; set; }


    // =========================================================
    // SLA
    // =========================================================

    public DateTime DueAt { get; set; }

    public bool IsEscalated { get; set; } = false;
}