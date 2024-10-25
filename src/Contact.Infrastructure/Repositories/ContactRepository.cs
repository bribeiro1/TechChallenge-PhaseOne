using Contacts.Domain.Contracts.Repositories;
using Contacts.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Contacts.Infrastructure.Repositories;
public class ContactRepository(AppDbContext context) : RepositoryBase<Contact>(context), IContactRepository
{
    public async Task<IEnumerable<Contact>> GetByDDDAsync(string ddd)
    {
        return await _dbSet.Where(c => c.Phone.DDD == ddd).ToListAsync();
    }
}
