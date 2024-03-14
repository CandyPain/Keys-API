using Keys.Models.Enums;

namespace Key2.Models
{
    public class CreateAppModel
    {
        public Guid KeyId { get; set; }
        public ScheduleCell Schedule { get; set; }
        public DateTime Date { get; set; }
        public bool isRepeat { get; set; }
    }
}
