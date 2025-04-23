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



        public User GetStudentProfile(int id)
        {

            return _db.Users
       .Include(d => d.IntakeBranchTrackUsers)
           .ThenInclude(b => b.Branch)
       .Include(d => d.IntakeBranchTrackUsers)
           .ThenInclude(i => i.Intake)
       .Include(d => d.IntakeBranchTrackUsers)
           .ThenInclude(t => t.Track)
       .Include(c => c.Courses)
       .FirstOrDefault(s => s.UserID == id);
        }



        public async Task<List<Topic>> GetTopicsByCourseId(int courseId)
        {
            return await _db.Topics.Where(t => t.CourseID == courseId).ToListAsync();
        }

        /*
        public async Task<List<Course>> GetCoursesByStudentId(int studentId)
        {
            return await _db.Users.Where(us => us.UserID == studentId)
               .Include(c=> c.Courses)
               
                .SelectMany(s=> s.Courses.Select(c=> new Course
                {
                    CourseID= c.CourseID,
                    Name = c.Name,
                    Duration = c.Duration,
                    Topics = c.Topics.ToList(),
                    grade = _db.UserExams.Where(g=> g.UserID == studentId &&)

                }))
                .Include(d => d.grade)
                .Include(s => s.Topics)
                
                .ToListAsync();
        }
    }
    */


        
        public async Task<List<StudentCoursesDTO>> GetCoursesByStudentId(int studentId)
        {
            var courses = await _db.Users
                .Where(u => u.UserID == studentId)
                .Include(u => u.Courses) 
                .SelectMany(u => u.Courses) 
                .Distinct()
                .ToListAsync();

            var userExams = await _db.UserExams
                .Where(ue => ue.UserID == studentId)
                .Include(ue => ue.Exam)
                    .ThenInclude(ex => ex.Course)
                .ToListAsync();

            var studentCourses = courses.Select(course => new StudentCoursesDTO
            {
                CourseID = course.CourseID,
                CourseName = course.Name,
                Duration = course.Duration,
               
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
        
        /*
        public async Task<List<StudentCoursesDTO>> GetCoursesByStudentId(int studentId)
        {
            var studentCourses = await (from course in _db.Courses
                                        join userExam in _db.UserExams
                                        on course.CourseID equals userExam.Exam.CourseID into userExams
                                        from userExam in userExams.DefaultIfEmpty() // Left Join to get all courses even if no exam record exists
                                        where userExam.UserID == studentId || userExam == null // Ensure we get the course even if no exam record exists
                                        select new StudentCoursesDTO
                                        {
                                            CourseID = course.CourseID,
                                            CourseName = course.Name,
                                            Duration = course.Duration,
                                            Grade = userExam.Grade , // If no exam, return grade as 0
                                            Topics = course.Topics.Select(t => new StudentTopicsDTO
                                            {
                                                TopicId = t.TopicID,
                                                TopicName = t.TopicName,
                                                Description = t.Description
                                            }).ToList()
                                        })
                                         .ToListAsync();

            return studentCourses;
        }
        */
    }

}
