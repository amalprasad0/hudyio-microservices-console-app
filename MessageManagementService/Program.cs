using MessageManagementService.Interface;
using MessageManagementService.Services.DBCacheService;
using MessageManagementService.Services.MsgStoreServices;
using MessageManagementService.Utilities;
using System.Data.SqlTypes;

var builder = WebApplication.CreateBuilder(args);


/*builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenAnyIP(80);
});*/
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IMessageManagement,MessageManagement>();
builder.Services.AddScoped<IDBCacheService, DBCacheService>();
builder.Services.AddScoped<SqlHelper>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
