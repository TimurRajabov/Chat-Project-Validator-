using System.ComponentModel.DataAnnotations;

namespace Chat.Api.Models;

public class TextModel
{
    [Required]
    public string Text { get; set; }
}