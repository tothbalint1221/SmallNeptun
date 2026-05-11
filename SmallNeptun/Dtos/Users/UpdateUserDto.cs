using SmallNeptun.Enums.UserEnums;

namespace SmallNeptun.Dtos.Users
{
    public class UpdateUserDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
        /// <summary>Ertekek: None, FullTime, PartTime.</summary>
        public StudyForm StudyForm { get; set; } = StudyForm.None;
    }
}
