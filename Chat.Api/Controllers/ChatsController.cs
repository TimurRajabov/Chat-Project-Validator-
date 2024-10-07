using Chat.Api.Helpers;
using Chat.Api.Managers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Chat.Api.Controllers
{

    [Authorize(Roles = "admin,user")]
    [Route("api/users/user_id/[controller]")]
    [ApiController]
    public class ChatsController(ChatManager chatManager, UserHelper userHelper) : ControllerBase
    {
        private readonly ChatManager _chatManager = chatManager;
        private readonly UserHelper _userHelper = userHelper;

        
        [HttpGet]
        public async Task<IActionResult> GetUserChats()
        {
            var chats = await _chatManager.GetAllUserChats(_userHelper.GetUserId());
            return Ok(chats);
        }

        [HttpPost]
        public async Task<IActionResult> AddOrEnterChat( [FromBody] Guid toUserId)
        {
            var chat = await _chatManager.AddOrEnterChat(_userHelper.GetUserId(), toUserId);
            return Ok(chat);
        }

        [HttpDelete("{chatId:guid}")]
        public async Task<IActionResult> DeleteChat(Guid chatId)
        {
            try
            {
                var result = await _chatManager.DeleteChat(_userHelper.GetUserId(), chatId);
                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

    }
}
