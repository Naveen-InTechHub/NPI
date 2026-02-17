using System.ComponentModel.DataAnnotations;

namespace NPI.Client.DTOs.Auth
{
    public class UserData
    {
        public int Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }

    public class LoginModel
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
