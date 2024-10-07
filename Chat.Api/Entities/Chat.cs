namespace Chat.Api.Entities;

public class Chat
{
    public Guid Id { get; set; }

    public List<string>? ChatNames { get; set; }

    public List<UserChat>? UserChats { get; set; }

    public List<Message>? Messages { get; set; }
}