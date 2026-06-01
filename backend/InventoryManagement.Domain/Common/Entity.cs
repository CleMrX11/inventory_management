namespace InventoryManagement.Domain.Common;

public abstract class Entity<TId>
    where TId : notnull
{
    public TId Id { get; protected set; }

    protected Entity()
    {
        Id = default!;
    }

    protected Entity(TId id)
    {
        Id = id;
    }
}
