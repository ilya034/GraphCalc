using GraphCalc.Domain.Entities;

namespace GraphCalc.Domain.Interfaces;

public interface IMathExpressionRepository
{
    MathExpression Add(MathExpression expression);
    MathExpression Update(MathExpression expression);
    MathExpression? GetById(Guid id);
    MathExpression? GetByName(string name);
    IEnumerable<MathExpression> GetAll();
    bool Remove(Guid id);
    void Clear();
}
