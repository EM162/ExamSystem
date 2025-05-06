using ITI.ExamSystem.DTOs.StudentDTO;
using ITI.ExamSystem.Models;

namespace ITI.ExamSystem.Repository
{
    public interface IStudentRepositary
    {
        User GetStudentProfile(Guid id);

        Task<List<StudentCoursesDTO>> GetCoursesByStudentId(Guid studentId);
        Task<List<Topic>> GetTopicsByCourseId(int courseId);
        Course GetCourseById(int courseId);

    }
}
