namespace SmallNeptun.Entities
{
    public class Grade
    {
        //A jegy tárgyra és félévre szól, nem konkrét kurzusra.
        public int Id { get; set; }
        public int Value { get; set; }
        public DateTime Time { get; set; }
        
        public int StudentId { get; set; }
        public User Student { get; set; }
        public int SubjectId { get; set; }
        public Subject Subject { get; set; }
        public int SemesterId { get; set; }
        public Semester Semester { get; set; }

    }
}
