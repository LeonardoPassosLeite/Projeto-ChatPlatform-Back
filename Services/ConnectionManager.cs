using System.Collections.Concurrent;
using System.Net.WebSockets;

namespace ChatPlatform.Services
{
    public class ConnectionManager
    {
        private readonly ConcurrentDictionary<WebSocket, string> _connections = new();

        public bool AddConnection(WebSocket socket, string userName)
        {
            return _connections.TryAdd(socket, userName);
        }

        public bool RemoveConnection(WebSocket socket, out string? userName)
        {
            return _connections.TryRemove(socket, out userName);
        }

        public string? GetUserName(WebSocket socket)
        {
            return _connections.TryGetValue(socket, out var userName) ? userName : null;
        }

        public IEnumerable<WebSocket> GetActiveConnections()
        {
            return _connections.Keys.Where(socket => socket.State == WebSocketState.Open);
        }

        public bool IsUserNameInUse(string userName)
        {
            return _connections.Values.Contains(userName);
        }
    }
}