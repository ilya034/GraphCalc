using GraphCalc.Domain.Common;
using GraphCalc.Domain.Interfaces;

namespace GraphCalc.Infrastructure.Persistence;

public abstract class InMemoryRepository<T> : IRepository<T> where T : Entity
{
    protected readonly Dictionary<Guid, T> store = new();

    public T? GetById(Guid id)
    {
        store.TryGetValue(id, out var entity);
        return entity;
    }

    public IReadOnlyList<T> GetAll()
    {
        var all = store.Values.ToList().AsReadOnly();
        return all;
    }

    public bool Add(T entity)
    {
        if (store.ContainsKey(entity.Id))
            throw new InvalidOperationException($"Entity with ID {entity.Id} already exists.");
        
        store[entity.Id] = entity;
        return true;
    }

    public bool Update(T entity)
    {
        if (!store.ContainsKey(entity.Id))
            throw new KeyNotFoundException($"Entity with ID {entity.Id} not found.");

        store[entity.Id] = entity;
        return true;
    }

    public bool Delete(Guid id) => store.Remove(id);
}