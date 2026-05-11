using SmallNeptun.Enums.UserEnums;

namespace SmallNeptun.Dtos.Users
{
    public class RegisterUserDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        /// <summary>Ertekek: Student, Teacher, Agent.</summary>
        public UserType UserType { get; set; }
        /// <summary>Ertekek: None, FullTime, PartTime.</summary>
        public StudyForm StudyForm { get; set; } = StudyForm.None;
    }
}
