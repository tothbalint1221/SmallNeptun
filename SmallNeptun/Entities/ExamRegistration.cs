namespace SmallNeptun.Entities
{
    public class ExamRegistration
    {
        public int Id { get; set; }
        public int? Grade { get; set; }
        public DateTime? GradeDate { get; set; }

        public int ExamId { get; set; }
        public Exam Exam { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }
    }
}
