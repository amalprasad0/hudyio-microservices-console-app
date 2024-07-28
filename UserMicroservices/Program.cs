using static UserMicroservices.Respository.DB.DbConnection;
using UserMicroservices.Interface;
using UserMicroservices.Respository.DAL;
using UserMicroservices.Helpers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddScoped<SqlDataAccess>();
builder.Services.AddScoped<IUser, UserMember>();
builder.Services.AddScoped<IHelpers, Helpers>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
