using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace NotificationService.Models
{
    public class NotificationContext : DbContext
    {
        public DbSet<NotificationUser> Users { get; set; }

        public NotificationContext() => Database.EnsureCreated();
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseNpgsql("Host=91.219.60.8;Port=5432;Database=notificationusersdb;Username=root;Password=root");
    }
}
