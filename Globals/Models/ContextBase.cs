using Microsoft.EntityFrameworkCore;
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
            => optionsBuilder.UseNpgsql($"Host=91.219.60.8;Port=5432;Database={TypeName }db;Username=root;Password=root");

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Ignore<EntityBase>();
            builder.Entity<T>().ToTable(TypeName);
        }
    }
}
