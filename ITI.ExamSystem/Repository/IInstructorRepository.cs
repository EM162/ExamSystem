
using ITI.ExamSystem.DTOs.InstructorsDTO;
using ITI.ExamSystem.Models;

namespace ITI.ExamSystem.Repository
{
    public interface IInstructorRepository
    {
        Task<InstructorProfileDto> GetInstructorProfileAsync(int userId);
        Task<List<InstructorCourseDto>> GetInstructorCoursesAsync(int userId);
        void update(int iuserId,string img);
        Task<List<Topic>> GetInstructorCourseTopicsAsync(int instructorId, int courseId);
        Task<List<User>> GetCourseStudentsWithExamDetailsAsync(int instructorId, int courseId);
        Task<List<Exam>> GetInstructorCourseExamsAsync(int instructorId, int courseId);
        Task<List<PublishedExam>> GetInstructorPublishedExamsAsync(int instructorId, int courseId);
        Task<List<User>> GetInstructorStudentsAsync(int instructorId);
        Task<List<Exam>> GetInstructorCreatedExamsAsync(int instructorId);
        Task<List<PublishedExam>> GetInstructorPublishedExamsAsync(int instructorId);
    }
}
