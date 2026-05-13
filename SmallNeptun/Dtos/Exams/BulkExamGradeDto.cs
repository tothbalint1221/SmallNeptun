namespace SmallNeptun.Dtos.Exams
{
    public class BulkExamGradeDto
    {
        public List<BulkExamGradeItemDto> Grades { get; set; } = new List<BulkExamGradeItemDto>();
    }
}
