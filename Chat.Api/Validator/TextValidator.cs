using Chat.Api.Models;
using FluentValidation;

namespace Chat.Api.Validator;

public class TextValidator : AbstractValidator<TextModel>
{

    public TextValidator()
    {

        RuleFor(t => t.Text)
            .NotEmpty()
            .WithMessage("Веди текст");


    }

}
