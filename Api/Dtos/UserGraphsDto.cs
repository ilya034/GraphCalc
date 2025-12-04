namespace GraphCalc.Api.Dtos;

public class UserGraphDto
{
    public Guid Id { get; set; }
    public string Expression { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class UserGraphSetDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public List<UserGraphDto> Graphs { get; set; } = new();
}

public class UserGraphsListResponse
{
    public Guid UserId { get; set; }
    public List<UserGraphDto> Graphs { get; set; } = new();
    public List<UserGraphSetDto> GraphSets { get; set; } = new();
}
