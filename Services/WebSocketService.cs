using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using ChatPlatform.Services;

public class WebSocketService
{
    private readonly ConnectionManager _connectionManager;

    public WebSocketService(ConnectionManager connectionManager)
    {
        _connectionManager = connectionManager;
    }

    public async Task HandleConnection(WebSocket webSocket)
    {
        string? userName = null;

        try
        {
            userName = await InitializeConnection(webSocket);
            if (userName == null) return;

            await BroadcastMessage(MessageBuilder.CreateSystemMessage($"{userName} entrou no chat."));

            while (webSocket.State == WebSocketState.Open)
            {
                var message = await ReceiveMessage(webSocket);
                if (message == null) break;

                await BroadcastMessage(MessageBuilder.CreateChatMessage("message", userName, message));
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro no WebSocket: {ex.Message}");
        }
        finally
        {
            if (userName != null)
            {
                await HandleDisconnection(webSocket, userName);
            }
        }
    }

    private async Task<string?> InitializeConnection(WebSocket webSocket)
    {
        var buffer = new byte[1024 * 4];
        var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
        var userName = Encoding.UTF8.GetString(buffer, 0, result.Count).Trim();

        if (string.IsNullOrEmpty(userName) || _connectionManager.IsUserNameInUse(userName))
        {
            await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Nome inválido ou já em uso", CancellationToken.None);
            return null;
        }

        _connectionManager.AddConnection(webSocket, userName);
        Console.WriteLine($"Usuário conectado: {userName}");
        return userName;
    }

    private async Task<string?> ReceiveMessage(WebSocket webSocket)
    {
        var buffer = new byte[1024 * 4];
        var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

        return result.MessageType == WebSocketMessageType.Close
            ? null
            : Encoding.UTF8.GetString(buffer, 0, result.Count).Trim();
    }

    private async Task HandleDisconnection(WebSocket webSocket, string userName)
    {
        _connectionManager.RemoveConnection(webSocket, out _);
        Console.WriteLine($"Usuário desconectado: {userName}");
        await BroadcastMessage(MessageBuilder.CreateSystemMessage($"{userName} saiu do chat."));
    }

    private async Task BroadcastMessage(object messageObject)
    {
        var serializedMessage = JsonSerializer.Serialize(messageObject);

        foreach (var connection in _connectionManager.GetActiveConnections())
        {
            try
            {
                await connection.SendAsync(
                    new ArraySegment<byte>(Encoding.UTF8.GetBytes(serializedMessage)),
                    WebSocketMessageType.Text,
                    true,
                    CancellationToken.None
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao enviar mensagem: {ex.Message}");
            }
        }
    }
}