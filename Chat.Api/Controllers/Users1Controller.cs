using Microsoft.AspNetCore.Mvc;
using Chat.Api.Models.UserModels;
using Chat.Api.Validator;
using Chat.Api.Models;
namespace Chat.Api.Controllers;



[Route("/api/[controller]")]
[ApiController]
public class Users1Controller : Controller
{
    [HttpPost("Register")]
    public IActionResult Register(CreateUserModel models)
    {
        var val = new UserValidator();
        var result = val.Validate(models);
        if (!result.IsValid) { return BadRequest(result.Errors); }
        return Ok("ok");

    }


    [HttpPost("Login")]
    public IActionResult Login(LoginModel models)
    {
        var val = new LoginValidator();
        var result = val.Validate(models);
        if (!result.IsValid) { return BadRequest(result.Errors); }
        return Ok("ok");

    }

    [HttpPost("UpdateUsername")]
    public IActionResult UpdateUsername(UpdateUsernameModel  models)
    {
        var val1 = new UpdateUsernameValidator();
        var result = val1.Validate(models);
        if (!result.IsValid) { return BadRequest(result.Errors); }
        return Ok("ok");

    }

    [HttpPost("Text")]
    public IActionResult Text(TextModel models)
    {
        var val1 = new TextValidator();
        var result = val1.Validate(models);
        if (!result.IsValid) { return BadRequest(result.Errors); }
        return Ok("ok");

    }



}
