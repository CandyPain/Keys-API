namespace Key2.Models
{
    public class Key
    {
        public Guid Id { get; set; }
        public int Number { get; set; }
        public Guid? CurrentUserId { get; set; }
        public User? CurrentUser { get; set; }
        public Guid DeanId { get; set; }
        public int Building { get; set; } = 0;
    }
}
