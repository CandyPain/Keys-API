using Keys.Models.Enums;

namespace Key2.Models
{
    public class Application
    {
        public Guid Id { get; set; }
        public Key Key { get; set; }
        public User AppFromUser { get; set; }
        public ScheduleCell Schedule { get; set; }
        public DateTime Date { get; set; }
        public bool IsConfirmation { get; set; }
        public bool IsRepeat { get; set; }
    }
}
