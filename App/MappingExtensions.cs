using GraphCalc.Api.Dtos;
using GraphCalc.Domain.Entities;
using GraphCalc.Domain.ValueObjects;

namespace GraphCalc.App;

public static class GraphMappingExtensions
{
    public static GraphItem ToDomain(this GraphItemDto dto)
        => new GraphItem(dto.Expression, dto.IsVisible);

    public static GraphItemDto ToDto(this GraphItem domain)
        => new GraphItemDto(domain.Expression, domain.IsVisible);

    public static IEnumerable<GraphItem> ToDomain(this IEnumerable<GraphItemDto> dtos)
        => dtos.Select(dto => dto.ToDomain());

    public static IEnumerable<GraphItemDto> ToDto(this IEnumerable<GraphItem> domains)
        => domains.Select(domain => domain.ToDto());

    public static NumericRange ToDomain(this NumericRangeDto dto)
        => new NumericRange(dto.Min, dto.Max, dto.Step);

    public static NumericRangeDto ToDto(this NumericRange domain)
        => new NumericRangeDto(domain.Min, domain.Max, domain.Step);

    public static MathPoint ToDomain(this PointDto dto)
        => new MathPoint(dto.X, dto.Y);

    public static PointDto ToDto(this MathPoint domain)
        => new PointDto(domain.X, domain.Y);

    public static IEnumerable<MathPoint> ToDomain(this IEnumerable<PointDto> dtos)
        => dtos.Select(dto => dto.ToDomain());

    public static IEnumerable<PointDto> ToDto(this IEnumerable<MathPoint> domains)
        => domains.Select(domain => domain.ToDto());

    public static Series ToDomain(this GraphSeriesDto dto)
        => new Series(dto.Expression, dto.Points.ToDomain());

    public static GraphSeriesDto ToDto(this Series domain)
        => new GraphSeriesDto(domain.Expression, domain.Points.ToDto().ToList());

    public static IEnumerable<Series> ToDomain(this IEnumerable<GraphSeriesDto> dtos)
        => dtos.Select(dto => dto.ToDomain());

    public static IEnumerable<GraphSeriesDto> ToDto(this IEnumerable<Series> domains)
        => domains.Select(domain => domain.ToDto());

    // User mappings
    public static User ToDomain(this UserDto dto)
        => User.CreateWithId(dto.Id, dto.Username, dto.Email);

    public static User ToDomain(this UserCreateRequest request)
        => User.Create(request.Username, request.Email);

    public static UserDto ToDto(this User domain)
        => new UserDto(domain.Id, domain.Username, domain.Email);

    public static IEnumerable<UserDto> ToDto(this IEnumerable<User> domains)
        => domains.Select(domain => domain.ToDto());

    // Graph mappings
    public static Graph ToDomain(this GraphDto dto)
        => Graph.CreateWithId(dto.Id, dto.Range.ToDomain(), dto.AuthorId, dto.Items.ToDomain().ToList());

    public static Graph ToDomain(this GraphCreateRequest request)
        => Graph.Create(request.Range.ToDomain(), request.AuthorId, request.Items.ToDomain().ToList());

    public static Graph ToDomain(this GraphCalculationRequest request, Guid authorId)
        => Graph.Create(request.Range.ToDomain(), authorId, request.Items.ToDomain().ToList());

    public static GraphDto ToDto(this Graph domain)
        => new GraphDto(domain.Id, domain.Range.ToDto(), domain.AuthorId, domain.Items.ToDto().ToList());

    public static IEnumerable<Graph> ToDomain(this IEnumerable<GraphDto> dtos)
        => dtos.Select(dto => dto.ToDomain());

    public static IEnumerable<GraphDto> ToDto(this IEnumerable<Graph> domains)
        => domains.Select(domain => domain.ToDto());
}