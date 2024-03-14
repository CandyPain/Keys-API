using Keys.Models.Enums;

namespace Key2.Models
{
    public class RoleResponseModel
    {
        public Role Role { get; set; }
        public bool IsActive { get; set; } 
        public bool IsDenied { get; set; }
        public bool IsDeanWorker { get; set; }
    }
}
