using Contacts.Domain.Common;

namespace Contacts.Domain.Contacts.Models;

public class Contact : EntityBase
{
    public string Name { get; set; }
    public ContactPhone Phone { get; set; }
    public ContactEmail Email { get; set; }
}
