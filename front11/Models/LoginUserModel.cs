using Microsoft.AspNetCore.Mvc;

namespace front11.Models
{
    public class LoginUserModel
    {
        public string? Sid { get; set; }
        public string? Spassword { get; set; }
        public string? Account { get; set; }
        public string? Apassword { get; set; }
        public string? UserRole { get; set; }
    }
}
