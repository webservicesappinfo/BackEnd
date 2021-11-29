using Microsoft.EntityFrameworkCore;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Globals.Models
{
    public class ContextBase<T> : DbContext where T : EntityBase
    {
        private String TypeName => typeof(T).Name.Replace("Context", String.Empty);
        public DbSet<T> Values { get; set; }

        public ContextBase() => Database.EnsureCreated();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            NpgsqlConnectionStringBuilder npgsqlConnectionStringBuilder = new NpgsqlConnectionStringBuilder();
            npgsqlConnectionStringBuilder.Host = "91.219.60.8";
            npgsqlConnectionStringBuilder.Port = 5432;
            npgsqlConnectionStringBuilder.Database = $"{TypeName }db";
            npgsqlConnectionStringBuilder.Username = "root";
            npgsqlConnectionStringBuilder.Password = "root";
            npgsqlConnectionStringBuilder.IncludeErrorDetails = true;

            optionsBuilder.UseNpgsql(npgsqlConnectionStringBuilder.ConnectionString);

            //var conn = new NpgsqlConnectionFactory().CreateConnection(npgsqlConnectionStringBuilder.ConnectionString.ToString());
            //optionsBuilder.UseNpgsql($"Host=91.219.60.8;Port=5432;Database={TypeName }db;Username=root;Password=root");

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
