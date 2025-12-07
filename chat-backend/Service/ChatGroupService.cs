using System.Net.WebSockets;
using System.Text;
using ChatApp.Models;

namespace ChatApp.Services;

public static class ChatGroupService
{
    public static Dictionary<int, WebSocket> Connections = new();
    public static Dictionary<int, HashSet<int>> Groups = new();

    public static bool AddConnection(int userId, WebSocket webSocket)
    {
        return Connections.TryAdd(userId, webSocket);
    }

    public static void AddToGroup(int userId, int groupId)
    {
        if (!Groups.ContainsKey(groupId))
        {
            Groups.Add(groupId, new HashSet<int>());
        }
        Groups[groupId].Add(userId);
    }
    public static async Task Broadcast(NewMessageDTO message)
    {   
        var bytes = Encoding.UTF8.GetBytes(message.Text);
        foreach (var userId in Groups[message.GroupId])
        {
            if (Connections[userId].State == WebSocketState.Open)
            {
                var arraySegment = new ArraySegment<byte>(bytes, 0, bytes.Length);
                await Connections[userId].SendAsync(arraySegment, WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }
    }
}
