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
                .Where(u => u.UserID == userId)
                .Include(u => u.Courses)
                .Include(u => u.IntakeBranchTrackUsers)
                .ThenInclude(ibt => ibt.Branch)
                .Include(u => u.IntakeBranchTrackUsers)
                .ThenInclude(ibt => ibt.Intake)
                .Include(u => u.IntakeBranchTrackUsers)
                .ThenInclude(ibt => ibt.Track)
                .FirstOrDefaultAsync();

            if (user == null) return null;

            return new InstructorProfileDto
            {
                FullName = user.FullName,
                Email = user.Email,
                ProfileImagePath = user.ProfileImagePath,
                Branch =string.Join(", ", user.IntakeBranchTrackUsers
                      .Where(ibt => ibt.Branch != null)
                      .Select(ibt => ibt.Branch.BranchName)
                      .Distinct()),
                Track = string.Join(", ", user.IntakeBranchTrackUsers
                      .Where(ib => ib.Track != null)
                      .Select(ib => ib.Track.TrackName)
                      .Distinct()),
                Intake = string.Join(", ", user.IntakeBranchTrackUsers
                      .Where(ibt => ibt.Intake != null)
                      .Select(ibt => ibt.Intake.IntakeName)
                      .Distinct()),
                CourseCount = user.Courses.Count,
                StudentCount = user.Courses.FirstOrDefault().Users.Count,
                Courses= user?.Courses.ToList()
            };

        }

        // جلب الكورسات الخاصة بالـ Instructor
        public async Task<List<Course>> GetInstructorCoursesAsync(int userId)
        {
            var user = await _context.Users
                .Where(u => u.UserID == userId)
                .Include(u => u.Courses)
                .FirstOrDefaultAsync();

            return user?.Courses.ToList();
        }
    }
}
