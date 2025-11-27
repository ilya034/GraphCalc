using GraphCalc.Domain.Entities;

namespace GraphCalc.Domain.Interfaces;

public interface IGraphRepository : IRepository<Graph>
{
    IEnumerable<Graph> GetByExpressionText(string expressionText);
}