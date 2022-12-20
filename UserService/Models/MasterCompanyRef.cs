using Globals.Models;

namespace UserService.Models
{
    public class MasterCompanyRef : RefBase<User>
    {
        public string Name { get; set; }
    }
}
