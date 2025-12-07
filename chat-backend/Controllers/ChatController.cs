using System.Net.WebSockets;
using ChatApp.Models;
using ChatApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class ChatController : ControllerBase
{
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

        using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
        ChatGroupService.AddConnection(userId, webSocket);
        ChatGroupService.AddToGroup(userId, groupId);

        var joinMsg = new NewMessageDTO{
            Text = $"{userId} joined the room",
            SenderId = userId,
            GroupId = groupId
        };
        
        await ChatGroupService.Broadcast(joinMsg);
    }
}
