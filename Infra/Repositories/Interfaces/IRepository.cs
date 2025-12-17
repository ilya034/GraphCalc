using GraphCalc.Domain.Common;

namespace GraphCalc.Domain.Interfaces;

public interface IRepository<TEntity> where TEntity : Entity
{
    IEnumerable<TEntity> GetAll();
    TEntity GetById(Guid id);
    void Add(TEntity entity);   
    void Update(TEntity entity);
    void Delete(Guid id);
}
