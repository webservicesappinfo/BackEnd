using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Auth.Data;
using Auth.Models.Entities;
using Auth.Services;
using Auth.Services.Interfaces.Logging;
using Auth.Services.Interfaces.Users;
using Auth.Services.Logging;
using Auth.Services.Users;
using System.Text;
using Streetcode.Auth.Services;
using Npgsql;

namespace Auth.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>()
                           ?? configuration.GetSection("Jwt").Get<JwtSettings>()
                           ?? new JwtSettings();

            if (string.IsNullOrWhiteSpace(jwtSettings.Key))
            {
                jwtSettings.Key = configuration["JwtSettings:Secret"]
                               ?? configuration["Jwt:Secret"]
                               ?? configuration["JwtSettings:Key"]
                               ?? configuration["Jwt:Key"]
                               ?? string.Empty;
            }

            if (string.IsNullOrWhiteSpace(jwtSettings.Key))
            {
                throw new InvalidOperationException("Jwt key/secret is missing in configuration.");
            }

            services.AddSingleton(jwtSettings);

            var connectionString = GetPostgresConnectionString(configuration);

            services.AddDbContext<AuthDbContext>(options =>
                options.UseNpgsql(
                    connectionString,
                    npgsql => npgsql.MigrationsHistoryTable("__AuthMigrationsHistory", "auth")));

            services.AddIdentity<User, IdentityRole<int>>(options => {
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 3;
                options.User.RequireUniqueEmail = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
            })
            .AddEntityFrameworkStores<AuthDbContext>()
            .AddDefaultTokenProviders();

            services.AddScoped<IJwtTokenService, JwtTokenService>();
            services.AddScoped<IRefreshTokenService, RefreshTokenService>();
            services.AddScoped<IAuthService, Services.Users.AuthService>();
            services.AddScoped<ILoggerService, LoggerService>();
            services.AddHostedService<TokenCleanupService>();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key)),
                    ValidateIssuer = false,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero 
                };
            });

            return services;
        }

        public static string GetPostgresConnectionString(IConfiguration configuration)
        {
            var host = Environment.GetEnvironmentVariable("POSTGRES_HOST")
                    ?? Environment.GetEnvironmentVariable("DB_HOST");

            var connectionString = configuration.GetConnectionString("AuthDb")
                                ?? configuration.GetConnectionString("DefaultConnection");

            if (!string.IsNullOrWhiteSpace(host) || string.IsNullOrWhiteSpace(connectionString))
            {
                bool isRunningInDocker = string.Equals(
                    Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER"),
                    "true",
                    StringComparison.OrdinalIgnoreCase);

                var finalHost = !string.IsNullOrWhiteSpace(host)
                    ? host
                    : (isRunningInDocker ? "postgres" : "localhost");

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
                    Host = finalHost,
                    Port = port,
                    Database = dbName,
                    Username = user,
                    Password = password,
                    IncludeErrorDetail = true
                };

                return npgsqlConnectionStringBuilder.ConnectionString;
            }

            return connectionString;
        }
    }
}
