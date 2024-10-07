using Chat.Api.Constants;
using Chat.Api.DTOs;
using Chat.Api.Entities;
using Chat.Api.Exceptions;
using Chat.Api.Extensions;
using Chat.Api.Helpers;
using Chat.Api.Models.UserModels;
using Chat.Api.Repositories;
using Microsoft.AspNetCore.Identity;

namespace Chat.Api.Managers;

public class UserManager(IUnitOfWork unitOfWork, JWTManager jwtManager, MemoryCacheManager cacheManager)
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly JWTManager _jwtManager = jwtManager;
    private readonly MemoryCacheManager _cacheManager = cacheManager; 

    private const string Key = "users";
    
    public async Task<List<UserDto>> GetAllUsers()
    {

        var dtos = _cacheManager.GetDtos(Key);
        if (dtos is not null)
        {
            return (List<UserDto>)dtos;
        }
        
        var users = await _unitOfWork.UserRepository.GetAllUsers();
        
        await Set();

        return users.ParseToDtos();
    }

    public async Task<UserDto> GetUserById(Guid userId)
    {
        var dtos = _cacheManager.GetDtos(Key);
        if (dtos is not null)
        {
            List<UserDto> users = (List<UserDto>)dtos;

            var userDto = users.SingleOrDefault(u => u.Id == userId);
            
            if (userDto is null)
                throw new UserNotFoundException();

            return userDto;
        }
        
        var user = await _unitOfWork.UserRepository.GetUserById(userId);

        await Set();
        
        return user.ParseToDto();
    }

    public async Task<string> Register(CreateUserModel model)
    {
        await CheckForExist(model.Username);

        var user = new User()
        {
            FirstName = model.FirstName,
            LastName = model.LastName,
            Username = model.Username,
            Gender = GetGender(model.Gender),
            Role = UserConstants.UserRole
        };


        if (user.Username == "super-admin")
        {
            user.Role = UserConstants.AdminRole;
        }

        
        // for any logics
        
        /*
        if (user.FirstName == "")
        {
            user.Role = UserConstants.AdminRole;
        }
        */
        
        var passwordHash = 
            new PasswordHasher<User>()
                .HashPassword(user, model.Password);

        user.PasswordHash = passwordHash;

        await _unitOfWork.UserRepository.AddUser(user);
        await Set();

        return "Registered successfully";
    }

    public async Task<string> Login(LoginModel model)
    {
        //must be lower case

        var user = await _unitOfWork.UserRepository.GetUserByUsername(model.Username);

        if (user is null)
            throw new Exception("Username is invalid");

        var result =
            new PasswordHasher<User>()
                .VerifyHashedPassword(user, user.PasswordHash, model.Password);

        if (result == PasswordVerificationResult.Failed)
            throw new Exception("Invalid password");

        if (string.IsNullOrEmpty(user.Role))
        {
            user.Role = UserConstants.UserRole;
            await _unitOfWork.UserRepository.UpdateUser(user);
        }
            
        var token = _jwtManager.GenerateToken(user);


        return token;
    }


    public async Task<byte[]> AddOrUpdatePhoto(Guid userId,IFormFile file)
    {
        var user = await _unitOfWork.UserRepository.GetUserById(userId);

        StaticHelper.IsPhoto(file);

        var data = StaticHelper.GetData(file);

        user.PhotoData = data;

        await _unitOfWork.UserRepository.UpdateUser(user);

        await Set();
        return data;
    }




    private async Task CheckForExist(string username)
    {
        var user = await _unitOfWork.UserRepository.GetUserByUsername(username);

        if (user is not null)
            throw new UserExistException();
    }

    private string GetGender(string? gender)
    {
        if (gender is null)
            return UserConstants.Male;

        var checkingForGenderExist = gender.ToLower() == UserConstants.Male
                                     || gender.ToLower() == UserConstants.Female;


        return checkingForGenderExist ? gender : UserConstants.Male;
    }

    public async Task<UserDto> UpdateBio(Guid userId, string bio)
    {
        var user = await _unitOfWork.UserRepository.GetUserById(userId);
        if (!string.IsNullOrEmpty(bio)) {

            user.Bio = bio;
            await _unitOfWork.UserRepository.UpdateUser(user);
        }
        await Set();
        return user.ParseToDto();
    }


    public async Task<UserDto> UpdateUserGeneralInfo(Guid userId, UpdateUserGeneralInfo info)
    {
        var user = await _unitOfWork.UserRepository.GetUserById(userId);

        bool check = false;
        
        if (!string.IsNullOrEmpty(info.Firstname))
        {
            user.FirstName = info.Firstname;
            check = true;
        }
        
        if (!string.IsNullOrEmpty(info.Lastname))
        {
            user.LastName = info.Lastname;
            check = true;
        }
        
        if (!string.IsNullOrEmpty(info.Age))
        {
           
            try
            {
                byte age = byte.Parse(info.Age);

                user.Age = age;
                check = true;
            }
            catch (Exception e)
            {
                throw new Exception("Age must be number");
            }
        }

        if (check)
        {
            await _unitOfWork.UserRepository.UpdateUser(user);
            await Set();
        }

        return user.ParseToDto();
    }

    public async Task<UserDto> UpdateUsername(Guid userId,UpdateUsernameModel model )
    {
        var user = await _unitOfWork.UserRepository.GetUserById(userId);

        await CheckForExist(model.Username);

        user.Username = model.Username;

        await _unitOfWork.UserRepository.UpdateUser(user);

        await Set();
        return user.ParseToDto();

    }

    private async Task Set()
    {
        var users = await _unitOfWork.UserRepository.GetAllUsers();
        _cacheManager.GetOrUpdateDtos(Key,users.ParseToDtos());
    }
    
    
}
