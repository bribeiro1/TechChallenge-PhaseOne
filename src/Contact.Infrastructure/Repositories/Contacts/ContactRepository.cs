using Contacts.Domain.Contacts.Models;
using Contacts.Infrastructure.Contexts;
using Contacts.Infrastructure.Repositories.Common;

namespace Contacts.Infrastructure.Repositories.Contacts;

public class ContactRepository : RepositoryBase<Contact>, IContactRepository
{
    public ContactRepository(AppDbContext context)
        : base(context)
    {
    }

    public bool ContactNameAlreadyExists(string contactName, Guid ignoreGuid = default)
    {
        var query = Query().Where(r => r.Name == contactName);

        if (ignoreGuid != Guid.Empty)
            query = query.Where(r => r.Id != ignoreGuid);

        return query.Any();
    }

    public bool ContactPhoneAlreadyExists(string contactPhoneDDD, string contactPhoneNumber, Guid ignoreGuid = default)
    {
        var query = Query().Where(r => r.Phone.DDD == contactPhoneDDD && r.Phone.Number == contactPhoneNumber);

        if (ignoreGuid != Guid.Empty)
            query = query.Where(r => r.Id != ignoreGuid);

        return query.Any();
    }

    public bool ContactEmailAlreadyExists(string contactEmailAddress, Guid ignoreGuid = default)
    {
        var query = Query().Where(r => r.Email.Address == contactEmailAddress);

        if (ignoreGuid != Guid.Empty)
            query = query.Where(r => r.Id != ignoreGuid);

        return query.Any();
    }
}
