using GraphCalc.Domain.Entities;
using GraphCalc.Api.Dtos;

namespace GraphCalc.Domain.Services;

public interface IUserService
{
    User RegisterUser(string username, string email, string? description);
    UserProfileResponse GetUserProfile(Guid userId);
    void UpdateUserDescription(Guid userId, string description);
    UserGraphsListResponse GetUserGraphs(Guid userId);
    void ValidateUserRegistration(string username, string email);
}