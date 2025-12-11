using Microsoft.AspNetCore.Mvc;
using GraphCalc.Api.Dtos;
using GraphCalc.Domain.Services;
using GraphCalc.Domain.ValueObjects;

namespace GraphCalc.Api.Controllers;

[ApiController]
[Route("api/graph-sets")]
public class GraphSetController : ControllerBase
{
    private readonly IGraphService graphService;


    public GraphSetController(IGraphService graphService)
    {
        this.graphService = graphService;
    }

    [HttpPost]
    public IActionResult Create([FromQuery] Guid userId, [FromBody] CreateGraphSetRequest request)
    {
        var result = graphService.CreateGraphSet(userId, request);
        return CreatedAtAction(nameof(Get), new { id = result.Id }, result);
    }

    [HttpGet("{id}")]
    public IActionResult Get(Guid id)
    {
        var result = graphService.GetGraphSet(id);
        return Ok(result);
    }

    [HttpGet("user/{userId}")]
    public IActionResult GetByUser(Guid userId)
    {
        var result = graphService.GetUserGraphSets(userId);
        return Ok(result);
    }

    [HttpPost("{id}/calculate")]
    public IActionResult Calculate(Guid id, [FromBody] CalculateSetRequest request)
    {
        var result = graphService.CalculateGraphSet(id, request.Range);
        return Ok(result);
    }
}