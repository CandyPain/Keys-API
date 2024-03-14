using Keys.Models.Enums;

namespace Key2.Models
{
    public class KeySchedulePreview
    {
        public DateTime Date {  get; set; }
        public ScheduleCell Sheduller { get; set; }
        public Guid? UserId { get; set; }
        public string? UserName { get; set; }
    }
}
