using Hangfire;
using Hangfire.SqlServer;
using ScheduledTaskService.Interface;
using ScheduledTaskService.Services;
using ScheduledTaskService.Tasks;
using System.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);

// Add Hangfire services
builder.Services.AddHangfire(configuration => configuration
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseSqlServerStorage(builder.Configuration.GetConnectionString("HangfireConnection"), new SqlServerStorageOptions
    {
        CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
        SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
        QueuePollInterval = TimeSpan.Zero,
        UseRecommendedIsolationLevel = true,
        UsePageLocksOnDequeue = true,
        DisableGlobalLocks = true
    }));

builder.Services.AddHangfireServer();

// Register services
builder.Services.AddSingleton<IJobTaskScheduler, JobTaskScheduler>();
builder.Services.AddSingleton<SyncSQLData>();

// Register HttpClient
builder.Services.AddHttpClient("CacheService", client =>
{
    client.BaseAddress = new Uri("https://localhost:7097");
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
});

builder.Services.AddControllers(); 
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseHangfireDashboard("/hangfire");

var syncSQLData = app.Services.GetRequiredService<SyncSQLData>();

syncSQLData.ScheduleApiDataFetchJob(
    "synccache",
    "/api/Cache/SyncCachedData",
    "0 0 * * *");
syncSQLData.ScheduleApiDataFetchJob(
    "removeCache",
    "/api/Cache/RemoveCachedMessages",
    "0 0 * * *");
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
