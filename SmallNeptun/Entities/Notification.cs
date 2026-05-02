namespace SmallNeptun.Entities
{
    public class Notification
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public string Reason { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int UserId { get; set; }
        public User User { get; set; }
        public int CourseId { get; set; }
        public Course Course { get; set; }

    }
}
