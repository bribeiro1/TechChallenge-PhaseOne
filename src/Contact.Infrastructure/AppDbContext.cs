using Contacts.Infrastructure.Mappings;
using Microsoft.EntityFrameworkCore;

namespace Contacts.Infrastructure;
public class AppDbContext : DbContext
{
    public DbSet<Domain.Entities.Contact> Contact { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new ContactMapping());
        base.OnModelCreating(modelBuilder);
    }
}
