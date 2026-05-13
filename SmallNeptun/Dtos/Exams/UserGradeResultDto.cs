namespace SmallNeptun.Dtos.Exams
{
    public class UserGradeResultDto
    {
        public int SubjectId { get; set; }
        public string SubjectCode { get; set; }
        public string SubjectName { get; set; }
        public int SemesterId { get; set; }
        public string Semester { get; set; }
        public string Result { get; set; }
        public int? Grade { get; set; }
        public DateTime? GradeDate { get; set; }
    }
}
