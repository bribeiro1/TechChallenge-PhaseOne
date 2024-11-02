using Contacts.Infrastructure.Models;
using Contacts.Infrastructure.Repositories;

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
