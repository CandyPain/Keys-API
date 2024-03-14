using Keys.Models.Enums;
using Microsoft.Identity.Client;

namespace Key2.Models
{
    public class AppChangeRole
    {
        public Guid Id { get; set; }
        public DateTime? CreateTime { get; set; }
        public string Name { get; set; }
        public Guid UserId { get; set; }
        public string Email { get; set; }
        public Role Role { get; set; }
        public Guid DeanId { get; set; }
    }
}
