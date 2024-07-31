using static UserMicroservices.Respository.DB.DbConnection;
using UserMicroservices.Interface;
using UserMicroservices.Respository.DAL;
using System.Reflection.PortableExecutable;
using UserMicroservices.Repository.HelperRepository;
using UserMicroservices.Dependencies;

var builder = WebApplication.CreateBuilder(args);

// Configure Kestrel to listen on port 80
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenAnyIP(80);
});

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddScoped<SqlDataAccess>();
builder.Services.AddScoped<IUser, UserMember>();
builder.Services.AddScoped<IHelpers, HelperRepository>();
builder.Services.AddScoped<Fast2SmsApi>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline

    app.UseSwagger();
    app.UseSwaggerUI();


app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
