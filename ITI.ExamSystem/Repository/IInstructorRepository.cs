
using ITI.ExamSystem.DTOs.InstructorsDTO;
using ITI.ExamSystem.Models;

namespace ITI.ExamSystem.Repository
{
    public interface IInstructorRepository
    {
        Task<InstructorProfileDto> GetInstructorProfileAsync(int userId);
        Task<List<Course>> GetInstructorCoursesAsync(int userId);
    }
}
