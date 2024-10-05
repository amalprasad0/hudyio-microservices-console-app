using MessageManagementService.Interface;
using MessageManagementService.ProtoContracts;
using MessageManagementService.Services.DBCacheService;
using MessageManagementService.Services.MsgStoreServices;
using MessageManagementService.Utilities;
using System.Data.SqlTypes;

var builder = WebApplication.CreateBuilder(args);


/*builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenAnyIP(80);
});*/
builder.Services.AddGrpc(options =>
{
    //options.KeepAlive = new TimeSpan(0, 0, 60); // 60 second keepalive
    options.MaxReceiveMessageSize = 1024 * 1024 * 10; // 10 MB
    options.MaxSendMessageSize = 1024 * 1024 * 10; // 10 MB
    options.EnableDetailedErrors = true;
});
builder.Services.AddControllers();
builder.Services.AddGrpcReflection();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IMessageManagement,MessageManagement>();
builder.Services.AddScoped<IDBCacheService, DBCacheService>();
builder.Services.AddScoped<SqlHelper>();
builder.Services.AddScoped<MessageServicePc>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    app.MapGrpcReflectionService();
}
app.MapGrpcService<MessageServicePc>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
