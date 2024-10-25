using Contacts.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Contacts.Infrastructure.Mappings;
public class ContactMapping : IEntityTypeConfiguration<Contact>
{
    public void Configure(EntityTypeBuilder<Contact> builder)
    {
        builder.ToTable("Contacts");
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name)
            .IsRequired()
            .HasColumnName("Name")
            .HasColumnType("VARCHAR(100)")
            .HasMaxLength(100);

        builder.OwnsOne(c => c.Phone, p =>
        {
            p.Property(t => t.Number)
                .IsRequired()
                .HasColumnName("PhoneNumber")
                .HasColumnType("VARCHAR(9)")
                .HasMaxLength(9);

            p.Property(t => t.DDD)
                .IsRequired()
                .HasColumnName("PhoneDDD")
                .HasColumnType("VARCHAR(2)")
                .HasMaxLength(2);
        });

        builder.OwnsOne(c => c.Email, e =>
        {
            e.Property(e => e.Address)
                .IsRequired()
                .HasColumnName("Email")
                .HasColumnType("VARCHAR(100)")
                .HasMaxLength(100);
        });
    }
}
