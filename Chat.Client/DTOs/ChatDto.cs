namespace Chat.Client.DTOs;

public class ChatDto
{
    public Guid Id { get; set; }

    public List<string>? ChatNames { get; set; }

    public List<UserChatDto>? UserChats { get; set; }

    public List<MessageDto>? Messages { get; set; }

    public string? ChatName { get; set; }
}