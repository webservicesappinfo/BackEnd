using LocationService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LocationService.Abstractions
{
    public interface ILocationRepoService
    {
        LocationUser GetUser(string guid);
        Boolean AddUser(LocationUser user);
        void GetUserLocation(string guid, out string lat, out string lng);
        Boolean SetUserLocation(string guid, string lat, string lng);
    }
}
