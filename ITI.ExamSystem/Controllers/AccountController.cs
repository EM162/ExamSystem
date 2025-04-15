using Microsoft.AspNetCore.Mvc;

namespace ITI.ExamSystem.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        public IActionResult AdminLogin()
        {
            return View();
        }
    }
}
