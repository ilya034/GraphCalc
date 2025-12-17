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
        var createdUserDto = userAppService.CreateUser(request);
        return CreatedAtAction(nameof(GetUserById), new { id = createdUserDto.Id }, createdUserDto);
    }

    [HttpPut("{id}")]
    public IActionResult UpdateUser(Guid id, [FromBody] UserDto userDto)
    {
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
        var graphDtos = userAppService.GetGraphsByUserId(userId);
        return Ok(graphDtos);
    }
}