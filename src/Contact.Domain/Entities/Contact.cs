using Contacts.Domain.ValueObjects;

namespace Contacts.Domain.Entities;
public class Contact : EntityBase<Guid>
{
    public string Name { get; private set; } = string.Empty;
    public Phone Phone { get; private set; } = null!;
    public Email Email { get; private set; } = null!;

    public Contact() { }

    public Contact(string name, Phone phone, Email email)
    {
        Name = name;
        Phone = phone;
        Email = email;
    }
}
