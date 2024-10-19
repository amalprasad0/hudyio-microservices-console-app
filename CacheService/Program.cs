using CacheService.CacheMigration;
using CacheService.Helper;
using CacheService.Interfaces;
using CacheService.Repository;
using CacheService.Services;
using System.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);
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
builder.Services.AddScoped<IRedisCache, CacheRespository>();
builder.Services.AddScoped<IQueueCache,QueueRepository>();
builder.Services.AddHttpClient("UserService", client =>
{
    client.BaseAddress = new Uri("https://localhost:7034");
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
});
builder.Services.AddHttpClient("MessageMngService", client =>
{
    client.BaseAddress = new Uri("https://localhost:7213");
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
});
/*builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenAnyIP(80);
});*/
builder.Services.AddScoped<IGetCachedData, GetCachedData>();
builder.Services.AddScoped<ApiService>();
builder.Services.AddScoped<IGetCachedData, GetCachedData>();
builder.Services.AddScoped<ISyncDataToSql,SyncCacheToSql>();
builder.Services.AddScoped<IGetDBCacheUsercs,GetDBCachedUsers>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    app.MapGrpcReflectionService();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
