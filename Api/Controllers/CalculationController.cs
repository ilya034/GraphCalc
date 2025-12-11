using Microsoft.AspNetCore.Mvc;
using GraphCalc.Api.Dtos;
using GraphCalc.Domain.Services;

namespace GraphCalc.Api.Controllers;

[ApiController]
[Route("api/calculation")]
public class CalculationController : ControllerBase
{
    private readonly IGraphService graphService;

    public CalculationController(IGraphService graphService)
    {
        this.graphService = graphService;
    }

    [HttpPost("preview")]
    public IActionResult CalculatePreview([FromBody] CalculatePreviewRequest request)
    {
        var result = graphService.CalculatePreview(request.Expression, request.Range);
        return Ok(result);
    }
}