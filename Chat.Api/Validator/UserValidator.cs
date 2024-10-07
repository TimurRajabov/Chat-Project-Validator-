using FluentValidation;
using Chat.Api.Models;
using Chat.Api.Models.UserModels;



namespace Chat.Api.Validator;

public class UserValidator : AbstractValidator<CreateUserModel>
{


    public UserValidator() 
    {

        RuleFor(u => u.FirstName)
            .NotEmpty()
            .Length(3, 16)
            .Must(Cheak)
            .WithMessage("FirstName must start with Capital letter");


        RuleFor(u => u.LastName)
            .NotEmpty()
            .Length(3, 16)
            .Must(Cheak1)
            .WithMessage("LastName must start with Capital letter");


        RuleFor(u => u.Username)
            .NotNull()
            .Length(5, 32);


        RuleFor(p => p.Password)
            .NotEmpty()
            .Length(8, 32) // Надо будет написать метод для проверки что то и положить в .Must().WithMessage();
            .WithMessage("Длина должна быть от 8 до 32");


        RuleFor(c => c.ConfirmPassword)
            .NotEmpty()
            .Equal(c => c.Password);



        RuleFor(g => g.Gender)
            .NotNull()
            .Must(CheakGender);
            

    }

    


    private bool CheakGender(string gender)
    {
        return gender == "Male" || gender == "Female";
    }


    public bool Cheak1(string lastname )
    {

        if (string.IsNullOrEmpty(lastname))
        {
            return false; 
        }       

        
        return char.IsUpper(lastname[0]);
    }


    public bool Cheak(string firstName)
    {

        if (string.IsNullOrEmpty(firstName))
        {
            return false; 
        }

        
        return char.IsUpper(firstName[0]);


    }



        /*
       for(int i = 0; i < firstName.Length; i++)
        {
            if(i == 0)
            {
                return char.IsUpper(firstName[i]);
            }
        }
       return false;
        */
    


}

