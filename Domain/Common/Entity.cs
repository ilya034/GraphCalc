namespace GraphCalc.Domain.Common;

public abstract class Entity
{
    public Guid Id { get; protected set; }

    protected Entity()
    {
        Id = Guid.NewGuid();
    }

    protected Entity(Guid id)
    {
        Id = id;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(this, obj))
            return true;

        if (obj is null || obj is not Entity other)
            return false;

        return Id == other.Id;
    }

    public override int GetHashCode() => Id.GetHashCode();
    public static bool operator ==(Entity? a, Entity? b) => Equals(a, b);
    public static bool operator !=(Entity? a, Entity? b) => !Equals(a, b);
}
