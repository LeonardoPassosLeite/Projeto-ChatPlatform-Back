using ChatPlatform.Services;

namespace ChatPlatform.Middlewares
{
    public class WebSocketMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly WebSocketService _webSocketService;

        public WebSocketMiddleware(RequestDelegate next, WebSocketService webSocketService)
        {
            _next = next;
            _webSocketService = webSocketService;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.WebSockets.IsWebSocketRequest)
            {
                var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                await _webSocketService.HandleConnection(webSocket);
            }
            else
            {
                await _next(context);
            }
        }
    }
}
