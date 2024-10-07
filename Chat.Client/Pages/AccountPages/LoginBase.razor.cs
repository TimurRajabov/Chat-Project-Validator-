using Chat.Client.Repositories;
using Microsoft.AspNetCore.Components;
using System.Net;
using Chat.Client.Models;
using Chat.Client.Repositories.Contracts;
using System.Reflection;
using Blazored.LocalStorage;

namespace Chat.Client.Pages.AccountPages;

public class LoginBase : ComponentBase
{
    [Inject]
    IUserIntegration UserIntegration { get; set; }

    [Inject]
    NavigationManager Manager { get; set; }

    [Inject]
    ILocalStorageService StorageService { get; set; }


    protected LoginModel Model = new();

    protected async Task LoginClicked()
    {

        var (statusCode, response) = await UserIntegration.Login(Model);

        bool isOk = statusCode == HttpStatusCode.OK;

        bool isBadRequest = statusCode == HttpStatusCode.BadRequest;



        if (isOk)
        {
            await StorageService.SetItemAsStringAsync("token", response);
            Manager.Refresh(true);
            Manager.NavigateTo($"/user-chats");
        }
        else if (isBadRequest)
            Manager.NavigateTo($"/error/{response}");


    }
}

