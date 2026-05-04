using SmallNeptun.Enums.SignatureEnums;

namespace SmallNeptun.Entities
{
    public class Signature
    {
        public int Id { get; set; }
        public SignatureValue Value { get; set; }
        public DateTime Time { get; set; }
        public int StudentId { get; set; }
        public User Student { get; set; }
        public int SubjectId { get; set; }
        public Subject Subject { get; set; }
        public int SemesterId { get; set; }
        public Semester Semester { get; set; }



    }
}
