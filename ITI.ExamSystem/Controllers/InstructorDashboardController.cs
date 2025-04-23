using ITI.ExamSystem.Repository;
using Microsoft.AspNetCore.Mvc;

namespace ITI.ExamSystem.Controllers
{
    public class InstructorDashboardController : Controller
    {
        private readonly IInstructorRepository _instructorRepository;

        public InstructorDashboardController(IInstructorRepository instructorRepository)
        {
            _instructorRepository = instructorRepository;
        }

        // Action لعرض بيانات الـ Instructor
        public async Task<IActionResult> Index()
        {
            var userId = 2;//User.Identity.GetUserId(); // استخدم الـ UserID الخاص بالمستخدم الحالي
            var instructorProfile = await _instructorRepository.GetInstructorProfileAsync(userId);

            if (instructorProfile == null)
            {
                return NotFound();
            }

            return View(instructorProfile);
        }
    }
}
