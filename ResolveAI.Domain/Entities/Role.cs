using Microsoft.AspNetCore.Identity;

namespace ResolveAI.Domain.Entities;

public class Role : IdentityRole<Guid>
{
    public string Description { get; set; } = string.Empty;
}