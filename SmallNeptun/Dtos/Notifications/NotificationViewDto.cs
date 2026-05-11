namespace SmallNeptun.Dtos.Notifications
{
    public class NotificationViewDto
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public string Reason { get; set; }
        public DateTime CreatedAt { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public int CourseId { get; set; }
        public string CourseCode { get; set; }
    }
}
