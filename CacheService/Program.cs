using CacheService.CacheMigration;
using CacheService.Helper;
using CacheService.Interfaces;
using CacheService.Repository;
using CacheService.Services;
using System.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
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
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
