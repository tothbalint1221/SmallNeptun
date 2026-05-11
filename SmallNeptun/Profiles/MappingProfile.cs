using AutoMapper;
using SmallNeptun.Dtos.Subjects;
using SmallNeptun.Dtos.Users;
using SmallNeptun.Entities;

namespace SmallNeptun.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<RegisterUserDto, User>();
            CreateMap<User, UserViewDto>();

            CreateMap<CreateSubjectDto, Subject>();
            CreateMap<Subject, SubjectViewDto>();
        }
    }
}
