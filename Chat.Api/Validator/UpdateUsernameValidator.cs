using FluentValidation;
using Microsoft.EntityFrameworkCore.Update;
using Chat.Api.Models;
using Chat.Api.Models.UserModels;



namespace Chat.Api.Validator;

public class UpdateUsernameValidator : AbstractValidator<UpdateUsernameModel>
{

    public UpdateUsernameValidator()    
    {

        RuleFor(u => u.Username).NotEmpty()
            .Length(5, 32)
            .WithMessage("Ot 5 do 32");


    }
     

}
