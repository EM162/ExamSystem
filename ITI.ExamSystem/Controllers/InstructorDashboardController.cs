using System.Drawing;
using ITI.ExamSystem.DTOs.InstructorsDTO;
using ITI.ExamSystem.Repository;
using Microsoft.AspNetCore.Mvc;
using static System.Net.Mime.MediaTypeNames;
using X.PagedList;
using X.PagedList.Extensions;
using NuGet.Protocol.Core.Types;

namespace ITI.ExamSystem.Controllers
{
    public class InstructorDashboardController : Controller
    {
        private readonly IInstructorRepository _instructorRepository;
        int userId = 8;//User.Identity.GetUserId(); // استخدم الـ UserID الخاص بالمستخدم الحالي

        public InstructorDashboardController(IInstructorRepository instructorRepository)
        {
            _instructorRepository = instructorRepository;
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

        
        public async Task<IActionResult> GetInstructorPublishedExams()
        {
            var publishedExams = await _instructorRepository.GetInstructorPublishedExamsAsync(userId);

            if (publishedExams == null || !publishedExams.Any())
                return NotFound("No published exams found.");

            return View(publishedExams);
        }
    }
}
