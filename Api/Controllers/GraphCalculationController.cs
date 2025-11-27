using Microsoft.AspNetCore.Mvc;
using GraphCalc.Api.Dtos;
using GraphCalc.Api.Mappers;
using GraphCalc.Infrastructure.Facade;
using GraphCalc.Domain.Interfaces;

namespace GraphCalc.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GraphCalculationController : ControllerBase
{
    private readonly GraphCalculationFacade calculationFacade;
    private readonly IGraphRepository graphRepository;

    public GraphCalculationController(
        GraphCalculationFacade calculationFacade,
        IGraphRepository graphRepository)
    {
        this.calculationFacade = calculationFacade;
        this.graphRepository = graphRepository;
    }

    [HttpPost("calculate")]
    [ProducesResponseType(typeof(GraphResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult Calculate([FromBody] GraphCalculationRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Expression))
                return BadRequest("Expression cannot be empty");

            if (request.XMin >= request.XMax)
                return BadRequest("XMin must be less than XMax");

            if (request.XStep <= 0)
                return BadRequest("XStep must be greater than 0");

            var graph = request.AutoYRange
                ? calculationFacade.GetGraphWithAutoYRange(
                    request.Expression,
                    request.XMin,
                    request.XMax,
                    request.XStep)
                : calculationFacade.GetGraph(
                    request.Expression,
                    request.XMin,
                    request.XMax,
                    request.XStep);

            var response = GraphToResponseMapper.Map(graph);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest($"Error calculating graph: {ex.Message}");
        }
    }

    [HttpPost("calculate-and-save")]
    [ProducesResponseType(typeof(GraphResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult CalculateAndSave([FromBody] GraphCalculationRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Expression))
                return BadRequest("Expression cannot be empty");

            if (request.XMin >= request.XMax)
                return BadRequest("XMin must be less than XMax");

            if (request.XStep <= 0)
                return BadRequest("XStep must be greater than 0");

            var graph = request.AutoYRange
                ? calculationFacade.GetGraphWithAutoYRange(
                    request.Expression,
                    request.XMin,
                    request.XMax,
                    request.XStep)
                : calculationFacade.GetGraph(
                    request.Expression,
                    request.XMin,
                    request.XMax,
                    request.XStep);

            graphRepository.Add(graph);

            var response = GraphToResponseMapper.Map(graph);
            return CreatedAtAction(nameof(GetGraphById), new { id = graph.Id }, response);
        }
        catch (Exception ex)
        {
            return BadRequest($"Error calculating and saving graph: {ex.Message}");
        }
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(GraphResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetGraphById(Guid id)
    {
        var graph = graphRepository.GetById(id);
        if (graph == null)
            return NotFound($"Graph with ID {id} not found");

        var response = GraphToResponseMapper.Map(graph);
        return Ok(response);
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<GraphResponse>), StatusCodes.Status200OK)]
    public IActionResult GetAllGraphs()
    {
        var graphs = graphRepository.GetAll();
        var responses = graphs.Select(GraphToResponseMapper.Map).ToList();
        return Ok(responses);
    }

    [HttpGet("search")]
    [ProducesResponseType(typeof(List<GraphResponse>), StatusCodes.Status200OK)]
    public IActionResult SearchByExpression([FromQuery] string expressionText)
    {
        if (string.IsNullOrWhiteSpace(expressionText))
            return BadRequest("Expression text cannot be empty");

        var graphs = graphRepository.GetByExpressionText(expressionText);
        var responses = graphs.Select(GraphToResponseMapper.Map).ToList();
        return Ok(responses);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult DeleteGraph(Guid id)
    {
        var success = graphRepository.Delete(id);
        if (!success)
            return NotFound($"Graph with ID {id} not found");

        return NoContent();
    }
}
