using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Reflection;
using Blazored.LocalStorage;
using Chat.Client.DTOs;
using Chat.Client.Models;
using Chat.Client.Repositories.Contracts;
using Chat.Client.Services;

namespace Chat.Client.Repositories;

public class UserIntegration(HttpClient httpClient, ILocalStorageService localStorageService, StorageService storageService) : IUserIntegration
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly ILocalStorageService _localStorageService = localStorageService;
    private readonly StorageService _storageService = storageService;

    public async Task<Tuple<HttpStatusCode, string>> Login(LoginModel model)
    {
        string url = "api/Users/login";


        var result = await _httpClient.PostAsJsonAsync(url, model);

        var statusCode = result.StatusCode;

        var response = await result.Content.ReadAsStringAsync();

        return new(statusCode, response);
    }

    public async Task<Tuple<HttpStatusCode, string>> Register(RegisterModel model)
    {
        string url = "api/Users/register";

        var result = await _httpClient.
            PostAsJsonAsync(url, model);

        var statusCode = result.StatusCode;

        var response = await result.Content.ReadAsStringAsync();


        return new(statusCode, response);
    }

    public async Task<Tuple<HttpStatusCode, object>> GetAllUsers()
    {
        string url = "api/users";

        var token = await _localStorageService.
            GetItemAsStringAsync("token");

        if (!string.IsNullOrEmpty(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization = 
                new AuthenticationHeaderValue("Bearer",token);
        }

        var result = await _httpClient.GetAsync(url);

        var statusCode = result.StatusCode;

        if (statusCode == HttpStatusCode.OK)
        {
            var users = await result.Content.ReadFromJsonAsync<List<UserDto>>();
            return new (statusCode, users!);
        }

        else if (statusCode == HttpStatusCode.BadRequest)
        {
            var response = await result.Content.ReadAsStringAsync();
            return new(statusCode, response);
        }

        else if (statusCode == HttpStatusCode.Unauthorized)
            return new(statusCode, "Unauthorized");

        return new(statusCode, "Something wrong");
    }

    public async Task<Tuple<HttpStatusCode, object?>> GetProfile()
    {
        await AddTokenToHeader();

        var url = "api/users/profile";

        var result = await _httpClient.GetAsync(url);

        var statusCode = result.StatusCode;

        object? response;

        if (statusCode == HttpStatusCode.OK)
        {
            response = await result.Content.ReadFromJsonAsync<UserDto>();
        }
        else
        {
            response = await result.Content.ReadFromJsonAsync<string>();
        }


        return new(statusCode,response);

    }

    public async Task<Tuple<HttpStatusCode, object?>> UpdateAge(byte age)
    {
        var url = "api/Users/update-user-general-info";

        await AddTokenToHeader();

        var model = new UpdateUserGeneralInfo()
        {
            Age = age.ToString()
        };

        var result = await _httpClient.PostAsJsonAsync(url, model);


        var statusCode = result.StatusCode;

        object response =  result.Content.ReadFromJsonAsync<object>();

        return new(statusCode, response);

    }

    public async Task<Tuple<HttpStatusCode, object>> UpdateBio(string bio)
    {
        var url = "api/Users/update-bio";

        await AddTokenToHeader();


        var result = await _httpClient.PostAsJsonAsync(url, bio);


        var statusCode = result.StatusCode;

        object response = result.Content.ReadFromJsonAsync<object>();

        return new(statusCode, response);
    }


    private async Task AddTokenToHeader()
    {

        string? token = await _storageService.GetToken();

        if (!string.IsNullOrEmpty(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }
    }
}