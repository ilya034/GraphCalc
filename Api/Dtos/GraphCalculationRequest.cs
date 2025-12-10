namespace GraphCalc.Api.Dtos;

public record GraphCalculationRequest(
    string Expression = "",
    double XMin = 0,
    double XMax = 0,
    double XStep = 0,
    bool AutoYRange = false);
