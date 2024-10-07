using Chat.Api.Exceptions;
using Chat.Api.Helpers;
using Chat.Api.Managers;
using Chat.Api.Models.UserModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Chat.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController(UserManager userManager, UserHelper userHelper) : ControllerBase
{
    private readonly UserManager _userManager = userManager;
    private readonly UserHelper _userHelper = userHelper;
    private Guid UserId => _userHelper.GetUserId();


    [Authorize(Roles = "admin,user")]
    [HttpGet]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _userManager.GetAllUsers();
        return Ok(users);
    }

    [Authorize(Roles = "admin,user")]
    [HttpGet("profile")]
    public async Task<IActionResult> GetAllUserById()
    {
        try
        {
            var user = await _userManager.GetUserById(UserId);
            return Ok(user);
        }
        catch (UserNotFoundException e)
        {
            return NotFound();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] CreateUserModel model)
    {
        var result = await _userManager.Register(model);
        return Ok(result);
    }


    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {try
        {

            var result = await _userManager.Login(model);
            return Ok(result);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [Authorize(Roles ="admin,user")]
    [HttpPost("add-or-update-photo")]
    public async Task<IActionResult> AddOrUpdateUserPhoto([FromForm] FileClass model)
    {
        var result = await _userManager.AddOrUpdatePhoto(UserId, model.File);
        return Ok(result);
    }


    [Authorize(Roles = "admin,user")]
    [HttpPost("update-bio")]
    public async Task<IActionResult> UpdateBio([FromBody] string bio) 
    {
        var result = await _userManager.UpdateBio(_userHelper.GetUserId(), bio);
        return Ok(result);

    }

    [Authorize(Roles = "admin,user")]
    [HttpPost("update-user-general-info")]
    public async Task<IActionResult> UpdateUserGeneralInfo([FromBody] UpdateUserGeneralInfo info)
    {
        try
        {
            var result = await _userManager.UpdateUserGeneralInfo(UserId, info);
            return Ok(result);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [Authorize(Roles = "admin,user")]
    [HttpPost("update-username")]
    public async Task<IActionResult> UpdateUsername([FromBody] UpdateUsernameModel model)
    {
        try
        {
            var result = await _userManager.UpdateUsername(UserId, model);
            return Ok(result);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    
    

}

public class FileClass
{
    public IFormFile File { get; set; }
}

