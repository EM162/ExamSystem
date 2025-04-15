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
            // Redirect to first question
            return RedirectToAction("Question", new { examId = examId, index = 0 });
        }

        //public IActionResult Question(int examId, int index)
        //{
        //    var exam = db.Exams
        //        .Include(e => e.Questions)
        //            .ThenInclude(q => q.QuestionChoices)
        //        .FirstOrDefault(e => e.ExamID == examId);

        //    if (exam == null) return NotFound();

        //    var orderedQuestions = exam.Questions
        //        .OrderBy(q => q.QuestionID) // ensure consistent ordering
        //        .ToList();

        //    if (index >= orderedQuestions.Count)
        //        return RedirectToAction("Summary", new { examId });

        //    var q = orderedQuestions[index];

        //    var model = new QuestionViewModel
        //    {
        //        ExamID = examId,
        //        ExamTitle = "Exam Title", // optionally load dynamically
        //        QuestionID = q.QuestionID,
        //        QuestionText = q.QuestionText,
        //        Choices = q.QuestionChoices.Select(c => new ChoiceViewModel
        //        {
        //            ChoiceID = c.ChoiceID,
        //            ChoiceText = c.ChoiceText
        //        }).ToList(),
        //        QuestionIndex = index,
        //        IsLastQuestion = index == orderedQuestions.Count - 1,
        //        Progress = (int)((index + 1) * 100.0 / orderedQuestions.Count),
        //        TimeRemainingSeconds = 5 * 60
        //    };

        //    return View(model);
        //}

        public IActionResult Question(int examId, int index)
        {
            var exam = db.Exams
                .Include(e => e.Questions)
                    .ThenInclude(q => q.QuestionChoices)
                .FirstOrDefault(e => e.ExamID == examId);

            if (exam == null) return NotFound();

            var orderedQuestions = exam.Questions.OrderBy(q => q.QuestionID).ToList();
            if (index >= orderedQuestions.Count) return RedirectToAction("Summary", new { examId });

            var question = orderedQuestions[index];
            var model = new QuestionViewModel
            {
                ExamID = examId,
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
                TimeRemaining = exam.Duration
            };

            return View(model);
        }



        [HttpPost]
        public IActionResult SubmitAnswer(QuestionViewModel model)
        {
            var existing = db.UsersExamsQuestions
                .FirstOrDefault(x => x.ExamID == model.ExamID &&
                                     x.UserID == 1 && // TODO: get real user ID
                                     x.QuestionID == model.QuestionID);

            if (existing != null)
            {
                existing.StudentAnswer = model.SelectedChoiceID.ToString();
            }
            else
            {
                var answer = new UsersExamsQuestion
                {
                    ExamID = model.ExamID,
                    UserID = 1,
                    QuestionID = model.QuestionID,
                    StudentAnswer = model.SelectedChoiceID.ToString()
                };
                db.UsersExamsQuestions.Add(answer);
            }

            db.SaveChanges();

            return RedirectToAction("Question", new { examId = model.ExamID, index = model.QuestionIndex + 1 });
        }


        public IActionResult Summary(int examId)
        {
            // Validate examId
            var exam = db.Exams
                .Include(e => e.Questions)
                .FirstOrDefault(e => e.ExamID == examId);

            if (exam == null)
            {
                return NotFound();
            }

            return View(exam); // or a ViewModel
        }
    }

}
