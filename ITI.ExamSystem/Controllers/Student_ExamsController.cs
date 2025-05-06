using ITI.ExamSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ITI.ExamSystem.Controllers
{
    public class Student_ExamsController : Controller
    {
        private readonly OnlineExaminationDBContext _db;
        public Student_ExamsController(OnlineExaminationDBContext db)
        {
            _db = db;
        }
        private int GetCurrentUserId()
        {
            var identityId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(identityId))
                throw new Exception("User not authenticated");

            var user = _db.Users.FirstOrDefault(u => u.IdentityUserId == identityId);
            if (user == null)
                throw new Exception("User not found in system");

            return user.UserID;
        }


        public IActionResult Exams()
        {
            int studentId = GetCurrentUserId();
            var now = DateTime.Now;

            // Get tracks/intakes of the student
            var userTracks = _db.IntakeBranchTrackUsers
                .Where(x => x.UserID == studentId)
                .Select(x => new { x.TrackID, x.IntakeID, x.BranchID })
                .ToList();

            var trackIds = userTracks.Select(t => t.TrackID).ToList();
            var intakeIds = userTracks.Select(t => t.IntakeID).ToList();
            var branchIds = userTracks.Select(t => t.BranchID).ToList();

            var upcoming = _db.PublishedExams
                .Include(p => p.Exam)
                .Include(p => p.Course)
                .Include(p => p.Track)
                .Include(p => p.Intake)
                .Include(p => p.Branch)
                .Where(p =>
                    p.PublishDate >= now &&
                    trackIds.Contains(p.TrackID) &&
                    intakeIds.Contains(p.IntakeID) &&
                    branchIds.Contains(p.BranchID)
                )
                .OrderBy(p => p.PublishDate)
                .ToList();


            // Previous exams taken by the student
            var previous = _db.UserExams
                .Include(ue => ue.Exam)
                .Where(ue => ue.UserID == studentId && ue.Exam.ExamDate < now)
                .ToList();

            var finishedExamIds = _db.UserExams
                .Where(ue => ue.UserID == studentId)
                .Select(ue => ue.ExamID)
                .ToHashSet();

            var model = new StudentExamsViewModel
            {
                UpcomingExams = upcoming,
                PreviousExams = previous,
                FinishedExamIds = finishedExamIds
            };

            return View(model);
        }


        public IActionResult ExamDetails(int examId)
        {
            int studentId = GetCurrentUserId();

            var exam = _db.Exams
                .Include(e => e.Course)
                .FirstOrDefault(e => e.ExamID == examId);

            var userExam = _db.UserExams
                .FirstOrDefault(ue => ue.ExamID == examId && ue.UserID == studentId);

            var questionAnswers = _db.UsersExamsQuestions
                .Include(q => q.Question)
                .ThenInclude(q => q.QuestionChoices)
                .Where(ueq => ueq.ExamID == examId && ueq.UserID == studentId)
                .ToList();

            var model = new ExamDetailsViewModel
            {
                ExamTitle = exam?.Course?.Name,
                ExamDate = exam?.ExamDate ?? DateTime.MinValue,
                Grade = userExam?.Grade ?? 0,
                Questions = questionAnswers.Select(q => new QuestionDetail
                {
                    QuestionText = q.Question.QuestionText,
                    SelectedAnswer = q.Question.QuestionChoices
                        .FirstOrDefault(c => c.ChoiceOrder.ToString() == q.StudentAnswer)?.ChoiceText,
                    CorrectAnswer = q.Question.QuestionChoices
                        .FirstOrDefault(c => c.CorrectChoice == 1)?.ChoiceText,
                    Score = q.StudentScore ?? 0
                }).ToList()

            };

            return View(model);
        }

    }
}