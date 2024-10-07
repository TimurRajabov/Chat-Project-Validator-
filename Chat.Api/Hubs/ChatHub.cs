using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;

namespace Chat.Api.Hubs;

public  class ChatHub : Hub
{
    public override async Task OnConnectedAsync()
    {
      var userId =  Context.User?.
          Claims.FirstOrDefault(c => c.Type ==
                                     ClaimTypes.NameIdentifier)!.Value;
      var username =  Context.User?.
          Claims.FirstOrDefault(c => c.Type ==
                                     ClaimTypes.Name)!.Value;
        var connectionId = Context.ConnectionId;

        ConnectionIdService.ConnectionIds.Add(new(Guid.Parse(userId!),connectionId ));

        Console.WriteLine($"User :{username}, {userId}: {connectionId}");

    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.User?.
            Claims.FirstOrDefault(c => c.Type ==
                                       ClaimTypes.NameIdentifier)!.Value;
        var username = Context.User?.
            Claims.FirstOrDefault(c => c.Type ==
                                       ClaimTypes.Name)!.Value;
        var connectionId = Context.ConnectionId;

        var item = ConnectionIdService
            .ConnectionIds.
            First(c => c.Item2 == connectionId);

        ConnectionIdService.ConnectionIds.Remove(item);

        Console.WriteLine($"User :{username}, {userId}: {connectionId}");
    }
}

public static class ConnectionIdService
{
    public static List<Tuple<Guid, string>> ConnectionIds { get; set; } = new();
}