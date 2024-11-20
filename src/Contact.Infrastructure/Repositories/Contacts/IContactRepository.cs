using Contacts.Domain.Contacts.Models;
using Contacts.Infrastructure.Repositories.Common;

namespace Contacts.Infrastructure.Repositories.Contacts;

public interface IContactRepository : IRepositoryBase<Contact>
{
    bool ContactNameAlreadyExists(string contactName, Guid ignoreGuid = default);
    bool ContactPhoneAlreadyExists(string contactPhoneDDD, string contactPhoneNumber, Guid ignoreGuid = default);
    bool ContactEmailAlreadyExists(string contactEmailAddress, Guid ignoreGuid = default);
}
