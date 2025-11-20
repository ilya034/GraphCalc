using GraphCalc.Domain.Entities;
using GraphCalc.Domain.Interfaces;

namespace GraphCalc.Infrastructure.Repositories;

public sealed class InMemoryMathExpressionRepository : IMathExpressionRepository
{
    private readonly Dictionary<Guid, MathExpression> expressions = new();
    private readonly Dictionary<string, Guid> nameIndex = new(StringComparer.OrdinalIgnoreCase);

    public MathExpression Add(MathExpression expression)
    {
        ArgumentNullException.ThrowIfNull(expression);

        expressions[expression.Id] = expression;
        IndexName(expression);
        return expression;
    }

    public MathExpression Update(MathExpression expression)
    {
        ArgumentNullException.ThrowIfNull(expression);

        if (!expressions.ContainsKey(expression.Id))
            throw new KeyNotFoundException($"Math expression with id {expression.Id} not found");

        UnIndexName(expression.Id);
        expressions[expression.Id] = expression;
        IndexName(expression);
        return expression;
    }

    public MathExpression? GetById(Guid id)
    {
        return expressions.TryGetValue(id, out var expression) ? expression : null;
    }

    public MathExpression? GetByName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return null;

        if (!nameIndex.TryGetValue(name, out var id))
            return null;

        return expressions.TryGetValue(id, out var expression) ? expression : null;
    }

    public IEnumerable<MathExpression> GetAll()
    {
        return expressions.Values.ToList();
    }

    public bool Remove(Guid id)
    {
        if (!expressions.Remove(id, out var removed))
            return false;

        if (!string.IsNullOrWhiteSpace(removed.Name))
            nameIndex.Remove(removed.Name);

        return true;
    }

    public void Clear()
    {
        expressions.Clear();
        nameIndex.Clear();
    }

    private void IndexName(MathExpression expression)
    {
        if (!string.IsNullOrWhiteSpace(expression.Name))
            nameIndex[expression.Name] = expression.Id;
    }

    private void UnIndexName(Guid expressionId)
    {
        if (!expressions.TryGetValue(expressionId, out var existing) || string.IsNullOrWhiteSpace(existing.Name))
            return;

        nameIndex.Remove(existing.Name);
    }
}
