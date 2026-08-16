namespace ResolveAI.Domain.Entities;


public class Department
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty; // Ex =  "Information Technology"
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    //  (Relationship)
    public ICollection<Team> Teams { get; set; } = new List<Team>();
}