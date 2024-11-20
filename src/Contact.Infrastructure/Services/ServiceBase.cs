using Contacts.Domain.Common;
using Contacts.Infrastructure.Repositories.Common;

namespace Contacts.Infrastructure.Services;

public class ServiceBase<TEntity, TRepository>
    where TEntity : EntityBase
    where TRepository : IRepositoryBase<TEntity>
{
    protected TRepository Repository { get; }

    public ServiceBase(TRepository repository)
    {
        Repository = repository;
    }
}
