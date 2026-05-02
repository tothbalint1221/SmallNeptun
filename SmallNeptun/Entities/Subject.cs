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
    }
}
