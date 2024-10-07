using Chat.Api.DTOs;
using Chat.Api.Entities;
using Chat.Api.Extensions;
using Chat.Api.Helpers;
using Chat.Api.Repositories;

namespace Chat.Api.Managers;

public class ChatManager(IUnitOfWork unitOfWork)
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    //For Admin
    public async Task<List<ChatDto>> GetAllChats()
    {
        var chats = await _unitOfWork.ChatRepository.GetAllChats();

        return chats.ParseToDtos();
    }


    public async Task<List<ChatDto>> GetAllUserChats(Guid userId)
    {
        var chats = await 
            _unitOfWork.ChatRepository.
                GetAllUserChats(userId);
        
        return chats.ParseToDtos();
    }

    public async Task<ChatDto> GetUserChatById(Guid userId, Guid chatId)
    {
        var chat = await _unitOfWork.ChatRepository.
            GetUserChatById(userId, chatId);

        return chat.ParseToDto();
    }

    public async Task<ChatDto> AddOrEnterChat(Guid fromUserId, Guid toUserId)
    {
        var (check,chat) =
            await _unitOfWork.ChatRepository.
                CheckChatExist(fromUserId, toUserId);

        if (check)
           return chat?.ParseToDto()!;

        var fromUser = await _unitOfWork.
            UserRepository.
            GetUserById(fromUserId);

        var toUser = await _unitOfWork.UserRepository.GetUserById(toUserId);

        List<string> names =
        [
            StaticHelper.GetFullName(fromUser.FirstName, fromUser.LastName),
            StaticHelper.GetFullName(toUser.FirstName, toUser.LastName)
        ];

        chat = new()
        {
            ChatNames = names,
        };

        await _unitOfWork.ChatRepository.AddChat(chat);

        var fromUserChat = new UserChat()
        {
            UserId = fromUserId,
            ToUserId = toUserId,
            ChatId = chat.Id
        };

        await _unitOfWork.UserChatRepository.AddUserChat(fromUserChat);

        var toUserChat = new UserChat()
        {
            UserId = toUserId,
            ChatId = chat.Id,
            ToUserId = fromUserId
        };

        await _unitOfWork.UserChatRepository.AddUserChat(toUserChat);

        return chat.ParseToDto();
    }

    public async Task<string> DeleteChat(Guid userId, Guid chatId)
    {
        var user = await _unitOfWork.UserRepository.GetUserById(userId);

        var chat = await _unitOfWork.ChatRepository.GetUserChatById(userId, chatId);

        await _unitOfWork.ChatRepository.DeleteChat(chat);

        return "Deleted successfully";
    }
}