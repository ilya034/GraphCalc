using GraphCalc.Domain.Entities;
using GraphCalc.Domain.Interfaces;
using GraphCalc.Api.Dtos;

namespace GraphCalc.Domain.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IGraphSetRepository _graphSetRepository;

    public UserService(
        IUserRepository userRepository,
        IGraphSetRepository graphSetRepository)
    {
        _userRepository = userRepository;
        _graphSetRepository = graphSetRepository;
    }

    public User RegisterUser(string username, string email, string? description)
    {
        ValidateUserRegistration(username, email);

        var existingByEmail = _userRepository.GetByEmail(email);
        if (existingByEmail != null)
            throw new InvalidOperationException("Email already registered");

        var existingByUsername = _userRepository.GetByUsername(username);
        if (existingByUsername != null)
            throw new InvalidOperationException("Username already taken");

        var user = User.Create(username, email, description);
        _userRepository.Add(user);
        return user;
    }

    public UserProfileResponse GetUserProfile(Guid userId)
    {
        var user = _userRepository.GetById(userId);
        if (user == null)
            throw new KeyNotFoundException($"User with ID {userId} not found");

        var graphSetsCount = _graphSetRepository.GetByAuthorId(userId).Count();

        return new UserProfileResponse(
            Id: user.Id,
            Username: user.Username,
            Email: user.Email,
            Description: user.Description,
            GraphSetsCount: graphSetsCount
        );
    }

    public void UpdateUserDescription(Guid userId, string description)
    {
        var user = _userRepository.GetById(userId);
        if (user == null)
            throw new KeyNotFoundException($"User with ID {userId} not found");

        user.UpdateDescription(description);
    }

    public UserGraphsListResponse GetUserGraphs(Guid userId)
    {
        var user = _userRepository.GetById(userId);
        if (user == null)
            throw new KeyNotFoundException($"User with ID {userId} not found");

        var graphSets = _graphSetRepository.GetByAuthorId(userId).ToList();
        var graphSetDtos = new List<UserGraphSetDto>();

        foreach (var graphSet in graphSets)
        {
            var setGraphDtos = new List<UserGraphDto>();
            foreach (var graph in graphSet.Graphs)
            {
                setGraphDtos.Add(new UserGraphDto(
                    Id: graph.Id,
                    Expression: graph.Expression.Text,
                    Title: $"Graph {setGraphDtos.Count + 1}",
                    Description: null
                ));
            }

            graphSetDtos.Add(new UserGraphSetDto(
                Id: graphSet.Id,
                Title: $"GraphSet {graphSetDtos.Count + 1}",
                Description: null,
                Graphs: setGraphDtos
            ));
        }

        return new UserGraphsListResponse(
            UserId: userId,
            Graphs: new List<UserGraphDto>(),
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