using GraphCalc.Api.Dtos;
using GraphCalc.Domain.Entities;

namespace GraphCalc.App;

internal static class GraphMappingExtensions
{
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
