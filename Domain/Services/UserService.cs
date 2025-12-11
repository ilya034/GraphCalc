using GraphCalc.Domain.Entities;
using GraphCalc.Domain.Interfaces;
using GraphCalc.Infrastructure.Repositories;
using GraphCalc.Api.Dtos;

namespace GraphCalc.Domain.Services;

public class UserService : IUserService
{
    private readonly IUserRepository userRepository;
    private readonly IGraphSetRepository graphSetRepository;

    public UserService(
        IUserRepository userRepository,
        IGraphRepository graphRepository)
    {
        this.userRepository = userRepository;
        this.graphRepository = graphRepository;
    }

    public User RegisterUser(string username, string email, string? description)
    {
        ValidateUserRegistration(username, email);

        var existingByEmail = userRepository.GetByEmail(email);
        if (existingByEmail != null)
            throw new InvalidOperationException("Email already registered");

        var existingByUsername = userRepository.GetByUsername(username);
        if (existingByUsername != null)
            throw new InvalidOperationException("Username already taken");

        var user = User.Create(username, email, description);
        userRepository.Add(user);
        return user;
    }

    public UserProfileResponse GetUserProfile(Guid userId)
    {
        var user = userRepository.GetById(userId);
        if (user == null)
            throw new KeyNotFoundException($"User with ID {userId} not found");

        return new UserProfileResponse(
            Id: user.Id,
            Username: user.Username,
            Email: user.Email,
            Description: user.Description,
            PublishedGraphCount: user.PublishedGraphIds.Count
        );
    }

    public UserGraphsListResponse GetUserGraphs(Guid userId)
    {
        var user = userRepository.GetById(userId);
        if (user == null)
            throw new KeyNotFoundException($"User with ID {userId} not found");

        var publishedGraphs = publishedGraphRepository.GetByUserId(userId);
        var graphDtos = new List<UserGraphDto>();

        foreach (var publishedGraph in publishedGraphs)
        {
            var graph = graphRepository.GetById(publishedGraph.GraphId);
            if (graph != null)
            {
                graphDtos.Add(new UserGraphDto(
                    Id: graph.Id,
                    Expression: graph.Expression.Text,
                    Title: publishedGraph.Metadata.Title,
                    Description: publishedGraph.Metadata.Description
                ));
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
                    setGraphDtos.Add(new UserGraphDto(
                        Id: graph.Id,
                        Expression: graph.Expression.Text,
                        Title: published.Metadata.Title,
                        Description: published.Metadata.Description
                    ));
                }
            }

            if (setGraphDtos.Count > 0)
            {
                graphSetDtos.Add(new UserGraphSetDto(
                    Id: graphSet.Id,
                    Title: $"GraphSet {graphSetDtos.Count + 1}",
                    Description: null,
                    Graphs: setGraphDtos
                ));
            }
        }

        return new UserGraphsListResponse(
            UserId: userId,
            Graphs: graphDtos,
            GraphSets: graphSetDtos
        );
    }

    public void ValidateUserRegistration(string username, string email)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException("Username cannot be empty", nameof(username));

        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be empty", nameof(email));
    }
}