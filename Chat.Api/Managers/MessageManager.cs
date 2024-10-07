using Chat.Api.DTOs;
using Chat.Api.Entities;
using Chat.Api.Extensions;
using Chat.Api.Hubs;
using Chat.Api.Models;
using Chat.Api.Repositories;
using Microsoft.AspNetCore.SignalR;

namespace Chat.Api.Managers;

public class MessageManager(IUnitOfWork unitOfWork, IHostEnvironment hostEnvironment, IHubContext<ChatHub> hubContext)
{
    //1. GetMessages
    //2. GetChatMessages
    //3. GetMessageById
    //4. GetChatMessageById
    //5. SendMessage

    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IHostEnvironment _hostEnvironment = hostEnvironment;
    private readonly IHubContext<ChatHub> _hubContext = hubContext;


    //Admin action
    public async Task<List<MessageDto>> GetMessages()
    {
        var messages = await _unitOfWork.MessageRepository.GetMessages();

        return messages.ParseToDtos();
    }

    public async Task<List<MessageDto>> GetChatMessages(Guid chatId)
    {
        var messages = await _unitOfWork
            .MessageRepository
            .GetChatMessages(chatId);

        return messages.ParseToDtos();
    }

    //Admin action
    public async Task<MessageDto> GetMessageById(int messageId)
    {
        var message = await _unitOfWork.MessageRepository.GetMessageById(messageId);

        return message.ParseToDto();
    }


    public async Task<MessageDto> GetChatMessageById(Guid chatId, int messageId)
    {
        var message = await _unitOfWork.MessageRepository.
            GetChatMessageById(chatId, messageId);

        return message.ParseToDto();
    }

    public async Task<MessageDto> SendTextMessage(Guid userId,Guid chatId, TextModel model )
    {

       await CheckingUserInChat(userId:userId,chatId:chatId);

        var user = await _unitOfWork.UserRepository.GetUserById(userId);

        var message = new Message()
        {
            Text = model.Text,
            FromUserId = userId,
            FromUserName = user.Username,
            ChatId = chatId
        };

        await _unitOfWork.MessageRepository.AddMessage(message);

        await _hubContext.Clients.All.SendAsync("NewMessage", message.ParseToDto());

        /*var connection1 = ConnectionIdService.ConnectionIds.First(c => c.Item1 == userId);

        var userChat = await _unitOfWork.UserChatRepository.GetUserChat(userId, chatId);


        var connection2 = ConnectionIdService.ConnectionIds.First(c => c.Item1 == userChat.ToUserId);

        if (!string.IsNullOrEmpty(connection1.Item2))
        {
            await _hubContext.Clients.Client(connection1.Item2).SendAsync("NewMessage", message.ParseToDto());
        }

        if (!string.IsNullOrEmpty(connection2.Item2))
        {

            await _hubContext.Clients.Client(connection2.Item2).SendAsync("NewMessage", message.ParseToDto());

        }*/

        return message.ParseToDto();
    }

    public async Task<MessageDto> SendFileMessage(Guid userId, Guid chatId,FileModel model)
    {

        var user = await _unitOfWork.UserRepository.GetUserById(userId);

        await CheckingUserInChat(userId: userId, chatId: chatId);


        var ms = new MemoryStream();

        await model.File.CopyToAsync(ms);

        var data = ms.ToArray();

        var fileUrl = GetFilePath();

        await File.WriteAllBytesAsync(fileUrl, data);


        var content = new Content()
        {
            FileUrl = fileUrl,
            Type = model.File.ContentType
        };

        var message = new Message()
        {
            FromUserId = userId,
            FromUserName = user.Username,
            ChatId = chatId,
            ContentId = content.Id,
            Content = content

        };


        await _unitOfWork.MessageRepository.AddMessage(message);

        return message.ParseToDto();
    }


    public string GetFilePath()
    {
        var generalPath = _hostEnvironment.ContentRootPath;

        var name = Guid.NewGuid();

        var fileName = generalPath + "\\wwwroot\\MessageFiles\\" + name;

        return fileName;
    }

    private async Task CheckingUserInChat(Guid userId, Guid chatId)
    {
        await _unitOfWork.UserChatRepository.GetUserChat(userId, chatId);
    }
}