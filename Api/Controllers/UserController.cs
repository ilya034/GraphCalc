using Microsoft.AspNetCore.Mvc;
using GraphCalc.Api.Dtos;
using GraphCalc.Infrastructure.Repositories;
using GraphCalc.Domain.Interfaces;
using GraphCalc.Domain.Entities;

namespace GraphCalc.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserRepository userRepository;
    private readonly IGraphRepository graphRepository;
    private readonly InMemoryPublishedGraphRepository publishedGraphRepository;
    private readonly InMemoryGraphSetRepository graphSetRepository;

    public UserController(
        IUserRepository userRepository,
        IGraphRepository graphRepository,
        InMemoryPublishedGraphRepository publishedGraphRepository,
        InMemoryGraphSetRepository graphSetRepository)
    {
        this.userRepository = userRepository;
        this.graphRepository = graphRepository;
        this.publishedGraphRepository = publishedGraphRepository;
        this.graphSetRepository = graphSetRepository;
    }

    [HttpPost("register")]
    [ProducesResponseType(typeof(UserGraphsListResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult RegisterUser([FromBody] CreateUserRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Username))
                return BadRequest("Username cannot be empty");

            if (string.IsNullOrWhiteSpace(request.Email))
                return BadRequest("Email cannot be empty");

            var existingByEmail = userRepository.GetByEmail(request.Email);
            if (existingByEmail != null)
                return BadRequest("Email already registered");

            var existingByUsername = userRepository.GetByUsername(request.Username);
            if (existingByUsername != null)
                return BadRequest("Username already taken");

            var user = User.Create(request.Username, request.Email, request.Description);
            userRepository.Add(user);

            var response = new UserGraphsListResponse
            {
                UserId = user.Id,
                Graphs = new(),
                GraphSets = new()
            };

            return CreatedAtAction(nameof(GetUserGraphs), new { userId = user.Id }, response);
        }
        catch (Exception ex)
        {
            return BadRequest($"Error registering user: {ex.Message}");
        }
    }

    [HttpGet("{userId}/graphs")]
    [ProducesResponseType(typeof(UserGraphsListResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetUserGraphs(Guid userId)
    {
        var user = userRepository.GetById(userId);
        if (user == null)
            return NotFound($"User with ID {userId} not found");

        var publishedGraphs = publishedGraphRepository.GetByUserId(userId);
        var graphDtos = new List<UserGraphDto>();

        foreach (var publishedGraph in publishedGraphs)
        {
            var graph = graphRepository.GetById(publishedGraph.GraphId);
            if (graph != null)
            {
                graphDtos.Add(new UserGraphDto
                {
                    Id = graph.Id,
                    Expression = graph.Expression.Text,
                    Title = publishedGraph.Metadata.Title,
                    Description = publishedGraph.Metadata.Description
                });
            }
        }

        var graphSets = graphSetRepository.GetAll()
            .Where(gs => gs.Graphs.Any(g => publishedGraphRepository
                .GetByGraphId(g.Id)
                .Any(pg => pg.UserId == userId)))
            .ToList();

        var graphSetDtos = new List<UserGraphSetDto>();
        foreach (var graphSet in graphSets)
        {
            var setGraphDtos = new List<UserGraphDto>();
            foreach (var graph in graphSet.Graphs)
            {
                var published = publishedGraphRepository.GetByGraphId(graph.Id)
                    .FirstOrDefault(pg => pg.UserId == userId);

                if (published != null)
                {
                    setGraphDtos.Add(new UserGraphDto
                    {
                        Id = graph.Id,
                        Expression = graph.Expression.Text,
                        Title = published.Metadata.Title,
                        Description = published.Metadata.Description
                    });
                }
            }

            if (setGraphDtos.Count > 0)
            {
                graphSetDtos.Add(new UserGraphSetDto
                {
                    Id = graphSet.Id,
                    Title = $"GraphSet {graphSetDtos.Count + 1}",
                    Description = null,
                    Graphs = setGraphDtos
                });
            }
        }

        var response = new UserGraphsListResponse
        {
            UserId = userId,
            Graphs = graphDtos,
            GraphSets = graphSetDtos
        };

        return Ok(response);
    }

    [HttpGet("{userId}")]
    [ProducesResponseType(typeof(UserProfileResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetUser(Guid userId)
    {
        var user = userRepository.GetById(userId);
        if (user == null)
            return NotFound($"User with ID {userId} not found");

        var response = new UserProfileResponse
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            Description = user.Description,
            PublishedGraphCount = user.PublishedGraphIds.Count
        };

        return Ok(response);
    }

    [HttpPut("{userId}/description")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult UpdateUserDescription(Guid userId, [FromBody] UpdateDescriptionRequest request)
    {
        var user = userRepository.GetById(userId);
        if (user == null)
            return NotFound($"User with ID {userId} not found");

        user.UpdateDescription(request.Description);
        return Ok(new { message = "Description updated successfully" });
    }
}

public class CreateUserRequest
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class UpdateDescriptionRequest
{
    public string? Description { get; set; }
}

public class UserProfileResponse
{
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int PublishedGraphCount { get; set; }
}
