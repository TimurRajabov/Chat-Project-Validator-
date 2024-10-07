namespace Chat.Client.DTOs;

public class MessageDto
{
    public int Id { get; set; }

    public string? Text { get; set; }

    public Guid FromUserId { get; set; }

    public string FromUserName { get; set; }

    public Guid ChatId { get; set; }


    public int ContentId { get; set; }
    public ContentDto? Content { get; set; }

    public bool IsEdited { get; set; }

    public DateTime SendAt => DateTime.UtcNow;
    public DateTime? EditedAt { get; set; }
}