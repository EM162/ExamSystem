using ITI.ExamSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

namespace ITI.ExamSystem.Controllers
{
    public class ExamController : Controller
    {
        private readonly OnlineExaminationDBContext db;

        public ExamController(OnlineExaminationDBContext _db)
        {
            db = _db;
        }

        public IActionResult Start(int examId)
        {
            int currentUserId = 1; // Replace with actual login logic

            var userExam = db.UserExams
                .FirstOrDefault(ue => ue.ExamID == examId && ue.UserID == currentUserId && ue.Grade != null);

            if (userExam != null)
            {
                TempData["Message"] = "You have already completed this exam.";
                return RedirectToAction("Summary", new { examId });
            }
            return RedirectToAction("Question", new { examId = examId, index = 0 });
        }


        public IActionResult Question(int examId, int index)
        {
            int currentUserId = 1;

            var userExam = db.UserExams
                .FirstOrDefault(ue => ue.ExamID == examId && ue.UserID == currentUserId && ue.Grade != null && ue.Grade != 0);

            if (userExam != null)
            {
                TempData["Message"] = "You have already completed this exam.";
                return RedirectToAction("Summary", new { examId });
            }

            var exam = db.Exams
                .Include(e => e.Questions)
                    .ThenInclude(q => q.QuestionChoices)
                .Include(e => e.Course)
                .FirstOrDefault(e => e.ExamID == examId);

            if (exam == null) return NotFound();

            var orderedQuestions = exam.Questions
                .OrderBy(q => q.QuestionID)
                .ToList();

            if (index >= orderedQuestions.Count)
                return RedirectToAction("Summary", new { examId });

            var question = orderedQuestions[index];

            var savedAnswer = db.UsersExamsQuestions
                .FirstOrDefault(x =>
                    x.ExamID == examId &&
                    x.UserID == currentUserId &&
                    x.QuestionID == question.QuestionID);

            int? selectedChoiceId = null;

            if (savedAnswer != null && int.TryParse(savedAnswer.StudentAnswer, out var parsed))
            {
                var selectedChoice = db.QuestionChoices
                    .FirstOrDefault(qc => qc.QuestionID == question.QuestionID && qc.ChoiceOrder == parsed);
                selectedChoiceId = selectedChoice?.ChoiceID;
            }

            var model = new QuestionViewModel
            {
                ExamID = examId,
                ExamTitle = exam.Course?.Name ?? "Exam",
                QuestionID = question.QuestionID,
                QuestionText = question.QuestionText,
                Choices = question.QuestionChoices.Select(c => new ChoiceViewModel
                {
                    ChoiceID = c.ChoiceID,
                    ChoiceText = c.ChoiceText
                }).ToList(),
                QuestionIndex = index,
                IsLastQuestion = index == orderedQuestions.Count - 1,
                Progress = (int)((index + 1) * 100.0 / orderedQuestions.Count),
                TimeRemaining = exam.Duration,
                QuestionDegree = question.Grade,
                SelectedChoiceID = selectedChoiceId,
                AllQuestions = orderedQuestions.Select((q, i) => (i, q.QuestionText)).ToList()
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult SubmitAnswer(QuestionViewModel model)
        {
            int currentUserId = 1;

            var choice = db.QuestionChoices
                .FirstOrDefault(c => c.ChoiceID == model.SelectedChoiceID);

            var existing = db.UsersExamsQuestions
                .FirstOrDefault(x => x.ExamID == model.ExamID &&
                                     x.UserID == currentUserId &&
                                     x.QuestionID == model.QuestionID);

            if (existing != null)
            {
                existing.StudentAnswer = choice?.ChoiceOrder.ToString();
            }
            else
            {
                db.UsersExamsQuestions.Add(new UsersExamsQuestion
                {
                    ExamID = model.ExamID,
                    UserID = currentUserId,
                    QuestionID = model.QuestionID,
                    StudentAnswer = choice?.ChoiceOrder.ToString()
                });
            }

            db.SaveChanges();

            var userExam = db.UserExams
                .FirstOrDefault(ue => ue.ExamID == model.ExamID && ue.UserID == currentUserId);

            if (userExam == null)
            {
                db.UserExams.Add(new UserExam
                {
                    ExamID = model.ExamID,
                    UserID = currentUserId,
                    Grade = 0
                });

                db.SaveChanges();
            }

            if (model.IsLastQuestion)
            {
                try
                {
                    db.Database.ExecuteSqlRaw("EXEC CorrectExam @p0, @p1;", model.ExamID, currentUserId);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("SP Error: " + ex.Message);
                }

                return RedirectToAction("Summary", new { examId = model.ExamID });
            }

            return RedirectToAction("Question", new { examId = model.ExamID, index = model.QuestionIndex + 1 });
        }

        public IActionResult Summary(int examId)
        {
            int currentUserId = 1;

            var exam = db.Exams
                .Include(e => e.Course)
                .Include(e => e.Questions)
                .AsNoTracking()
                .FirstOrDefault(e => e.ExamID == examId);

            var userExam = db.UserExams
                .AsNoTracking()
                .FirstOrDefault(ue => ue.ExamID == examId && ue.UserID == currentUserId);

            if (exam == null || userExam == null)
                return NotFound();

            var model = new ExamSummaryViewModel
            {
                ExamTitle = exam.Course?.Name ?? "Exam",
                TotalScore = userExam.Grade,
                TotalQuestions = exam.Questions.Count
            };

            return View(model);
        }




    }

}
