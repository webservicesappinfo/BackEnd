using Auth.Data;
using Auth.Extensions;
using Serilog;
using Auth.Models.DTO;
using AuthService.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Server.Kestrel.Core;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    options.ConfigureEndpointDefaults(listenOptions =>
    {
        listenOptions.Protocols = HttpProtocols.Http2;
    });
});

builder.Services.AddGrpc();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

// Register AutoMapper
builder.Services.AddAutoMapper(typeof(Program).Assembly);

// Register DataProtection and Infrastructure (PostgreSQL DbContext, Identity, JWT, Token Services)
builder.Services.AddDataProtection();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Host.UseSerilog((context, configuration) =>
    configuration
        .MinimumLevel.Debug()
        .WriteTo.Console()
        .ReadFrom.Configuration(context.Configuration));

var app = builder.Build();

// Ensure Database and Schema are created on PostgreSQL
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
    dbContext.Database.EnsureCreated();

    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();
    foreach (var roleName in Enum.GetNames<UserRole>())
    {
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            await roleManager.CreateAsync(new IdentityRole<int>(roleName));
        }
    }
}

app.UseAuthentication();
app.UseAuthorization();

app.MapGrpcService<AuthServiceImp>();

app.MapControllers();

app.MapGet("/", async context =>
{
    await context.Response.WriteAsync("Communication with gRPC endpoints must be made through a gRPC client.");
});

app.Run();
