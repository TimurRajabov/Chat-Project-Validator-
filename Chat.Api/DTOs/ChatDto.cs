using Chat.Api.Entities;

namespace Chat.Api.DTOs;

public class ChatDto
{
    public Guid Id { get; set; }

    public List<string>? ChatNames { get; set; }

    public List<UserChatDto>? UserChats { get; set; }

    public List<MessageDto>? Messages { get; set; }
}