using ChatService.Hubs;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSignalR();

var app = builder.Build();

app.UseWebSockets();


app.MapHub<ChatHub>("/chat-hub");
await app.RunAsync();
