using Microsoft.EntityFrameworkCore;
using Npgsql;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Globals.Models
{
    public class ContextBase<T> : DbContext where T : EntityBase
    {
        private static readonly ConcurrentDictionary<Type, bool> _initializedContexts = new();
        private String TypeName => typeof(T).Name.Replace("Context", String.Empty);
        public DbSet<T> Values { get; set; }

        public ContextBase()
        {
            EnsureDatabaseCreated();
        }

        private void EnsureDatabaseCreated()
        {
            var type = GetType();
            if (!_initializedContexts.ContainsKey(type))
            {
                lock (_initializedContexts)
                {
                    if (!_initializedContexts.ContainsKey(type))
                    {
                        Database.EnsureCreated();
                        _initializedContexts[type] = true;
                    }
                }
            }
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
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

            NpgsqlConnectionStringBuilder npgsqlConnectionStringBuilder = new NpgsqlConnectionStringBuilder
            {
                Host = host,
                Port = port,
                Database = $"{TypeName}db",
                Username = user,
                Password = password,
                IncludeErrorDetails = true
            };

            optionsBuilder.UseNpgsql(npgsqlConnectionStringBuilder.ConnectionString);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Ignore<EntityBase>();
            builder.Entity<T>().ToTable(TypeName);

            ModelBuilderConfigure(builder);
        }

        protected virtual void ModelBuilderConfigure(ModelBuilder builder) { }
    }
}
