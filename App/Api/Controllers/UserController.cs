using GraphCalc.Api.Dtos;
using GraphCalc.App.Services;
using Microsoft.AspNetCore.Mvc;

namespace GraphCalc.Api.Controllers;

[ApiController]
[Route("api/users")]
public class UserController : ControllerBase
{
    private readonly UserAppService userAppService;

    public UserController(UserAppService userAppService)
    {
        this.userAppService = userAppService;
    }

    [HttpGet]
    public IActionResult GetAllUsers()
    {
        var userDtos = userAppService.GetAllUsers();
        return Ok(userDtos);
    }

    [HttpGet("{id}")]
    public IActionResult GetUserById(Guid id)
    {
        var userDto = userAppService.GetUserById(id);
        return Ok(userDto);
    }

    [HttpPost]
    public IActionResult CreateUser([FromBody] UserCreateRequest request)
    {
        ValidateUserCreateRequest(request);
        var createdUserDto = userAppService.CreateUser(request);
        return CreatedAtAction(nameof(GetUserById), new { id = createdUserDto.Id }, createdUserDto);
    }

    [HttpPut("{id}")]
    public IActionResult UpdateUser(Guid id, [FromBody] UserDto userDto)
    {
        ValidateGuid(id, nameof(id));
        ValidateUserDto(userDto);
        var updatedUserDto = userAppService.UpdateUser(id, userDto);
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
        var graphDtos = userAppService.GetGraphsByUserId(userId);
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