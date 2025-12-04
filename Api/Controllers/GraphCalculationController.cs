using Microsoft.AspNetCore.Mvc;
using GraphCalc.Api.Dtos;
using GraphCalc.Api.Mappers;
using GraphCalc.Infrastructure.Facade;
using GraphCalc.Infrastructure.Repositories;
using GraphCalc.Domain.Interfaces;
using GraphCalc.Domain.Entities;

namespace GraphCalc.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GraphCalculationController : ControllerBase
{
    private readonly GraphCalculationFacade calculationFacade;
    private readonly IGraphRepository graphRepository;
    private readonly IUserRepository userRepository;
    private readonly InMemoryPublishedGraphRepository publishedGraphRepository;
    private readonly InMemoryGraphSetRepository graphSetRepository;

    public GraphCalculationController(
        GraphCalculationFacade calculationFacade,
        IGraphRepository graphRepository,
        IUserRepository userRepository,
        InMemoryPublishedGraphRepository publishedGraphRepository,
        InMemoryGraphSetRepository graphSetRepository)
    {
        this.calculationFacade = calculationFacade;
        this.graphRepository = graphRepository;
        this.userRepository = userRepository;
        this.publishedGraphRepository = publishedGraphRepository;
        this.graphSetRepository = graphSetRepository;
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

    [HttpPost("save")]
    [ProducesResponseType(typeof(GraphResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult SaveGraph([FromBody] SaveGraphRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Expression))
                return BadRequest("Expression cannot be empty");

            if (request.XMin >= request.XMax)
                return BadRequest("XMin must be less than XMax");

            if (request.XStep <= 0)
                return BadRequest("XStep must be greater than 0");

            if (string.IsNullOrWhiteSpace(request.Title))
                return BadRequest("Title cannot be empty");

            var user = userRepository.GetById(request.UserId);
            if (user == null)
                return NotFound($"User with ID {request.UserId} not found");

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

            var publishedGraph = PublishedGraph.Create(
                request.UserId,
                graph.Id,
                request.Title,
                request.Description);

            publishedGraphRepository.Add(publishedGraph);
            user.PublishGraph(graph.Id);

            var response = GraphToResponseMapper.Map(graph);
            return CreatedAtAction(nameof(GetGraphById), new { id = graph.Id }, response);
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
            if (request.Graphs == null || request.Graphs.Count == 0)
                return BadRequest("GraphSet must contain at least one graph");

            if (string.IsNullOrWhiteSpace(request.Title))
                return BadRequest("Title cannot be empty");

            var user = userRepository.GetById(request.UserId);
            if (user == null)
                return NotFound($"User with ID {request.UserId} not found");

            var graphSet = GraphSet.Create();
            var graphDtos = new List<UserGraphDto>();

            foreach (var graphRequest in request.Graphs)
            {
                if (string.IsNullOrWhiteSpace(graphRequest.Expression))
                    return BadRequest("All graphs must have an expression");

                if (graphRequest.XMin >= graphRequest.XMax || graphRequest.XStep <= 0)
                    return BadRequest("Invalid range or step for graph");

                var graph = graphRequest.AutoYRange
                    ? calculationFacade.GetGraphWithAutoYRange(
                        graphRequest.Expression,
                        graphRequest.XMin,
                        graphRequest.XMax,
                        graphRequest.XStep)
                    : calculationFacade.GetGraph(
                        graphRequest.Expression,
                        graphRequest.XMin,
                        graphRequest.XMax,
                        graphRequest.XStep);

                graphRepository.Add(graph);
                graphSet.AddGraph(graph);

                var publishedGraph = PublishedGraph.Create(
                    request.UserId,
                    graph.Id,
                    graphRequest.Title ?? $"Graph {graphDtos.Count + 1}",
                    graphRequest.Description);

                publishedGraphRepository.Add(publishedGraph);
                user.PublishGraph(graph.Id);

                graphDtos.Add(new UserGraphDto
                {
                    Id = graph.Id,
                    Expression = graph.Expression.Text,
                    Title = graphRequest.Title ?? $"Graph {graphDtos.Count}",
                    Description = graphRequest.Description
                });
            }

            graphSetRepository.Add(graphSet);

            var response = new UserGraphSetDto
            {
                Id = graphSet.Id,
                Title = request.Title,
                Description = request.Description,
                Graphs = graphDtos
            };

            return CreatedAtAction(nameof(GetUserGraphs), new { userId = request.UserId }, response);
        }
        catch (Exception ex)
        {
            return BadRequest($"Error saving graph set: {ex.Message}");
        }
    }
}

