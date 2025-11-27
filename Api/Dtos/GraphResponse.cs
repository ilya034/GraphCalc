namespace GraphCalc.Api.Dtos;

public class GraphResponse
{
    public Guid Id { get; set; }
    public string Expression { get; set; } = string.Empty;
    public string IndependentVariable { get; set; } = string.Empty;
    public List<MathPointDto> Points { get; set; } = new();
    public NumericRangeDto? Range { get; set; }
}

public class MathPointDto
{
    public double X { get; set; }
    public double Y { get; set; }
}

public class NumericRangeDto
{
    public double Min { get; set; }
    public double Max { get; set; }
    public double Step { get; set; }
}
