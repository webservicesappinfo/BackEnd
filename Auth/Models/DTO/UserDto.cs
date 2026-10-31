using System.Text.Json.Serialization;

namespace Auth.Models.DTO
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Login { get; set; } = string.Empty;

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public UserRole Role { get; set; }
    }

    public enum UserRole
    {
        MainAdministrator,
        Admin,
        Moderator,
        User,
        Guest
    }
}
