using ResolveAI.Domain.Enums;

namespace ResolveAI.Domain.Entities;

public class Ticket
{
    public Guid Id { get; set; }
    public string TicketNumber { get; set; } = string.Empty; // ex. INC-1001
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    
    public TicketStatus Status { get; set; } = TicketStatus.New;
    public TicketPriority Priority { get; set; } = TicketPriority.Medium;

    // who  
    public Guid CreatedById { get; set; }
    public User? CreatedBy { get; set; }

    // where department
    public Guid DepartmentId { get; set; }
    public Department? Department { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ResolvedAt { get; set; }
}