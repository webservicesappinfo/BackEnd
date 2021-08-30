using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LocationService.Models
{
    public class LocationContext : DbContext
    {
        public DbSet<LocationUser> Users { get; set; }

        public LocationContext() => Database.EnsureCreated();
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseNpgsql("Host=91.219.60.8;Port=5432;Database=locationusersdb;Username=root;Password=root");
    }
}
