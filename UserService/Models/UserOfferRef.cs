using Globals.Models;

namespace UserService.Models
{
    public class UserOfferRef : RefBase<User>
    {
        public string Name { get; set; }
    }
}
