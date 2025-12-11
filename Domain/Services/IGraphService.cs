using GraphCalc.Api.Dtos;
using GraphCalc.Domain.ValueObjects;

namespace GraphCalc.Domain.Services;

public interface IGraphService
{
    CalculatedGraphDto CalculatePreview(string expression, NumericRange range);

    GraphSetDto CreateGraphSet(Guid userId, CreateGraphSetRequest request);
    GraphSetDto GetGraphSet(Guid id);
    IEnumerable<GraphSetDto> GetUserGraphSets(Guid userId);

    CalculatedGraphSetDto CalculateGraphSet(Guid graphSetId, NumericRange range);
}