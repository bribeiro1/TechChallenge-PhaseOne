using Contacts.Domain.Entities;

namespace Contacts.Domain.Contracts.Repositories;
public interface IContactRepository : IRepositoryBase<Contact>
{
    Task<IEnumerable<Contact>> GetByDDDAsync(string ddd);
}
