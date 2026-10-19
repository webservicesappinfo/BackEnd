using Auth.Data;
using Auth.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

// Build PostgreSQL connection string
var host = Environment.GetEnvironmentVariable("POSTGRES_HOST")
        ?? Environment.GetEnvironmentVariable("DB_HOST");

if (string.IsNullOrWhiteSpace(host))
{
    bool isRunningInDocker = string.Equals(
        Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER"),
        "true",
        StringComparison.OrdinalIgnoreCase);

    host = isRunningInDocker ? "postgres" : "localhost";
}

var portEnv = Environment.GetEnvironmentVariable("POSTGRES_PORT")
           ?? Environment.GetEnvironmentVariable("DB_PORT");
int port = int.TryParse(portEnv, out var p) ? p : 5432;

var user = Environment.GetEnvironmentVariable("POSTGRES_USER")
        ?? Environment.GetEnvironmentVariable("DB_USER")
        ?? "postgres";

var password = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD")
            ?? Environment.GetEnvironmentVariable("DB_PASSWORD")
            ?? "postgres";

var dbName = Environment.GetEnvironmentVariable("POSTGRES_DB") ?? "authdb";

var npgsqlConnectionStringBuilder = new NpgsqlConnectionStringBuilder
{
    Host = host,
    Port = port,
    Database = dbName,
    Username = user,
    Password = password,
    IncludeErrorDetail = true
};

var connectionString = builder.Configuration.GetConnectionString("AuthDb");
if (!string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("POSTGRES_HOST")) || string.IsNullOrWhiteSpace(connectionString))
{
    connectionString = npgsqlConnectionStringBuilder.ConnectionString;
}

// Register DbContext
builder.Services.AddDbContext<AuthDbContext>(options =>
{
    options.UseNpgsql(connectionString);
});

// Register Identity and DataProtection
builder.Services.AddDataProtection();
builder.Services.AddIdentityCore<User>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
})
.AddRoles<IdentityRole<int>>()
.AddEntityFrameworkStores<AuthDbContext>()
.AddDefaultTokenProviders();

var app = builder.Build();

// Ensure Database and Schema are created on PostgreSQL
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
    dbContext.Database.EnsureCreated();
}

//app.MapGet("/", () => "Hello World!");

app.MapControllers();

app.Run();
