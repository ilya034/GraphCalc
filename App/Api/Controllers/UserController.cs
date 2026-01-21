using GraphCalc.Api.Dtos;
using GraphCalc.App;
using GraphCalc.Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace GraphCalc.Api.Controllers;

[ApiController]
[Route("api/users")]
public class UserController : ControllerBase
{
    private readonly IUserAppService userAppService;

    public UserController(IUserAppService userAppService)
    {
        this.userAppService = userAppService;
    }

    [HttpGet]
    public IActionResult GetAllUsers()
    {
        var users = userAppService.GetAllUsers();
        var userDtos = users.ToDto();
        return Ok(userDtos);
    }

    [HttpGet("{id}")]
    public IActionResult GetUserById(Guid id)
    {
        var user = userAppService.GetUserById(id);
        var userDto = user.ToDto();
        return Ok(userDto);
    }

    [HttpPost]
    public IActionResult CreateUser([FromBody] UserCreateRequest request)
    {
        ValidateUserCreateRequest(request);
        var createdUser = userAppService.CreateUser(request);
        var createdUserDto = createdUser.ToDto();
        return CreatedAtAction(nameof(GetUserById), new { id = createdUserDto.Id }, createdUserDto);
    }

    [HttpPut("{id}")]
    public IActionResult UpdateUser(Guid id, [FromBody] UserDto userDto)
    {
        ValidateGuid(id, nameof(id));
        ValidateUserDto(userDto);
        var updatedUser = userAppService.UpdateUser(id, userDto);
        var updatedUserDto = updatedUser.ToDto();
        return Ok(updatedUserDto);
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteUser(Guid id)
    {
        userAppService.DeleteUser(id);
        return NoContent();
    }

    [HttpGet("{userId}/graphs")]
    public IActionResult GetGraphsByUserId(Guid userId)
    {
        ValidateGuid(userId, nameof(userId));
        var graphs = userAppService.GetGraphsByUserId(userId);
        var graphDtos = graphs.ToDto();
        return Ok(graphDtos);
    }

    private void ValidateUserCreateRequest(UserCreateRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (string.IsNullOrWhiteSpace(request.Username))
            throw new ArgumentException("Username cannot be null or empty.", nameof(request.Username));

        if (request.Username.Length > 255)
            throw new ArgumentException("Username cannot exceed 255 characters.", nameof(request.Username));

        if (string.IsNullOrWhiteSpace(request.Email))
            throw new ArgumentException("Email cannot be null or empty.", nameof(request.Email));

        if (request.Email.Length > 255)
            throw new ArgumentException("Email cannot exceed 255 characters.", nameof(request.Email));
    }

    private void ValidateUserDto(UserDto userDto)
    {
        ArgumentNullException.ThrowIfNull(userDto);

        ValidateGuid(userDto.Id, nameof(userDto.Id));

        if (string.IsNullOrWhiteSpace(userDto.Username))
            throw new ArgumentException("Username cannot be null or empty.", nameof(userDto.Username));

        if (userDto.Username.Length > 255)
            throw new ArgumentException("Username cannot exceed 255 characters.", nameof(userDto.Username));

        if (string.IsNullOrWhiteSpace(userDto.Email))
            throw new ArgumentException("Email cannot be null or empty.", nameof(userDto.Email));

        if (userDto.Email.Length > 255)
            throw new ArgumentException("Email cannot exceed 255 characters.", nameof(userDto.Email));
    }

    private void ValidateGuid(Guid id, string paramName)
    {
        if (id == Guid.Empty)
            throw new ArgumentException($"{paramName} cannot be empty.", paramName);
    }
}