using Contacts.Domain.Common;

namespace Contacts.Infrastructure.Repositories.Common;

public interface IRepositoryBase<TEntity> where TEntity : EntityBase
{
    IQueryable<TEntity> Query(bool tracking = true);
    TEntity GetById(Guid id, bool tracking = false);
    void Create(TEntity entity);
    void Update(TEntity entity);
    void Delete(Guid id);
}