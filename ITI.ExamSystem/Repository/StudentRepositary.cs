using ITI.ExamSystem.DTOs.StudentDTO;
using ITI.ExamSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace ITI.ExamSystem.Repository
{
    public class StudentRepositary : IStudentRepositary
    {
        private OnlineExaminationDBContext _db;

        public StudentRepositary(OnlineExaminationDBContext db)
        {

            this._db = db;
        }



        public User GetStudentProfile(Guid id)
        {
            return _db.Users
           .Include(d => d.IntakeBranchTrackUsers)
               .ThenInclude(b => b.Branch)
           .Include(d => d.IntakeBranchTrackUsers)
               .ThenInclude(i => i.Intake)
           .Include(d => d.IntakeBranchTrackUsers)
               .ThenInclude(t => t.Track)
           .Include(c => c.Courses)
            .FirstOrDefault(s => s.IdentityUserId == id.ToString());
        }



        public async Task<List<Topic>> GetTopicsByCourseId(int courseId)
        {
            return await _db.Topics.Where(t => t.CourseID == courseId).ToListAsync();
        }

        public async Task<List<StudentCoursesDTO>> GetCoursesByStudentId(Guid studentId)
        {
            // First, find the internal UserID (int) by GUID
            var user = await _db.Users.FirstOrDefaultAsync(u => u.IdentityUserId == studentId.ToString());
            if (user == null)
            {
                return new List<StudentCoursesDTO>();  // or throw an error if preferred
            }

            var courses = await _db.Users
                .Where(u => u.UserID == user.UserID)
                .Include(u => u.Courses)
                .SelectMany(u => u.Courses)
                .Distinct()
                .ToListAsync();

            var userExams = await _db.UserExams
                .Where(ue => ue.UserID == user.UserID)
                .Include(ue => ue.Exam)
                    .ThenInclude(ex => ex.Course)
                .ToListAsync();

            var studentCourses = courses.Select(course => new StudentCoursesDTO
            {
                CourseID = course.CourseID,
                CourseName = course.Name,
                Duration = course.Duration,
                CourseImagePath = course.CourseImagePath,
                Topics = course.Topics.Select(t => new StudentTopicsDTO
                {
                    TopicId = t.TopicID,
                    TopicName = t.TopicName,
                    Description = t.Description
                }).ToList(),
                Grade = userExams.FirstOrDefault(ue => ue.Exam.CourseID == course.CourseID)?.Grade ?? 0
            }).ToList();

            return studentCourses;
        }


        public Course GetCourseById(int Id)
        {
            return _db.Courses.FirstOrDefault(c => c.CourseID == Id);
        }
    }

}
