using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SmallNeptun.Dtos.Subjects;
using SmallNeptun.Entities;
using SmallNeptun.Repository;

namespace SmallNeptun.Services.Subjects
{
    public class SubjectService : ISubjectService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public SubjectService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<SubjectViewDto>> GetAllAsync(SubjectQueryDto query)
        {
            var subjectsQuery = _unitOfWork.Subjects.Query();

            if (!query.IncludeInactive)
            {
                subjectsQuery = subjectsQuery.Where(s => s.IsActive);
            }

            var subjects = await subjectsQuery.ToListAsync();
            return _mapper.Map<IEnumerable<SubjectViewDto>>(subjects);
        }

        public async Task<(int statusCode, string errorMessage, SubjectViewDto? subject)> GetByIdAsync(int subjectId)
        {
            var subject = await _unitOfWork.Subjects.GetByIdAsync(subjectId);
            if (subject is null)
            {
                return (1, $"Subject with id {subjectId} was not found.", null);
            }

            return (0, "", _mapper.Map<SubjectViewDto>(subject));
        }

        public async Task<(int statusCode, string errorMessage, SubjectViewDto? subject)> CreateAsync(CreateSubjectDto dto)
        {
            if (await _unitOfWork.Subjects.Query().AnyAsync(s => s.Code == dto.Code))
            {
                return (2, "Subject code is already used.", null);
            }

            if (dto.Credits <= 0)
            {
                return (3, "Credits must be greater than zero.", null);
            }

            var subject = _mapper.Map<Subject>(dto);
            subject.IsActive = true;

            await _unitOfWork.Subjects.AddAsync(subject);
            await _unitOfWork.SaveAsync();

            return (0, "", _mapper.Map<SubjectViewDto>(subject));
        }

        public async Task<(int statusCode, string errorMessage, SubjectViewDto? subject)> UpdateAsync(int subjectId, UpdateSubjectDto dto)
        {
            var subject = await _unitOfWork.Subjects.GetByIdAsync(subjectId);
            if (subject is null)
            {
                return (1, $"Subject with id {subjectId} was not found.", null);
            }

            if (await _unitOfWork.Subjects.Query().AnyAsync(s => s.Code == dto.Code && s.Id != subjectId))
            {
                return (2, "Subject code is already used.", null);
            }

            if (dto.Credits <= 0)
            {
                return (3, "Credits must be greater than zero.", null);
            }

            subject.Code = dto.Code;
            subject.Name = dto.Name;
            subject.Credits = dto.Credits;

            _unitOfWork.Subjects.Update(subject);
            await _unitOfWork.SaveAsync();

            return (0, "", _mapper.Map<SubjectViewDto>(subject));
        }

        public async Task<(int statusCode, string errorMessage)> DeactivateAsync(int subjectId)
        {
            var subject = await _unitOfWork.Subjects.GetByIdAsync(subjectId);
            if (subject is null)
            {
                return (1, $"Subject with id {subjectId} was not found.");
            }

            subject.IsActive = false;
            _unitOfWork.Subjects.Update(subject);
            await _unitOfWork.SaveAsync();

            return (0, "");
        }

        public async Task<(int statusCode, string errorMessage)> ReactivateAsync(int subjectId)
        {
            var subject = await _unitOfWork.Subjects.GetByIdAsync(subjectId);
            if (subject is null)
            {
                return (1, $"Subject with id {subjectId} was not found.");
            }

            subject.IsActive = true;
            _unitOfWork.Subjects.Update(subject);
            await _unitOfWork.SaveAsync();

            return (0, "");
        }
    }
}
