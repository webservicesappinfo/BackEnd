using Auth.Models.Entities;

namespace Auth.Extensions
{
    public static class UserExtensions
    {
        public static void EnsureSecurityStamp(this User user)
        {
            if (string.IsNullOrEmpty(user.SecurityStamp))
            {
                user.SecurityStamp = Guid.NewGuid().ToString();
            }
        }
    }
}
