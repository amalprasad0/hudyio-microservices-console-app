using ChatService.Hubs;
using System.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpClient("ServiceClient", client =>
{
    client.BaseAddress = new Uri("http://host.docker.internal:4202"); 
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
});
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenAnyIP(80);
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSignalR();

var app = builder.Build();

app.UseWebSockets();


app.MapHub<ChatHub>("/chat-hub");
await app.RunAsync();
