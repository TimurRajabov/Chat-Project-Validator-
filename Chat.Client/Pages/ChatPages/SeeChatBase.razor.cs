using System.Net;
using Chat.Client.DTOs;
using Chat.Client.Repositories;
using Chat.Client.Repositories.Contracts;
using Chat.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;

namespace Chat.Client.Pages.ChatPages;

public abstract class SeeChatBase:ComponentBase
{
    [Inject] IChatIntegration ChatIntegration { get; set; }
    protected ChatDto Chat { get; set; } = new();

    protected string Text {get; set; }

    protected List<MessageDto> Messages { get; set; } = new();

    [Parameter] public Guid ToUserId { get; set; }

    private HubConnection? HubConnection { get; set; }

    [Inject]StorageService StorageService {get; set; }

    protected override async Task OnInitializedAsync()
    {
        await DisConnectHub();
        await ConnectToHub();

        var (statusCode, response) = await ChatIntegration.GetChat(ToUserId);


        if (statusCode == HttpStatusCode.OK)
        {
            Chat = (ChatDto)response;

            var (statusCode1, response1) = 
                await ChatIntegration.GetChatMessages(Chat.Id);


            if (statusCode1 == HttpStatusCode.OK)
            {
                Messages = (List<MessageDto>)response1;
            }

        }




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

      await  HubConnection!.StartAsync();


    }

    private async Task DisConnectHub()
    {
        if(HubConnection is not null)
            await HubConnection.StopAsync();
    }



}