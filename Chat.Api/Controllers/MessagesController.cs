using Chat.Api.Helpers;
using Chat.Api.Managers;
using Chat.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Chat.Api.Controllers;


[Route("api/users/user_id/chats/{chatId}/[controller]")]
[ApiController]
public class MessagesController(MessageManager messageManager, UserHelper userHelper) : ControllerBase
{
    private readonly MessageManager _messageManager = messageManager;
    private readonly UserHelper _userHelper = userHelper;

    //Admin actions
    [Authorize(Roles = "admin")]
    [HttpGet("/api/messages")]
    public async Task<IActionResult> GetAllMessages()
    { 
        try
        {

            var messages = await _messageManager.GetMessages();

            return Ok(messages);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }


    [Authorize(Roles = "admin")]
    [HttpGet("/api/messages/{messageId:int}")]
    public async Task<IActionResult> GetMessageById(int messageId)
    {try
        {

            var message = await _messageManager.GetMessageById(messageId);

            return Ok(message);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }


    [Authorize(Roles = "admin,user")]
    [HttpGet]
    public async Task<IActionResult> GetChatMessages(Guid chatId)
    {
        try
        {
            var messages = await _messageManager.GetChatMessages(chatId);
            return Ok(messages);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [Authorize(Roles = "admin,user")]
    [HttpGet("{messageId:int}")]
    public async Task<IActionResult> GetChatMessageById(Guid chatId, int messageId)
    {
        try
        {
            var message = await _messageManager.GetChatMessageById(chatId, messageId);

            return Ok(message);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [Authorize(Roles = "admin,user")]
    [HttpPost("send-text-message")]
    public async Task<IActionResult> SendTextMessage(Guid chatId,[FromBody] TextModel model)
    {
        try
        {
            var userId = userHelper.GetUserId();

            var message = await _messageManager.SendTextMessage(userId,chatId,model);

            return Ok(message);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }


    [Authorize(Roles = "admin,user")]
    [HttpPost("send-file-message")]
    public async Task<IActionResult> SendFileMessage(Guid chatId, FileModel model)
    {
        try
        {
            var userId = userHelper.GetUserId();
            var result = await _messageManager.SendFileMessage(userId, chatId, model);
            return Ok(result);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }


}