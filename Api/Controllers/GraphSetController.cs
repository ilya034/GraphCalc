using Microsoft.AspNetCore.Mvc;
using GraphCalc.Domain.Services;
using GraphCalc.Api.Dtos;
using GraphCalc.Domain.ValueObjects;

namespace GraphCalc.Api.Controllers;

[ApiController]
[Route("api/projects")]
public class GraphSetController : ControllerBase
{
    private readonly IGraphCalculationService calculationService;

    public GraphSetController(IGraphCalculationService calculationService)
    {
        this.calculationService = calculationService;
    }

    [HttpPost]
    public IActionResult CreateGraphSet(
        [FromBody] SaveGraphSetRequest request,
        [FromQuery] Guid userId)
    {
        if (request == null || request.Graphs == null || request.Graphs.Count == 0)
            return BadRequest("GraphSet must contain at least one graph");

        try
        {
            var result = calculationService.CreateAndCalculateGraphSet(
                request.Graphs,
                request.Title,
                request.Description,
                userId,
                request.GlobalRange);

            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{id}")]
    public IActionResult GetGraphSet(Guid id)
    {
        try
        {
            var result = calculationService.CalculateGraphSet(id);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpPost("{id}/items")]
    public IActionResult AddGraphToSet(
        Guid id,
        [FromBody] SaveGraphRequest graphRequest)
    {
        try
        {
            var result = calculationService.AddGraphToSet(id, graphRequest);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}/items/{itemId}")]
    public IActionResult UpdateGraphInSet(
        Guid id,
        Guid itemId,
        [FromBody] UpdateExpressionRequest request)
    {
        try
        {
            var result = calculationService.UpdateGraphInSet(id, itemId, request.NewExpression);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message ?? "An error occurred");
        }
    }
}

public record SaveGraphSetRequest(
    string Title,
    string? Description = null,
    NumericRange? GlobalRange = null,
    List<SaveGraphRequest> Graphs = null);

public record UpdateExpressionRequest(string NewExpression);