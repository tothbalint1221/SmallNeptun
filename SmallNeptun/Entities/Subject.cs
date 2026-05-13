namespace SmallNeptun.Entities
{
    public class Subject
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int Credits { get; set; }
        public bool IsActive { get; set; } = true;
        public ICollection<Course> Courses { get; set; } = new List<Course>();

        public ICollection<Grade> Grades { get; set; } = new List<Grade>();
        public ICollection<Signature> Signatures { get; set; } = new List<Signature>();
        public ICollection<Exam> Exams { get; set; } = new List<Exam>();


    }
}
