using GraphCalc.Domain.Entities;
using GraphCalc.Domain.ValueObjects;
using GraphCalc.Api.Dtos;

namespace GraphCalc.Domain.Services;

public interface IGraphCalculationService
{
    UserGraphSetDto CalculateGraphSet(Guid graphSetId);
    UserGraphSetDto CreateAndCalculateGraphSet(
        List<SaveGraphRequest> graphRequests,
        string title,
        string? description,
        Guid userId,
        NumericRange? globalRange = null);
    UserGraphSetDto AddGraphToSet(
        Guid graphSetId,
        SaveGraphRequest graphRequest);
    UserGraphSetDto UpdateGraphInSet(
        Guid graphSetId,
        Guid graphItemId,
        string newExpression);
}