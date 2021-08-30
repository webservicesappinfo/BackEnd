using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LocationService.Models
{
    public class LocationUser
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Guid { get; set; }
        public double? Lat { get; set; }
        public double? Lng { get; set; }
    }
}
