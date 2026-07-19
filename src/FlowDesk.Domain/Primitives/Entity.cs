using FlowDesk.Domain.Exceptions;
 
namespace FlowDesk.Domain.Primitives;

public abstract class Entity<TId> : IEquatable<Entity<TId>>
where TId : notnull
{
    public TId Id { get; protected set; }

    protected Entity(TId id)
    {
        if(id is null)
        throw new DomainException("Id cannot be null.");
        Id = id;
    }

    public bool Equals(Entity<TId>? other)
    {
        if(other is null)
        return false;

        if (ReferenceEquals(this, other))
        return true;

        if(other.GetType() != GetType())
        return false;

        return EqualityComparer<TId>.Default.Equals(Id, other.Id);
    }

    public override bool Equals(object? obj)
    {
        return obj is Entity<TId> other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(GetType(), Id);
    }

    public static bool operator ==(Entity<TId>? left, Entity<TId>? right)
    {
        if (left is null)
            return right is null;

        return left.Equals(right);
    }
    public static bool operator !=(Entity<TId>? left, Entity<TId>? right)
    {
        return !(left == right);
    }
}

