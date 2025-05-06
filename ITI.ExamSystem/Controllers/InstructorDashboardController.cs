using System.Drawing;
using ITI.ExamSystem.DTOs.InstructorsDTO;
using ITI.ExamSystem.Repository;
using Microsoft.AspNetCore.Mvc;
using static System.Net.Mime.MediaTypeNames;
using X.PagedList;
using X.PagedList.Extensions;
using NuGet.Protocol.Core.Types;
using Microsoft.AspNetCore.Mvc.Rendering;
using ITI.ExamSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace ITI.ExamSystem.Controllers
{
    public class InstructorDashboardController : Controller
    {
        private readonly IInstructorRepository _instructorRepository;
        int userId = 8;//User.Identity.GetUserId(); // استخدم الـ UserID الخاص بالمستخدم الحالي
        private readonly OnlineExaminationDBContext _context;

        public InstructorDashboardController(IInstructorRepository instructorRepository, OnlineExaminationDBContext context)
        {
            _instructorRepository = instructorRepository;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View();
        }
        // Action لعرض بيانات الـ Instructor
        public async Task<IActionResult> InstructorProfile()
        {
            var instructorProfile = await _instructorRepository.GetInstructorProfileAsync(userId);

            if (instructorProfile == null)
            {
                return NotFound();
            }

            return View(instructorProfile);
        }

        [HttpPost]
        public IActionResult InstructorProfile(IFormFile img)
        {
            if (img != null)
            {
                using var stream = new FileStream($"wwwroot/images/{img.FileName}", FileMode.Create);
                img.CopyTo(stream);

                _instructorRepository.update(userId,img.FileName);
            }
            return RedirectToAction("InstructorProfile");
        }

        public async Task<IActionResult> InstructorCourses()
        {
            var instructorCourses = await _instructorRepository.GetInstructorCoursesAsync(userId);

            if (instructorCourses == null)
            {
                return NotFound();
            }

            return View(instructorCourses);
        }

        public async Task<IActionResult> GetCourseTopics(int Id)
        {
            var topics = await _instructorRepository.GetInstructorCourseTopicsAsync(userId, Id);
            if (topics == null)
            {
                return NotFound();
            }
            return View(topics);
        }

        public async Task<IActionResult> GetCourseStudents(int Id, int? page)
        {
            var students = await _instructorRepository.GetCourseStudentsWithExamDetailsAsync(userId, Id);
            int pageSize = 10;
            int pageNumber = page ?? 1;
            var pagedStudents = students.ToPagedList(pageNumber, pageSize);
            return View(pagedStudents);
        }

        public async Task<IActionResult> GetCourseExams(int Id)
        {
            var exams = await _instructorRepository.GetInstructorCourseExamsAsync(userId,Id);
            return View(exams);
        }

        public async Task<IActionResult> GetCoursePublishedExams(int Id)
        {
            var published = await _instructorRepository.GetInstructorPublishedExamsAsync(userId, Id);
            return View(published);
        }

        public async Task<IActionResult> GetInstructorStudents(int? page)
        {
            var students = await _instructorRepository.GetInstructorStudentsAsync(userId);

            if (students == null || !students.Any())
                return NotFound("No students found.");

            int pageSize = 10;
            int pageNumber = page ?? 1;
            var pagedStudents = students.ToPagedList(pageNumber, pageSize);

            return View(pagedStudents);
        }

       
        public async Task<IActionResult> GetInstructorCreatedExams()
        {
            var exams = await _instructorRepository.GetInstructorCreatedExamsAsync(userId);

            if (exams == null || !exams.Any())
                return NotFound("No exams found.");

            return View(exams);
        }

        [HttpGet]
        public async Task<IActionResult> CreateExam()
        {
            // Get the list of courses for this instructor
            var instructorCourses = await _instructorRepository.GetInstructorCoursesAsync(userId);

            ViewBag.Courses = instructorCourses.Select(c => new SelectListItem
            {
                Value = c.CourseID.ToString(),
                Text = c.Name
            }).ToList();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateExam(Exam exam)
        {
            if (!ModelState.IsValid)
            {
                // repopulate courses if invalid
                var instructorCourses = await _instructorRepository.GetInstructorCoursesAsync(userId);
                ViewBag.Courses = instructorCourses.Select(c => new SelectListItem
                {
                    Value = c.CourseID.ToString(),
                    Text = c.Name
                }).ToList();
                return View(exam);
            }

            _context.Exams.Add(exam);
            await _context.SaveChangesAsync();

            return RedirectToAction("AddQuestions", new { examId = exam.ExamID });

        }


        [HttpGet]
        public async Task<IActionResult> AddQuestions(int examId)
        {
            var exam = await _context.Exams.Include(e => e.Course).FirstOrDefaultAsync(e => e.ExamID == examId);
            if (exam == null)
            {
                return NotFound("Exam not found.");
            }
            var topics = await _context.Topics
                .Where(t => t.CourseID == exam.CourseID)
                .ToListAsync();

            ViewBag.Topics = topics.Select(t => new SelectListItem
            {
                Value = t.TopicID.ToString(),
                Text = t.TopicName
            }).ToList();


            ViewBag.ExamTitle = exam.Course?.Name + " - " + exam.ExamType;
            ViewBag.ExamId = exam.ExamID;

            return View(new Question());  // Empty model to bind the form
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddQuestions(Question question, int examId, List<string> Choices, int CorrectChoice)
        {
            if (!ModelState.IsValid)
            {
                var exam = await _context.Exams.Include(e => e.Course).FirstOrDefaultAsync(e => e.ExamID == examId);
                ViewBag.ExamTitle = exam?.Course?.Name + " - " + exam?.ExamType;
                ViewBag.ExamId = examId;
                return View(question);
            }

            //question.TopicID = 0;
            _context.Questions.Add(question);
            await  _context.SaveChangesAsync();

            // Link question to exam
            _context.Exams.First(e => e.ExamID == examId).Questions.Add(question);
            await _context.SaveChangesAsync();

            // Add choices if MCQ
            if (Choices != null && Choices.Any())
            {
                for (int i = 0; i < Choices.Count; i++)
                {
                    if (!string.IsNullOrWhiteSpace(Choices[i]))
                    {
                        var choice = new QuestionChoice
                        {
                            QuestionID = question.QuestionID,
                            ChoiceText = Choices[i],
                            ChoiceOrder = i + 1,
                            CorrectChoice = (i + 1 == CorrectChoice) ? 1 : 0
                        };
                        _context.QuestionChoices.Add(choice);
                    }
                }
                await _context.SaveChangesAsync();
            }

            TempData["Success"] = "Question added successfully.";

            return RedirectToAction("AddQuestions", new { examId = examId });
        }

        [HttpGet]
        public async Task<IActionResult> PublishExam(int examId)
        {
            var exam = await _context.Exams
                .Include(e => e.Course)
                .FirstOrDefaultAsync(e => e.ExamID == examId);

            if (exam == null)
            {
                return NotFound("Exam not found.");
            }

            // Get all branches/tracks/intakes to let instructor pick where to publish
            ViewBag.Branches = new SelectList(_context.Branches, "BranchID", "BranchName");
            ViewBag.Tracks = new SelectList(_context.Tracks, "TrackID", "TrackName");
            ViewBag.Intakes = new SelectList(_context.Intakes, "IntakeID", "IntakeName");

            ViewBag.ExamTitle = exam.Course?.Name + " - " + exam.ExamType;

            return View(new PublishedExam
            {
                ExamID = examId,
                CourseID = exam.CourseID,
                PublishDate = DateTime.Now
            });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PublishExam(PublishedExam publishedExam)
        {
            if (!ModelState.IsValid)
            {
                // Repopulate dropdowns
                ViewBag.Branches = new SelectList(_context.Branches, "BranchID", "BranchName");
                ViewBag.Tracks = new SelectList(_context.Tracks, "TrackID", "TrackName");
                ViewBag.Intakes = new SelectList(_context.Intakes, "IntakeID", "IntakeName");

                return View(publishedExam);
            }
            var exists = await _context.PublishedExams.AnyAsync(p =>
                p.ExamID == publishedExam.ExamID &&
                p.BranchID == publishedExam.BranchID &&
                p.TrackID == publishedExam.TrackID &&
                p.IntakeID == publishedExam.IntakeID);

            if (exists)
            {
                ModelState.AddModelError("", "This exam is already published to the selected Branch/Track/Intake.");
                ViewBag.Branches = new SelectList(_context.Branches, "BranchID", "BranchName");
                ViewBag.Tracks = new SelectList(_context.Tracks, "TrackID", "TrackName");
                ViewBag.Intakes = new SelectList(_context.Intakes, "IntakeID", "IntakeName");
                return View(publishedExam);
            }

            publishedExam.PublishDate = DateTime.Now;

            _context.PublishedExams.Add(publishedExam);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Exam published successfully!";
            return RedirectToAction("GetInstructorCreatedExams");
        }

        public async Task<IActionResult> ViewExamQuestions(int examId)
        {
            var exam = await _context.Exams
                .Include(e => e.Course)
                .Include(e => e.Questions)
                    .ThenInclude(q => q.QuestionChoices)
                .FirstOrDefaultAsync(e => e.ExamID == examId);

            if (exam == null)
            {
                return NotFound("Exam not found.");
            }

            ViewBag.ExamTitle = exam.Course?.Name + " - " + exam.ExamType;

            return View(exam);
        }



        public async Task<IActionResult> GetInstructorPublishedExams()
        {
            var publishedExams = await _instructorRepository.GetInstructorPublishedExamsAsync(userId);

            if (publishedExams == null || !publishedExams.Any())
                return NotFound("No published exams found.");

            return View(publishedExams);
        }
    }
}
