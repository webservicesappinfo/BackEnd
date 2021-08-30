using EventBus.Abstractions;
using LocationService.Abstractions;
using LocationService.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LocationService.Services
{
    public class LocationRepoService : ILocationRepoService
    {
        private readonly ILogger<LocationRepoService> _logger;
        private readonly IEventBus _eventBus;

        public LocationRepoService(ILogger<LocationRepoService> logger, IEventBus eventBus)
        {
            _logger = logger;
            _eventBus = eventBus;
        }

        public bool AddUser(LocationUser user)
        {
            using (var db = new LocationContext())
            {
                if (db.Users.Any(x => x.Guid == user.Guid)) return false;
                db.Users.Add(user);
                db.SaveChanges();
            }
            return true;
        }

        public LocationUser GetUser(string guid)
        {
            LocationUser user = null;
            using (var db = new LocationContext())
                user = db.Users.FirstOrDefault(x => x.Guid == guid);
            return user;
        }

        public void GetUserLocation(string guid, out string lat, out string lng)
        {
            lat = String.Empty;
            lng = String.Empty;
            using (var db = new LocationContext())
            {
                var user = db.Users.FirstOrDefault(x => x.Guid == guid);
                lat = user?.Lat?.ToString() ?? String.Empty;
                lng = user?.Lng?.ToString() ?? String.Empty;
            }
        }

        public bool SetUserLocation(string guid, string lat, string lng)
        {
            using (var db = new LocationContext())
            {
                var user = db.Users.FirstOrDefault(x => x.Guid == guid);
                if (user == null 
                    || !Double.TryParse(lat, out double dLat) 
                    || !Double.TryParse(lng, out double dLng)) return false;
                user.Lat = dLat;
                user.Lng = dLng;
                db.SaveChanges();
                return true;
            }
        }
    }
}
