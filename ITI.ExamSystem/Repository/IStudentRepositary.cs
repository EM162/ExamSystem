using ITI.ExamSystem.DTOs.StudentDTO;
using ITI.ExamSystem.Models;

namespace ITI.ExamSystem.Repository
{
    public interface IStudentRepositary
    {
        User GetStudentProfile(int id);

        Task<List<StudentCoursesDTO>> GetCoursesByStudentId(int studentId);
        Task<List<Topic>> GetTopicsByCourseId(int courseId);
        Course GetCourseById(int courseId);

    }
}
