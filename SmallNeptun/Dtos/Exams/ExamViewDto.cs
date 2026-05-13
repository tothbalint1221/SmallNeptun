namespace SmallNeptun.Dtos.Exams
{
    public class ExamViewDto
    {
        public int Id { get; set; }
        public int SubjectId { get; set; }
        public string SubjectCode { get; set; }
        public string SubjectName { get; set; }
        public int SemesterId { get; set; }
        public string Semester { get; set; }
        public DateTime ExamTime { get; set; }
        public int RegistrationCount { get; set; }
    }
}
