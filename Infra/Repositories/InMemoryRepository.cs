using GraphCalc.Domain.Common;
using GraphCalc.Domain.Interfaces;
using System.Collections.Concurrent;

namespace GraphCalc.Infra.Repositories;

public class InMemoryRepository<TEntity> : IRepository<TEntity> where TEntity : Entity
{
    protected readonly ConcurrentDictionary<Guid, TEntity> entities = new();

    public virtual IEnumerable<TEntity> GetAll()
    {
        return entities.Values.AsEnumerable();
    }

    public virtual TEntity GetById(Guid id)
    {
        if (entities.TryGetValue(id, out var entity))
            return entity;

        throw new KeyNotFoundException($"Entity with id {id} not found.");
    }

    public virtual void Add(TEntity entity)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));
        
        entities.TryAdd(entity.Id, entity);
    }

    public virtual void Update(TEntity entity)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));
        
        if (!entities.ContainsKey(entity.Id))
            throw new KeyNotFoundException($"Entity with id {entity.Id} not found.");
        
        entities[entity.Id] = entity;
    }

    public virtual void Delete(Guid id)
    {
        if (!entities.ContainsKey(id))
            throw new KeyNotFoundException($"Entity with id {id} not found.");
        
        entities.TryRemove(id, out _);
    }
}