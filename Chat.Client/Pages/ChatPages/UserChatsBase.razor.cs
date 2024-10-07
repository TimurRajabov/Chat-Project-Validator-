using System.Net;
using System.Security.Claims;
using Chat.Client.DTOs;
using Chat.Client.Repositories.Contracts;
using Chat.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace Chat.Client.Pages.ChatPages;

public abstract class UserChatsBase:ComponentBase
{

    [Inject] private IChatIntegration ChatIntegration { get; set; }
    [Inject] NavigationManager NavigationManager { get; set; }

    [Inject] AuthenticationStateProvider AuthenticationStateProvider { get; set; }

    protected List<ChatDto> Chats { get; set; } = new();

    protected override async Task OnInitializedAsync()
    {
        var (statusCode, response) = await ChatIntegration.GetUserChats();


        if (statusCode == HttpStatusCode.OK)
        {
            Chats = (List<ChatDto>)response;

        }

    }


    protected async Task SeeChat(Guid chatId)
    {

        var chat = Chats.First(c => c.Id == chatId);

        var userId = await GetUserId();

        var userChat = chat.UserChats?.First(c => c.UserId == userId);


        NavigationManager.NavigateTo($"/see-chat/{userChat?.ToUserId}");
    }


    private async Task<Guid> GetUserId()
    {

        var stateProvider = (CustomAuthHandler)AuthenticationStateProvider;

        var state = await stateProvider.GetAuthenticationStateAsync();

        ClaimsPrincipal user = state.User;

        var userId =
            user.Claims.FirstOrDefault
                (c => c.Type == ClaimTypes.NameIdentifier)!.Value;

        return Guid.Parse(userId);
    }


}