using System.Net;
using Chat.Client.DTOs;
using Chat.Client.Models;

namespace Chat.Client.Repositories.Contracts;

public interface IUserIntegration
{
    Task<Tuple<HttpStatusCode, string>> Login(LoginModel model);
    Task<Tuple<HttpStatusCode, string>> Register(RegisterModel model);

    Task<Tuple<HttpStatusCode, object>> GetAllUsers();

    Task<Tuple<HttpStatusCode, object?>> GetProfile();

    Task<Tuple<HttpStatusCode,object?>> UpdateAge(byte  age);

    Task<Tuple<HttpStatusCode,object>> UpdateBio(string  bio);

}