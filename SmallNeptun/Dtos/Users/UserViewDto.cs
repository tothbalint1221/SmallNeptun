using SmallNeptun.Enums.UserEnums;

namespace SmallNeptun.Dtos.Users
{
    public class UserViewDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public UserType UserType { get; set; }
        public StudyForm StudyForm { get; set; }
        public bool IsActive { get; set; }
    }
}
