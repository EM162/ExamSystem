using Microsoft.AspNetCore.Mvc;

namespace ITI.ExamSystem.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Dashboard()
        {
            return View();
        }
    }
}
