using Microsoft.AspNetCore.Mvc;
using GraphCalc.Api.Dtos;
using GraphCalc.Domain.Services;
using GraphCalc.Domain.Interfaces;
using GraphCalc.Infrastructure.Repositories;
using DomainUser = GraphCalc.Domain.Entities.User;

namespace GraphCalc.Api.Controllers;

[ApiController]
[Route("api/user")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("register")]
    [ProducesResponseType(typeof(UserGraphsListResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult RegisterUser([FromBody] CreateUserRequest request)
    {
        var user = _userService.RegisterUser(request.Username, request.Email, request.Description);

        var response = new UserGraphsListResponse(
            UserId: user.Id,
            Graphs: new(),
            GraphSets: new()
        );

        return CreatedAtAction(nameof(GetUserGraphs), new { userId = user.Id }, response);
    }

    [HttpGet("{userId}/graphs")]
    [ProducesResponseType(typeof(UserGraphsListResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetUserGraphs(Guid userId)
    {
        var response = _userService.GetUserGraphs(userId);
        return Ok(response);
    }

    [HttpGet("{userId}")]
    [ProducesResponseType(typeof(UserProfileResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetUser(Guid userId)
    {
        var response = _userService.GetUserProfile(userId);
        return Ok(response);
    }

    [HttpPut("{userId}/description")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult UpdateUserDescription(Guid userId, [FromBody] UpdateDescriptionRequest request)
    {
        _userService.UpdateUserDescription(userId, request.Description ?? string.Empty);
        return Ok(new { message = "Description updated successfully" });
    }
}
