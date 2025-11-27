using GraphCalc.Domain.Common;

namespace GraphCalc.Domain.Interfaces;

public interface IRepository<T> where T : Entity
{
    T? GetById(Guid id);
    IReadOnlyList<T> GetAll();
    bool Add(T entity);
    bool Update(T entity);
    bool Delete(Guid id);
}