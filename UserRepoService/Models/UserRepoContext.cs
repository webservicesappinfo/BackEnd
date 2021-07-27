using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace UserRepoService.Models
{
    public class UserRepoContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public UserRepoContext() => Database.EnsureCreated();
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseNpgsql("Host=91.219.60.8;Port=5432;Database=usersdb;Username=root;Password=root");
    }
}
