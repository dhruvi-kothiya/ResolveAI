using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ResolveAI.Domain.Entities;
using System.Net.Sockets;

namespace ResolveAI.Infrastructure.Persistence;

// IdentityDbContext વાપરવાથી યુઝર અને રોલના ટેબલ્સ આપોઆપ બની જશે
public class ApplicationDbContext : IdentityDbContext<User, Role, Guid>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // ભવિષ્યમાં આપણે અહીં બીજા ટેબલ્સ (જેમ કે Tickets) ઉમેરીશું
    public DbSet<Ticket> Tickets { get; set; }
    public DbSet<Department> Departments { get; set; }
    public DbSet<Team> Teams { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // અહીં આપણે ટેબલના નામ ટૂંકા કરી શકીએ છીએ (Optional)
        builder.Entity<User>(entity => entity.ToTable("Users"));
        builder.Entity<Role>(entity => entity.ToTable("Roles"));
    }
}