using GraphCalc.Api.Dtos;
using GraphCalc.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace GraphCalc.Api.Controllers;

[ApiController]
[Route("api/graph")]
public class GraphController : ControllerBase
{
    private readonly GraphAppService graphAppService;

    public GraphController(GraphAppService graphAppService)
    {
        this.graphAppService = graphAppService;
    }

    [HttpPost("calculate")]
    public IActionResult Calculate([FromBody] GraphCalculationRequest request)
    {
        var response = graphAppService.CalculateGraph(request);
        return Ok(response);
    }
}