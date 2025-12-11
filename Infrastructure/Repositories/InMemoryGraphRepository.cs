using GraphCalc.Domain.Entities;
using GraphCalc.Domain.Interfaces;

namespace GraphCalc.Infrastructure.Persistence;

public class InMemoryGraphRepository : InMemoryRepository<Graph>, IGraphRepository
{
    public IEnumerable<Graph> GetByExpressionText(string expressionText)
    {
        var result = store.Values
            .Where(g => g.Expression.Text.Contains(expressionText, StringComparison.OrdinalIgnoreCase));
        
        return result;
    }
}