using System.Net.WebSockets;
using System.Text;
using ChatApp.Models;
using ChatApp.Services;
using Microsoft.EntityFrameworkCore;

public class WebSocketMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IServiceScopeFactory _scopeFactory;

    public WebSocketMiddleware(RequestDelegate next, IServiceScopeFactory scopeFactory)
    {
        _next = next;
        _scopeFactory = scopeFactory;
    }

    public async Task InvokeAsync(HttpContext ctx)
    {
        if (ctx.Request.Path != "/chat")
        {
            await _next(ctx);
            return;
        }

        if (!ctx.WebSockets.IsWebSocketRequest)
        {
            ctx.Response.StatusCode = StatusCodes.Status400BadRequest;
            return;
        }

        var userIdStr = ctx.Request.Query["userId"].ToString();
        var groupIdStr = ctx.Request.Query["groupId"].ToString();
        var userId = !string.IsNullOrEmpty(userIdStr) ? int.Parse(userIdStr) : 0;
        var groupId = !string.IsNullOrEmpty(groupIdStr) ? int.Parse(groupIdStr) : 0;
        if (userId == 0 || groupId == 0)
        {
            ctx.Response.StatusCode = StatusCodes.Status400BadRequest;
            return;
        }

        WebSocket webSocket = await ctx.WebSockets.AcceptWebSocketAsync();
        await HandleWebSocketConnection(ctx, webSocket, userId, groupId);
    }

    private async Task HandleWebSocketConnection(HttpContext ctx, WebSocket webSocket, int userId, int groupId)
    {
        NewMessageDTO joinMsg;
        using (var scope = _scopeFactory.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var sender = await db.Users
                .AsTracking()
                .SingleOrDefaultAsync(u => u.Id == userId);
            if (sender == null)
            {
                ctx.Response.StatusCode = StatusCodes.Status400BadRequest;
                return;
            }
            var group = await db.Groups
                .Where(g => g.Members.Contains(sender))
                .SingleOrDefaultAsync(g => g.Id == groupId);
            if (group == null)
            {
                ctx.Response.StatusCode = StatusCodes.Status400BadRequest;
                return;
            }

            joinMsg = new NewMessageDTO
            {
                Text = $"{sender.Name} joined the group",
                SenderId = userId,
                GroupId = groupId
            };

            var msg = new Message(joinMsg);
            msg.Sender = sender;
            msg.Group = group;

            db.Messages.Add(msg);
            await db.SaveChangesAsync();

        }
        ChatGroupService.AddConnection(userId, webSocket);
        ChatGroupService.AddToGroup(userId, groupId);
        await ChatGroupService.Broadcast(joinMsg);

        try
        {
            await HandleWebSocketRecievingMessage(webSocket, userId, groupId);
        }
        finally
        {
            var exitMsg = new NewMessageDTO
            {
                Text = $"{userId} left the group",
                SenderId = userId,
                GroupId = groupId
            };
            await ChatGroupService.Broadcast(exitMsg);

            ChatGroupService.RemoveFromGroup(userId, groupId);
            ChatGroupService.RemoveConnection(userId);
            if (webSocket.State == WebSocketState.Open || webSocket.State == WebSocketState.CloseReceived)
            {
                await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Connection closed.", CancellationToken.None);
            }
        }
    }

    private async Task HandleWebSocketRecievingMessage(WebSocket webSocket, int userId, int groupId)
    {
        var buffer = new byte[1024 * 4];

        while (webSocket.State == WebSocketState.Open)
        {
            var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            if (result.MessageType == WebSocketMessageType.Text)
            {
                string text = Encoding.UTF8.GetString(buffer, 0, result.Count);
                using (var scope = _scopeFactory.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    var newMsg = new NewMessageDTO
                    {
                        Text = text,
                        SenderId = userId,
                        GroupId = groupId
                    };

                    var msg = new Message(newMsg);
                    db.Messages.Add(msg);
                    await db.SaveChangesAsync();

                    await ChatGroupService.Broadcast(newMsg);
                }
            }
        }
    }
}
