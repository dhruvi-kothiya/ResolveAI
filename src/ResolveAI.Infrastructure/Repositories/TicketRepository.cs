using ResolveAI.Application.Interfaces;
using ResolveAI.Domain.Entities;
using ResolveAI.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ResolveAI.Infrastructure.Repositories;

public class TicketRepository : ITicketRepository
{
    private readonly ApplicationDbContext _context;
    public TicketRepository(ApplicationDbContext context) => _context = context;

    public async Task AddAsync(Ticket ticket)
    {
        await _context.Tickets.AddAsync(ticket);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Ticket>> GetAllAsync() => await _context.Tickets.ToListAsync();
}