using System.ComponentModel.DataAnnotations;

namespace Chat.Client.Models;

public class TextModel
{
    [Required]
    public string Text { get; set; }
}