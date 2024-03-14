using Keys.Models.Enums;
using Microsoft.AspNetCore.Identity;

namespace Key2.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public bool IsActive { get; set; } = false;
        public bool IsDenied { get; set; } = false;
        public string Name { get; set; } = "";
    }
}
