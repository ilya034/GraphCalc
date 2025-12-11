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
    private readonly IUserService userService;

    public UserController(IUserService userService)
    {
        this.userService = userService;
    }

    [HttpPost("register")]
    [ProducesResponseType(typeof(UserGraphsListResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult RegisterUser([FromBody] CreateUserRequest request)
    {
        var user = userService.RegisterUser(request.Username, request.Email, request.Description);

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
        var response = userService.GetUserGraphs(userId);
        return Ok(response);
    }

    [HttpGet("{userId}")]
    [ProducesResponseType(typeof(UserProfileResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetUser(Guid userId)
    {
        var response = userService.GetUserProfile(userId);
        return Ok(response);
    }

    [HttpPut("{userId}/description")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult UpdateUserDescription(Guid userId, [FromBody] UpdateDescriptionRequest request)
    {
        userService.UpdateUserDescription(userId, request.Description ?? string.Empty);
        return Ok(new { message = "Description updated successfully" });
    }
}
