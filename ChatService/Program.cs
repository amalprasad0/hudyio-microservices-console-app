using ChatService.Hubs;
using ChatService.Interface;
using ChatService.Repository;
using ChatService.Repository.MessageStoreService;
using ChatService.Utilities;
using System.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpClient("UserService", client =>
{
    client.BaseAddress = new Uri("http://host.docker.internal:4202"); 
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
});
/*builder.Services.AddHttpClient("CacheService", client =>
{
    client.BaseAddress = new Uri("http://host.docker.internal:4204");
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
});*/
//local run
builder.Services.AddHttpClient("CacheService", client =>
{
    client.BaseAddress = new Uri("https://localhost:7097");
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
});
builder.Services.AddHttpClient("MessageService", client =>
{
    client.BaseAddress = new Uri("https://localhost:7213");
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
});

/*builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenAnyIP(80);
});*/
builder.Services.AddScoped<IMessageQueue, UserMessageRepository>();
builder.Services.AddScoped<IMessageService, MessageService>();
builder.Services.AddScoped<CacheServiceClient>();
builder.Services.AddScoped<UserConnectionManager>();
builder.Services.AddScoped<ApiUtility>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSignalR();

var app = builder.Build();

app.UseWebSockets();


app.MapHub<MessageHub>("/chat-hub");
await app.RunAsync();
