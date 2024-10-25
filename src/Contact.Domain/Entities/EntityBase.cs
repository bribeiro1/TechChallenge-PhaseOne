namespace Contacts.Domain.Entities;
public abstract class EntityBase<TId>
{
    public TId Id { get; protected set; }

    public DateTime CreatedAt { get; protected set; }

    protected EntityBase()
    {
        CreatedAt = DateTime.Now;
    }
}
