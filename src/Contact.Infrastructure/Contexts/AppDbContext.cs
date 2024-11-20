using Contacts.Infrastructure.Mappings;
using Microsoft.EntityFrameworkCore;

namespace Contacts.Infrastructure.Contexts;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new ContactMapping());
        base.OnModelCreating(modelBuilder);
    }
}
