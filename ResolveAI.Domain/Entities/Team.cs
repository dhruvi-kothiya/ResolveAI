namespace ResolveAI.Domain.Entities;

public class Team
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty; // Ex. "Software Development"
    public Guid DepartmentId { get; set; } // 

    // Navigation Property
    public Department? Department { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}