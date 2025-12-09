using System.Net.WebSockets;
using System.Text;
using ChatApp.Models;
using ChatApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class ChatController : ControllerBase
{
    private readonly AppDbContext _appDbContext;

    public ChatController(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }


    [Route("/chat")]
    public async Task JoinChat()
    {
        if (!HttpContext.WebSockets.IsWebSocketRequest)
        {
            HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            return;
        }
        
        var userIdStr = HttpContext.Request.Query["userId"].ToString();
        var groupIdStr = HttpContext.Request.Query["groupId"].ToString();
        var userId = !string.IsNullOrEmpty(userIdStr) ? int.Parse(userIdStr) : 0 ;
        var groupId = !string.IsNullOrEmpty(groupIdStr) ? int.Parse(groupIdStr) : 0;
        if (userId == 0 || groupId == 0)
        {
            HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            return;
        }

        var sender = await _appDbContext.Users
            .AsTracking()
            .SingleOrDefaultAsync(u => u.Id == userId);
        if (sender == null)
        {
            HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            return;
        }
        var group = await _appDbContext.Groups
            .Where(g => g.Members.Contains(sender))
            .SingleOrDefaultAsync(g => g.Id == groupId);
        if (group == null)
        {
            HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            return;
        }

        using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
        ChatGroupService.AddConnection(userId, webSocket);
        ChatGroupService.AddToGroup(userId, groupId);

        var joinMsg = new NewMessageDTO{
            Text = $"{userId} joined the group",
            SenderId = userId,
            GroupId = groupId
        };
        await ChatGroupService.Broadcast(joinMsg);

        var oldMessages = await _appDbContext.Messages
            .Where(m => m.GroupId.Equals(groupId))
            .ToListAsync();
        foreach (var oldMsg in oldMessages)
        {
            if (oldMsg.Text != null){
                var bytes = Encoding.UTF8.GetBytes(oldMsg.Text);
                await ChatGroupService.SendMessage(webSocket, bytes);
            }
        }

        await ChatGroupService.ReceiveMessage(webSocket,
        async (result, buffer) =>
        {
            if (result.MessageType == WebSocketMessageType.Text)
            {
                string text = Encoding.UTF8.GetString(buffer, 0, result.Count);
                var newMessage = new NewMessageDTO{
                    Text = text,
                    SenderId = userId,
                    GroupId = groupId
                };

                var message = new Message(newMessage);
                message.Sender = sender;
                message.Group = group;

                _appDbContext.Messages.Add(message);
                await _appDbContext.SaveChangesAsync();

                await ChatGroupService.Broadcast(newMessage);
            }
            else if (result.MessageType == WebSocketMessageType.Close || webSocket.State == WebSocketState.Aborted)
            {
                ChatGroupService.RemoveConnection(userId);
                ChatGroupService.RemoveFromGroup(userId, groupId);
                var exitMsg = new NewMessageDTO{
                    Text = $"{userId} left the group",
                    SenderId = userId,
                    GroupId = groupId
                };
                await ChatGroupService.Broadcast(exitMsg);
                await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
            }
        });
    }
}
