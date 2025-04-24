using ITI.ExamSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ITI.ExamSystem.Controllers
{
    public class Student_ExamsController : Controller
    {
        private readonly OnlineExaminationDBContext _db;
        public Student_ExamsController(OnlineExaminationDBContext db)
        {
            _db = db;
        }

        public IActionResult Exams()
        {
            int studentId = int.Parse(User.FindFirst("UserID").Value);
            var now = DateTime.Now;

            // Get tracks/intakes of the student
            var userTracks = _db.IntakeBranchTrackUsers
                .Where(x => x.UserID == studentId)
                .Select(x => new { x.TrackID, x.IntakeID, x.BranchID })
                .ToList();

            // Upcoming exams filtered by track/intake/branch match
            var upcoming = _db.PublishedExams
                .Include(p => p.Exam)
                .Include(p => p.Course)
                .Include(p => p.Track)
                .Include(p => p.Intake)
                .Include(p => p.Branch)
                .Where(p => p.PublishDate > now)
                .AsEnumerable()
                .Where(p => userTracks.Any(t => t.TrackID == p.TrackID && t.IntakeID == p.IntakeID && t.BranchID == p.BranchID))
                .OrderBy(p => p.PublishDate)
                .ToList();

            // Previous exams taken by the student
            var previous = _db.UserExams
                .Include(ue => ue.Exam)
                .Where(ue => ue.UserID == studentId && ue.Exam.ExamDate <= now)
                .ToList();

            var model = new StudentExamsViewModel
            {
                UpcomingExams = upcoming,
                PreviousExams = previous
            };

            return View(model);
        }
    }
}