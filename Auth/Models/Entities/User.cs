using Auth.Models.DTO;
using Microsoft.AspNetCore.Identity;

namespace Auth.Models.Entities
{
    public class User : IdentityUser<int>
    {
        required public string Name { get; set; }
        required public string Surname { get; set; }

        public UserRole Role { get; set; }
    }
}
