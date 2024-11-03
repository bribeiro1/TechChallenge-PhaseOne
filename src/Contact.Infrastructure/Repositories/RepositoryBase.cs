using Contacts.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace Contacts.Infrastructure.Repositories;

public class RepositoryBase<TEntity> : IRepositoryBase<TEntity> where TEntity : EntityBase
{
    protected readonly DbContext _context;
    protected readonly DbSet<TEntity> _dbSet;

    public RepositoryBase(DbContext context)
    {
        _context = context;
        _dbSet = _context.Set<TEntity>();
    }

    public IQueryable<TEntity> Query(bool tracking = true)
    {
        if (tracking)
            return _dbSet;
        else
            return _dbSet.AsNoTracking();
    }

    public TEntity GetById(Guid id, bool tracking = false)
    {
        if (id == Guid.Empty)
            throw new ArgumentException($"Id shouldn't be empty!");

        return Query(tracking).SingleOrDefault(r => r.Id == id);
    }

    public void Create(TEntity entity)
    {
        // Generate values for Id and CreatedAt
        entity.Id = Guid.NewGuid();
        entity.CreatedAt = DateTime.Now;

        _dbSet.Add(entity);
        _context.SaveChanges();

        _context.Entry(entity).State = EntityState.Detached;
    }

    public void Update(TEntity entity)
    {
        _dbSet.Update(entity);
        _context.SaveChanges();

        _context.Entry(entity).State = EntityState.Detached;
    }

    public void Delete(Guid id)
    {
        var entity = GetById(id, tracking: true);
        
        if (entity == null)
            throw new ArgumentException($"Id not found!");

        _dbSet.Remove(entity);
        _context.SaveChanges();
    }
}
