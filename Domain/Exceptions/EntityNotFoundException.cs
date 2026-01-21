namespace GraphCalc.Domain.Exceptions;

/// <summary>
/// Исключение при попытке найти несуществующую сущность
/// </summary>
public class EntityNotFoundException : DomainException
{
    public Guid EntityId { get; set; }
    public string EntityType { get; set; }

    public EntityNotFoundException(string entityType, Guid entityId)
        : base($"{entityType} with ID {entityId} was not found.", "ENTITY_NOT_FOUND")
    {
        EntityType = entityType;
        EntityId = entityId;
    }
}
