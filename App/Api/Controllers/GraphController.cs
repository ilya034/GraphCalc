using GraphCalc.Api.Dtos;
using GraphCalc.App;
using GraphCalc.Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace GraphCalc.Api.Controllers;

[ApiController]
[Route("api/graphs")]
public class GraphController : ControllerBase
{
    private readonly IGraphAppService graphAppService;

    public GraphController(IGraphAppService graphAppService)
    {
        this.graphAppService = graphAppService;
    }

    [HttpGet]
    public IActionResult GetAllGraphs()
    {
        var graphs = graphAppService.GetAllGraphs();
        var graphDtos = graphs.ToDto();
        return Ok(graphDtos);
    }

    [HttpGet("{id}")]
    public IActionResult GetGraphById(Guid id)
    {
        var graph = graphAppService.GetGraphById(id);
        var graphDto = graph.ToDto();
        return Ok(graphDto);
    }

    [HttpPost]
    public IActionResult CreateGraph([FromBody] GraphCalculationRequest request, [FromQuery] Guid authorId)
    {
        ValidateGraphCalculationRequest(request);
        ValidateGuid(authorId, nameof(authorId));
        var createdGraph = graphAppService.CreateGraphWithAuthor(request, authorId);
        var createdGraphDto = createdGraph.ToDto();
        return CreatedAtAction(nameof(GetGraphById), new { id = createdGraphDto.Id }, createdGraphDto);
    }

    [HttpPut("{id}")]
    public IActionResult UpdateGraph(Guid id, [FromBody] GraphDto graphDto)
    {
        ValidateGuid(id, nameof(id));
        ValidateGraphDto(graphDto);
        var updatedGraph = graphAppService.UpdateGraph(id, graphDto);
        var updatedGraphDto = updatedGraph.ToDto();
        return Ok(updatedGraphDto);
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteGraph(Guid id)
    {
        graphAppService.DeleteGraph(id);
        return NoContent();
    }

    [HttpPost("{id}/calculate")]
    public IActionResult CalculateGraph(Guid id)
    {
        var series = graphAppService.CalculateGraph(id);
        var dto = new GraphCalculationResponse(series.ToDto().ToList());
        return Ok(dto);
    }

    [HttpPost("calculate")]
    public IActionResult CalculateGraph([FromBody] GraphCalculationRequest request)
    {
        ValidateGraphCalculationRequest(request);
        var series = graphAppService.CalculateGraph(request);
        var dto = new GraphCalculationResponse(series.ToDto().ToList());
        return Ok(dto);
    }

    private void ValidateGraphCalculationRequest(GraphCalculationRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(request.Range);
        ValidateNumericRange(request.Range);
    }

    private void ValidateNumericRange(NumericRangeDto range)
    {
        if (range.Min > range.Max)
            throw new ArgumentException($"Min value ({range.Min}) cannot be greater than Max value ({range.Max}).");

        if (range.Step <= 0)
            throw new ArgumentException($"Step value must be greater than 0. Provided: {range.Step}");

        if (double.IsNaN(range.Min) || double.IsNaN(range.Max) || double.IsNaN(range.Step))
            throw new ArgumentException("Range values cannot be NaN.");

        if (double.IsInfinity(range.Min) || double.IsInfinity(range.Max) || double.IsInfinity(range.Step))
            throw new ArgumentException("Range values cannot be infinite.");
    }

    private void ValidateGraphDto(GraphDto graphDto)
    {
        ArgumentNullException.ThrowIfNull(graphDto);

        ValidateGuid(graphDto.Id, nameof(graphDto.Id));
        ValidateGuid(graphDto.AuthorId, nameof(graphDto.AuthorId));

        ArgumentNullException.ThrowIfNull(graphDto.Range);

        ValidateNumericRange(graphDto.Range);

        if (graphDto.Items == null || graphDto.Items.Count == 0)
            throw new ArgumentException("Items collection cannot be null or empty.", nameof(graphDto.Items));
    }

    private void ValidateGuid(Guid id, string paramName)
    {
        if (id == Guid.Empty)
            throw new ArgumentException($"{paramName} cannot be empty.", paramName);
    }
}
