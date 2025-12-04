namespace GraphCalc.Api.Dtos;

public class SaveGraphRequest
{
    public string Expression { get; set; } = string.Empty;
    public double XMin { get; set; }
    public double XMax { get; set; }
    public double XStep { get; set; }
    public bool AutoYRange { get; set; } = false;
    public Guid UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class SaveGraphSetRequest
{
    public List<SaveGraphRequest> Graphs { get; set; } = new();
    public Guid UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
}
