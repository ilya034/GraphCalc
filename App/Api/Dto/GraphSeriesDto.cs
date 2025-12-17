namespace GraphCalc.Api.Dtos;

public record GraphSeriesDto(
    string Expression,
    List<PointDto> Points
);