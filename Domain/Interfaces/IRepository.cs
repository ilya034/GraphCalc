using GraphCalc.Domain.Common;

namespace GraphCalc.Domain.Interfaces;

public interface IRepository<T> where T : Entity
{
    T? GetById(Guid id);
    IReadOnlyList<T> GetAll();
    void Add(T entity);
    void Update(T entity);
    void Delete(Guid id);
}