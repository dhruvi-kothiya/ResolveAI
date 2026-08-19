using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ResolveAI.Domain.Entities;
using System.Net.Sockets;

namespace ResolveAI.Infrastructure.Persistence;

// IdentityDbContext use atometic creat
public class ApplicationDbContext : IdentityDbContext<User, Role, Guid>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // future add table
    public DbSet<Ticket> Tickets { get; set; }
    public DbSet<Department> Departments { get; set; }
    public DbSet<Team> Teams { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<KnowledgeCategory> KnowledgeCategories { get; set; }
    public DbSet<KnowledgeArticle> KnowledgeArticles { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        
        builder.Entity<User>(entity => entity.ToTable("Users"));
        builder.Entity<Role>(entity => entity.ToTable("Roles"));

    }
}