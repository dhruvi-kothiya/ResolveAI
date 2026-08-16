using Microsoft.AspNetCore.Identity;

namespace ResolveAI.Domain.Entities;

public class User : IdentityUser<Guid>
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;


    public Guid? DepartmentId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}