namespace SmallNeptun.Dtos.Exams
{
    public class CreateExamDto
    {
        public int SubjectId { get; set; }
        public int SemesterId { get; set; }
        public DateTime ExamTime { get; set; }
    }
}
