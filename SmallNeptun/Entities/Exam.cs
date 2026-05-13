namespace SmallNeptun.Entities
{
    public class Exam
    {
        public int Id { get; set; }
        public DateTime ExamTime { get; set; }

        public int SubjectId { get; set; }
        public Subject Subject { get; set; }

        public int SemesterId { get; set; }
        public Semester Semester { get; set; }

        public ICollection<ExamRegistration> ExamRegistrations { get; set; } = new List<ExamRegistration>();
    }
}
