using Globals.Models;

namespace UserService.Models
{
    public class UserCompanyRef : RefBase<User>
    {
        public string Name { get; set; }
    }
}
