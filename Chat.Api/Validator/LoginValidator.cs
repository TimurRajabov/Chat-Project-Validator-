using Chat.Api.Models.UserModels;
using FluentValidation;

namespace Chat.Api.Validator
{
    public class LoginValidator : AbstractValidator<LoginModel>
    {

        public LoginValidator() 
        {

            RuleFor(l => l.Username)
                .NotEmpty()
                .Length(5, 32)
                .WithMessage("ot 5 do 32");


            RuleFor(l => l.Password)
                .NotEmpty()
                .Length(8, 32)
                .WithMessage("ot 8 do 32");



        
        
        }

        

    }
}
