using Microsoft.AspNetCore.Mvc;
using GraphCalc.Api.Dtos;
using GraphCalc.Api.Mappers;
using GraphCalc.Domain.Services;
using GraphCalc.Domain.Interfaces;
using GraphCalc.Domain.Entities;

namespace GraphCalc.Api.Controllers;

[ApiController]
[Route("api/graphcalculation")]
public class GraphCalculationController : ControllerBase
{
    private readonly IGraphCalculationService _graphCalculationService;
    private readonly IGraphRepository _graphRepository;

    public GraphCalculationController(
        IGraphCalculationService graphCalculationService,
        IGraphRepository graphRepository)
    {
        _graphCalculationService = graphCalculationService;
        _graphRepository = graphRepository;
    }

    [HttpPost("calculate")]
    [ProducesResponseType(typeof(GraphResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult Calculate([FromBody] GraphCalculationRequest request)
    {
        try
        {
            var graph = request.AutoYRange
                ? _graphCalculationService.CalculateGraphWithAutoYRange(
                    request.Expression,
                    request.XMin,
                    request.XMax,
                    request.XStep)
                : _graphCalculationService.CalculateGraph(
                    request.Expression,
                    request.XMin,
                    request.XMax,
                    request.XStep);

            var response = GraphToResponseMapper.Map(graph);
            return Ok(response);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
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
            var graph = _graphCalculationService.CalculateAndSaveGraph(
                request.Expression,
                request.XMin,
                request.XMax,
                request.XStep,
                request.AutoYRange);

            var response = GraphToResponseMapper.Map(graph);
            return CreatedAtAction(nameof(GetGraphById), new { id = graph.Id }, response);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
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
        var graph = _graphRepository.GetById(id);
        if (graph == null)
            return NotFound($"Graph with ID {id} not found");

        var response = GraphToResponseMapper.Map(graph);
        return Ok(response);
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<GraphResponse>), StatusCodes.Status200OK)]
    public IActionResult GetAllGraphs()
    {
        var graphs = _graphRepository.GetAll();
        var responses = graphs.Select(GraphToResponseMapper.Map).ToList();
        return Ok(responses);
    }

    [HttpGet("search")]
    [ProducesResponseType(typeof(List<GraphResponse>), StatusCodes.Status200OK)]
    public IActionResult SearchByExpression([FromQuery] string expressionText)
    {
        if (string.IsNullOrWhiteSpace(expressionText))
            return BadRequest("Expression text cannot be empty");

        var graphs = _graphRepository.GetByExpressionText(expressionText);
        var responses = graphs.Select(GraphToResponseMapper.Map).ToList();
        return Ok(responses);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult DeleteGraph(Guid id)
    {
        var success = _graphRepository.Delete(id);
        if (!success)
            return NotFound($"Graph with ID {id} not found");

        return NoContent();
    }

    [HttpPost("save")]
    [ProducesResponseType(typeof(GraphResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult SaveGraph([FromBody] SaveGraphRequest request)
    {
        try
        {
            var graph = _graphCalculationService.SaveGraph(
                request.Expression,
                request.XMin,
                request.XMax,
                request.XStep,
                request.AutoYRange,
                request.Title,
                request.Description,
                request.UserId);

            var response = GraphToResponseMapper.Map(graph);
            return CreatedAtAction(nameof(GetGraphById), new { id = graph.Id }, response);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest($"Error saving graph: {ex.Message}");
        }
    }

    [HttpPost("saveset")]
    [ProducesResponseType(typeof(UserGraphSetDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult SaveGraphSet([FromBody] SaveGraphSetRequest request)
    {
        try
        {
            var graphSet = _graphCalculationService.SaveGraphSet(
                request.Graphs,
                request.Title,
                request.Description,
                request.UserId);

            var graphDtos = new List<UserGraphDto>();
            foreach (var graph in graphSet.Graphs)
            {
                graphDtos.Add(new UserGraphDto(
                    Id: graph.Id,
                    Expression: graph.Expression.Text,
                    Title: $"Graph {graphDtos.Count + 1}",
                    Description: null
                ));
            }

            var response = new UserGraphSetDto(
                Id: graphSet.Id,
                Title: request.Title,
                Description: request.Description,
                Graphs: graphDtos
            );

            return CreatedAtAction(nameof(GetGraphById), new { id = graphSet.Id }, response);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest($"Error saving graph set: {ex.Message}");
        }
    }

    [HttpGet("user/{userId}/graphs")]
    [ProducesResponseType(typeof(UserGraphsListResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetUserGraphs(Guid userId)
    {
        // Этот метод будет перенесен в UserController
        return BadRequest("This endpoint has been moved to UserController");
    }
}

