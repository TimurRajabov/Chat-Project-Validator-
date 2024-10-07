using System.ComponentModel.DataAnnotations;

namespace Chat.Client.Models;

public class RegisterModel
{


    //[Required]
    public string FirstName { get; set; }

    public string? LastName { get; set; }


    //[Required]
    public string Username { get; set; }

    //[Required]
    public string Password { get; set; }

    //[Required]
    //[Compare(nameof(Password))]
    public string ConfirmPassword { get; set; }

    //[Required]
    public string Gender { get; set; }
}