using GraphCalc.Domain.ValueObjects;

namespace GraphCalc.Api.Dtos;

public record SaveGraphRequest(
    string Expression = "",
    NumericRange? XRange = null,
    bool AutoYRange = false,
    Guid UserId = default,
    string Title = "",
    string? Description = null);
