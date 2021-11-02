using Globals.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserService.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Guid { get; set; }
        public string Token { get; set; }
        public String Name { get; set; }
        public String LastName { get; set; }
        public String Email { get; set; }
        public List<Ref> Companies { get; set; }
        public List<Ref> Offers { get; set; }
    }
}
