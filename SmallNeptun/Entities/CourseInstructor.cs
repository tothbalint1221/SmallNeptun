namespace SmallNeptun.Entities
{
    public class CourseInstructor
    {
        public int Id { get; set; }
        public int TeacherId { get; set; }
        public User Teacher { get; set; }

        public int CourseId { get; set; }
        public Course Course { get; set; }

    }
}
