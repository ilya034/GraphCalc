namespace GraphCalc.Api.Dtos;

public class GraphCalculationRequest
{
    public string Expression { get; set; } = string.Empty;
    public double XMin { get; set; }
    public double XMax { get; set; }
    public double XStep { get; set; }
    public bool AutoYRange { get; set; } = false;
}
