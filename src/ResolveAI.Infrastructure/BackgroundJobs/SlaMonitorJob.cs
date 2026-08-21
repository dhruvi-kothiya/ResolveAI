using ResolveAI.Infrastructure.Persistence;
using ResolveAI.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace ResolveAI.Infrastructure.BackgroundJobs;

public class SlaMonitorJob
{
    private readonly ApplicationDbContext _context;
    public SlaMonitorJob(ApplicationDbContext context) => _context = context;

    public async Task CheckSlaBreaches()
    {
        var overdueTickets = await _context.Tickets
            .Where(t => t.Status != TicketStatus.Resolved &&
                        t.Status != TicketStatus.Closed &&
                        t.DueAt < DateTime.UtcNow &&
                        !t.IsEscalated)
            .ToListAsync();

        foreach (var ticket in overdueTickets)
        {
            ticket.IsEscalated = true; 
           
        }

        await _context.SaveChangesAsync();
    }
}