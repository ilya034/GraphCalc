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
    public IActionResult CreateGraph([FromBody] GraphDto graphDto)
    {
        var createdGraphDto = graphAppService.CreateGraph(graphDto);
        return CreatedAtAction(nameof(GetGraphById), new { id = createdGraphDto.Id }, createdGraphDto);
    }

    [HttpPut("{id}")]
    public IActionResult UpdateGraph(Guid id, [FromBody] GraphDto graphDto)
    {
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
}
