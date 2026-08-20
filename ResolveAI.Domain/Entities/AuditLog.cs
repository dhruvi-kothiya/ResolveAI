namespace ResolveAI.Domain.Entities;

public class AuditLog
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; } 
    public string Action { get; set; } = string.Empty; // ex. "Created Ticket", "Resolved Ticket"
    public string EntityName { get; set; } = string.Empty; //  (Tickets, Users)
    public string EntityId { get; set; } = string.Empty; 
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}