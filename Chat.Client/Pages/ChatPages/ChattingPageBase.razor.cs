using System.Net;
using System.Security.Claims;
using Chat.Client.DTOs;
using Chat.Client.Repositories.Contracts;
using Chat.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.SignalR.Client;

namespace Chat.Client.Pages.ChatPages;  

public abstract class ChattingPageBase : ComponentBase
{
    [Parameter] public required List<ChatDto> Chats { get; set; }

    [Inject] private AuthenticationStateProvider StateProvider { get; set; }

    protected string? Username { get; set; }


    protected UserDto? User { get; set; }

    protected List<UserDto>? Users { get; set; }


    [Inject] IUserIntegration UserIntegration {get; set; }

    private HubConnection? HubConnection { get; set; }

    [Inject] StorageService StorageService { get; set; }


    [Inject] IChatIntegration ChatIntegration { get; set; }
    protected ChatDto? Chat { get; set; } 

    protected List<MessageDto>? Messages { get; set; }


    protected Guid? ChatId { get; set; }


    protected string Text { get; set; }

    protected override async Task OnInitializedAsync()
    {

        GetChatNames();
        var (statusCode1, response1) = await UserIntegration.GetAllUsers();

        if (statusCode1 == HttpStatusCode.OK)
        {
            Users = (List<UserDto>)response1!;
        }


        var (statusCode, response) = await UserIntegration.GetProfile();

        if (statusCode == HttpStatusCode.OK)
        {
            User = (UserDto)response!;
        }

        Username = await GetUserInfo();

        await SortContacts();


        await DisConnectHub();
        await ConnectToHub();

        if (ChatId != null)
        {
            Chat = Chats.Single(c => c.Id == ChatId);
            Messages = Chat.Messages!;
        }



        StateHasChanged();
    }

    private async Task<string> GetUserInfo()
    {
        var stateProvider = (CustomAuthHandler)StateProvider;

        var state = await stateProvider.GetAuthenticationStateAsync();


        var user = state.User;

        var username = user.Claims
            .First(c => c.Type == ClaimTypes.Name).Value;


        return username;

    }


    protected async Task SendMessage()
    {

        var (statusCode, response) = await ChatIntegration.SendTextMessage(Chat.Id, Text);

        if (statusCode == HttpStatusCode.OK)
        {
            var message = (MessageDto)response;
            Text = string.Empty;
        }

    }


    private async Task ConnectToHub()
    {
        var token = await StorageService.GetToken();
        if (!string.IsNullOrEmpty(token))
        {
            if (HubConnection == null)
            {
                HubConnection = new HubConnectionBuilder()
                    .WithUrl($"http://localhost:5068/chat-hub?token={token}")
                    .Build();
            }
        }


        HubConnection?.On<MessageDto>("NewMessage", model =>
        {
            Messages.Add(model);
            StateHasChanged();
        });

        await HubConnection!.StartAsync();


    }

    private async Task DisConnectHub()
    {
        if (HubConnection is not null)
            await HubConnection.StopAsync();
    }


    protected async Task SelectChat(Guid chatId)
    {
        ChatId = chatId;

        Chat = Chats.Single(c => c.Id == ChatId);
        Messages = Chat.Messages;

        StateHasChanged();
    }


    private void GetChatNames()
    {
        var currentFullname = GetFullName(User?.FirstName, User?.LastName);
        foreach (var chat in Chats)
        {
            chat.ChatName = chat.ChatNames?.First(c => c != currentFullname);
        }
        StateHasChanged();
    }

    private string GetChatName(List<string> chatNames)
    {
        var currentFullname = GetFullName(User?.FirstName, User?.LastName);

       var chatName = chatNames?.First(c => c != currentFullname);

       return chatName!;

    }


    private static string GetFullName(string? firstName, string? lastName)
    {
        return $"{firstName}  {lastName}";
    }



    private async Task SortContacts()
    {
        var currentUser = Users?.SingleOrDefault(u => u.Username == Username);
        if (currentUser is not null)
            Users?.Remove(currentUser);
        foreach (var chat in Chats)
        {
            if (chat.UserChats is not null)
            {
                var userChat = chat.UserChats[0];

                //for every kind of users
                /*
                var userId_1 = userChat.UserId;
                var userId_2 = userChat.ToUserId;

                var user_1 = Users?.SingleOrDefault(u => u.Id == userId_1);

                if (user_1 is not null)
                    Users?.Remove(user_1);

                var user_2 = Users?.SingleOrDefault(u => u.Id == userId_2);

                if(user_2 is not null)
                    Users?.Remove(user_2);
                    */



                var toUserId = userChat.ToUserId;

                var toUser = Users?.SingleOrDefault(u => u.Id == toUserId);

                if (toUser is not null)
                    Users?.Remove(toUser);


            }
        }
    }

    protected async Task Pressed(KeyboardEventArgs e)
    {
        if (e.Key == "Enter")
        {
            await SendMessage();
        }
    }

    protected async Task CreateChat(Guid toUserId)
    {

        var (statusCode, response) = await ChatIntegration.GetChat(toUserId);

        if (statusCode == HttpStatusCode.OK)
        {
            Chat = (ChatDto)response;
            Chat.ChatName = GetChatName(Chat.ChatNames!);
            Messages = Chat.Messages!;

            var toUser = Users?.SingleOrDefault(u => u.Id == toUserId);

            if (toUser is not null)
                Users?.Remove(toUser);

            Chats.Add(Chat);


        }

    }



}

