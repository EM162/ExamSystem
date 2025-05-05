using ITI.ExamSystem.DTOs.InstructorsDTO;
using ITI.ExamSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace ITI.ExamSystem.Repository
{
    public class InstructorRepository:IInstructorRepository
    {
        private readonly OnlineExaminationDBContext _context;

        public InstructorRepository(OnlineExaminationDBContext context)
        {
            _context = context;
        }

        // جلب بيانات البروفايل
        public async Task<InstructorProfileDto> GetInstructorProfileAsync(int userId)
        {
            var user = await _context.Users
                .Where(u => u.UserID == userId && u.Roles.Any(r => r.RoleName == "Instructor"))
                .Include(u => u.Courses)
                    .ThenInclude(c => c.Users)
                        .ThenInclude(u => u.Roles)
                .Include(u => u.Courses)
                    .ThenInclude(c => c.Exams)
                .Include(u => u.Courses)
                    .ThenInclude(c => c.PublishedExams)
                .Include(u => u.IntakeBranchTrackUsers)
                    .ThenInclude(ibt => ibt.Branch)
                .Include(u => u.IntakeBranchTrackUsers)
                    .ThenInclude(ibt => ibt.Intake)
                .Include(u => u.IntakeBranchTrackUsers)
                    .ThenInclude(ibt => ibt.Track)
                .FirstOrDefaultAsync();

            if (user == null) return null;

            var courses = user.Courses;

            // حساب عدد الطلاب الحقيقيين (Role = "Student")
            var allStudentsCount = courses
                .SelectMany(c => c.Users)
                .Where(u => u.Roles.Any(r => r.RoleName == "Student"))
                .Distinct()
                .Count();

            // إجمالي عدد الامتحانات
            var totalExams = courses.Sum(c => c.Exams.Count);

            // إجمالي عدد الامتحانات المنشورة
            var totalPublishedExams = courses.Sum(c => c.PublishedExams.Count);

            return new InstructorProfileDto
            {
                UserID = user.UserID,
                FullName = user.FullName,
                Email = user.Email,
                RegistrationDate = user.RegistrationDate,
                ProfileImagePath = user.ProfileImagePath,

                Branch = user.IntakeBranchTrackUsers
                    .Where(ibt => ibt.Branch != null)
                    .Select(ibt => ibt.Branch)
                    .Distinct().ToList(),

                Track = user.IntakeBranchTrackUsers
                    .Where(ib => ib.Track != null)
                    .Select(ib => ib.Track)
                    .Distinct().ToList(),

                Intake = user.IntakeBranchTrackUsers
                    .Where(ibt => ibt.Intake != null)
                    .Select(ibt => ibt.Intake)
                    .Distinct().ToList(),

                CourseCount = courses.Count,
                StudentCount = allStudentsCount,
                ExamCount = totalExams,
                PublishedExamCount = totalPublishedExams
            };
        }


        // جلب الكورسات الخاصة بالـ Instructor
        public async Task<List<InstructorCourseDto>> GetInstructorCoursesAsync(int userId)
        {
            var courses = await _context.Courses
                .Where(c => c.Users.Any(u => u.UserID == userId && u.Roles.Any(r => r.RoleName == "Instructor")))
                .Include(c => c.Users)
                    .ThenInclude(u => u.Roles)
                .Include(c => c.Exams)
                .Include(c => c.PublishedExams)
                .Include(c => c.Topics)
                .ToListAsync();

            var result = courses.Select(c => new InstructorCourseDto
            {
                CourseID = c.CourseID,
                Name = c.Name,
                Duration = c.Duration,
                CourseImagePath = c.CourseImagePath,
                StudentCount = c.Users.Count(u => u.Roles.Any(r => r.RoleName == "Student")),
                ExamCount = c.Exams.Count,
                PublishedExamCount = c.PublishedExams.Count,
                TopicCount = c.Topics.Count
            }).ToList();

            return result;
        }


        public void update(int userId,string img)
        {
            var i=_context.Users.Find(userId);
            i.ProfileImagePath=img;
            _context.Entry(i).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            _context.SaveChanges();
        }

        public async Task<List<Topic>> GetInstructorCourseTopicsAsync(int instructorId, int courseId)
        {
            var course = await _context.Courses
                .Where(c => c.CourseID == courseId && c.Users.Any(u => u.UserID == instructorId && u.Roles.Any(r => r.RoleName == "Instructor")))
                .Include(c => c.Topics)
                .FirstOrDefaultAsync();

            return course?.Topics.ToList();
        }

        public async Task<List<User>> GetCourseStudentsWithExamDetailsAsync(int instructorId, int courseId)
        {
            var course = await _context.Courses
                .Where(c => c.CourseID == courseId && c.Users.Any(u => u.UserID == instructorId && u.Roles.Any(r => r.RoleName == "Instructor")))
                .Include(c => c.Users)
                    .ThenInclude(u => u.Roles)
                .FirstOrDefaultAsync();

            return course?.Users
                .Where(u => u.Roles.Any(r => r.RoleName == "Student"))
                .ToList() ?? new List<User>();
        }




        public async Task<List<Exam>> GetInstructorCourseExamsAsync(int instructorId, int courseId)
        {
            var isInstructorAssigned = await _context.Courses
                .AnyAsync(c => c.CourseID == courseId &&
                               c.Users.Any(u => u.UserID == instructorId &&
                                                u.Roles.Any(r => r.RoleName == "Instructor")));

            if (!isInstructorAssigned)
                return new List<Exam>();

            var exams = await _context.Exams
                .Where(e => e.CourseID == courseId)
                .ToListAsync();

            return exams;
        }


        public async Task<List<PublishedExam>> GetInstructorPublishedExamsAsync(int instructorId, int courseId)
        {
            var isInstructorAssigned = await _context.Courses
                .AnyAsync(c => c.CourseID == courseId &&
                               c.Users.Any(u => u.UserID == instructorId &&
                                                u.Roles.Any(r => r.RoleName == "Instructor")));

            if (!isInstructorAssigned)
                return new List<PublishedExam>();

            var publishedExams = await _context.PublishedExams
                .Include(pe => pe.Exam)
                .Where(pe => pe.CourseID == courseId)
                .Include(pe=>pe.Intake)
                .Include(pe=>pe.Track)
                .Include(pe=>pe.Branch)
                .ToListAsync();

            return publishedExams;
        }

        public async Task<List<User>> GetInstructorStudentsAsync(int instructorId)
        {
            var students = await _context.Courses
                .Where(c => c.Users.Any(u => u.UserID == instructorId && u.Roles.Any(r => r.RoleName == "Instructor")))
                .SelectMany(c => c.Users)
                .Where(u => u.Roles.Any(r => r.RoleName == "Student"))
                .Distinct()
                .ToListAsync();

            return students;
        }

        public async Task<List<Exam>> GetInstructorCreatedExamsAsync(int instructorId)
        {
            var exams = await _context.Courses
                .Where(c => c.Users.Any(u => u.UserID == instructorId && u.Roles.Any(r => r.RoleName == "Instructor")))
                .SelectMany(c => c.Exams)
                .ToListAsync();

            return exams;
        }

        public async Task<List<PublishedExam>> GetInstructorPublishedExamsAsync(int instructorId)
        {
            var publishedExams = await _context.Courses
                .Where(c => c.Users.Any(u => u.UserID == instructorId && u.Roles.Any(r => r.RoleName == "Instructor")))
                .SelectMany(c => c.PublishedExams)
                .Include(pe => pe.Exam)
                .Include(pe=>pe.Branch)
                .Include(pe=>pe.Intake)
                .Include (pe=>pe.Track)
                .ToListAsync();

            return publishedExams;
        }



    }
}
