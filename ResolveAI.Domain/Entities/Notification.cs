namespace ResolveAI.Domain.Entities;

public class Notification
{
    public Guid Id { get; set; }
    public string Message { get; set; } = string.Empty; // ex "New Ticket INC-1008 Assigned"
    public bool IsRead { get; set; } = false; // use can see notofication
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // 
    public Guid UserId { get; set; }
    public User? User { get; set; }

    public Guid? TicketId { get; set; }
    public Ticket? Ticket { get; set; }
}