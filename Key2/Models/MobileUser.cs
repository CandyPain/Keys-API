using Keys.Models.Enums;

namespace Key2.Models
{
    public class MobileUser:User
    {
        public Role Role { get; set; }
        public Guid DeanId { get; set; }
        public bool? IsDeanWorker { get; set; }
        public DateTime? BirthDate { get; set; }
    }
}
