namespace SmallNeptun.Entities
{
    public class Enrollment
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public User Student { get; set; }

        public int CourseId { get; set; }
        public Course Course { get; set; }

        //public int GradeId { get; set; }
        //public Grade? Grade { get; set; }
        ////public bool Signature { get; set; }

    }
}
