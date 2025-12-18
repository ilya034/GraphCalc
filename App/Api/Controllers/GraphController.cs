using GraphCalc.Api.Dtos;
using GraphCalc.App.Services;
using Microsoft.AspNetCore.Mvc;

namespace GraphCalc.Api.Controllers;

[ApiController]
[Route("api/graphs")]
public class GraphController : ControllerBase
{
    private readonly GraphAppService graphAppService;

    public GraphController(GraphAppService graphAppService)
    {
        this.graphAppService = graphAppService;
    }

    [HttpGet]
    public IActionResult GetAllGraphs()
    {
        var graphDtos = graphAppService.GetAllGraphs();
        return Ok(graphDtos);
    }

    [HttpGet("{id}")]
    public IActionResult GetGraphById(Guid id)
    {
        var graphDto = graphAppService.GetGraphById(id);
        return Ok(graphDto);
    }

    [HttpPost]
    public IActionResult CreateGraph([FromBody] GraphCalculationRequest request, [FromQuery] Guid authorId)
    {
        ValidateGraphCalculationRequest(request);
        ValidateGuid(authorId, nameof(authorId));
        var createdGraphDto = graphAppService.CreateGraphWithAuthor(request, authorId);
        return CreatedAtAction(nameof(GetGraphById), new { id = createdGraphDto.Id }, createdGraphDto);
    }

    [HttpPut("{id}")]
    public IActionResult UpdateGraph(Guid id, [FromBody] GraphDto graphDto)
    {
        ValidateGuid(id, nameof(id));
        ValidateGraphDto(graphDto);
        var updatedGraphDto = graphAppService.UpdateGraph(id, graphDto);
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
        var response = graphAppService.CalculateGraph(id);
        return Ok(response);
    }

    [HttpPost("calculate")]
    public IActionResult CalculateGraph([FromBody] GraphCalculationRequest request)
    {
        ValidateGraphCalculationRequest(request);
        var response = graphAppService.CalculateGraph(request);
        return Ok(response);
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
