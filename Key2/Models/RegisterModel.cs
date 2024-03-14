namespace Key2.Models
{
    public class RegisterModel
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public DateTime? BirthDate { get; set; }
        public DateTime DateTime { get; set; } = DateTime.Now;
    }
}
