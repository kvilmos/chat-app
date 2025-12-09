using System.Net.WebSockets;
using System.Text;
using ChatApp.Models;

namespace ChatApp.Services;

public static class ChatGroupService
{
    private static Dictionary<int, WebSocket> _connections = new();
    private static Dictionary<int, HashSet<int>> _groups = new();

    public static bool AddConnection(int userId, WebSocket webSocket)
    {
        return _connections.TryAdd(userId, webSocket);
    }

    public static bool RemoveConnection(int userId)
    {
        return _connections.Remove(userId);
    }

    public static void AddToGroup(int userId, int groupId)
    {
        if (!_groups.ContainsKey(groupId))
        {
            _groups.Add(groupId, new HashSet<int>());
        }
        _groups[groupId].Add(userId);
    }

    public static void RemoveFromGroup(int userId, int groupId)
    {
        _groups[groupId].Remove(userId);
        if (_groups[groupId].Count == 0)
        {
            _groups.Remove(groupId);
        }
    }
    
    public static async Task Broadcast(NewMessageDTO message)
    {   
        var bytes = Encoding.UTF8.GetBytes(message.Text);
        foreach (var userId in _groups[message.GroupId])
        {
            await SendMessage(_connections[userId], bytes);
        }
    }

    public static async Task SendMessage(WebSocket webSocket, byte[] bytes)
    {
        if (webSocket.State == WebSocketState.Open)
        {
            var arraySegment = new ArraySegment<byte>(bytes, 0, bytes.Length);
            await webSocket.SendAsync(arraySegment, WebSocketMessageType.Text, true, CancellationToken.None);
        }
    }

    public static async Task ReceiveMessage(WebSocket socket, Action<WebSocketReceiveResult, byte[]> handleMessage)
    {
        var buffer = new byte[1024 * 4];
        while (socket.State == WebSocketState.Open)
        {
            var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            handleMessage(result, buffer);
        }
    }
}
