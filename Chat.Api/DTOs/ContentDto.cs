using System.ComponentModel.DataAnnotations;

namespace Chat.Api.DTOs;

public class ContentDto
{
    public int Id { get; set; }

    public string? Caption { get; set; }
    public string FileUrl { get; set; }

    public string? Type { get; set; }

    public int MessageId { get; set; }
}