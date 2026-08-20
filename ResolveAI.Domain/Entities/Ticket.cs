using ResolveAI.Domain.Enums;

namespace ResolveAI.Domain.Entities;

public class Ticket
{
    public Guid Id { get; set; }
    public string TicketNumber { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public TicketStatus Status { get; set; } = TicketStatus.New;
    public TicketPriority Priority { get; set; } = TicketPriority.Medium;

    // AI
    public bool IsAiProcessed { get; set; } = false;
    public string? ResolutionSummary { get; set; }
    public DateTime? ResolvedAt { get; set; }

    // SLA / Resolution tracking
    public DateTime? FirstResponseAt { get; set; }
    public DateTime? ClosedAt { get; set; }
    public string? ResolutionCode { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Connection
    public Guid CreatedById { get; set; }
    public User? CreatedBy { get; set; }

    public Guid DepartmentId { get; set; }
    public Department? Department { get; set; }

    public DateTime DueAt { get; set; }
    public bool IsEscalated { get; set; } = false;
}