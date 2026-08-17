using ResolveAI.Domain.Entities;
namespace ResolveAI.Application.Interfaces;

public interface ITicketRepository
{
    Task AddAsync(Ticket ticket);
    Task<IEnumerable<Ticket>> GetAllAsync();
}