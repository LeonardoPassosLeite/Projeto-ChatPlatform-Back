using ChatPlatform.Middlewares;
using ChatPlatform.Services;

var builder = WebApplication.CreateBuilder(args);

// Configurar serviços
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Registra o ConnectionManager como singleton
builder.Services.AddSingleton<ConnectionManager>();

// Registra o WebSocketService como singleton
builder.Services.AddSingleton<WebSocketService>();

var app = builder.Build();

// Configuração de middleware
app.UseCors("AllowAll");
app.UseWebSockets();

// Adicionar o WebSocketMiddleware para lidar com conexões WebSocket
app.UseMiddleware<WebSocketMiddleware>();

// Middleware para rotas ou outras solicitações HTTP
app.MapGet("/", () => "Servidor de Chat WebSocket está rodando!");

app.Run();
