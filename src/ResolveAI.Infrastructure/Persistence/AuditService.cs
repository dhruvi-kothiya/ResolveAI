using ResolveAI.Application.Interfaces;
using ResolveAI.Domain.Entities;
using ResolveAI.Infrastructure.Persistence;

namespace ResolveAI.Infrastructure.Persistence;

public class AuditService : IAuditService
{
    private readonly ApplicationDbContext _context;

    public AuditService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task LogAsync(Guid userId, string action, string entityName, string entityId)
    {
        var log = new AuditLog
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Action = action,
            EntityName = entityName,
            EntityId = entityId,
            Timestamp = DateTime.UtcNow
        };

        _context.AuditLogs.Add(log);
        await _context.SaveChangesAsync();
    }
}