using AutoMapper;
using ITI.ExamSystem.DTOs.StudentDTO;
using ITI.ExamSystem.Models;
using ITI.ExamSystem.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ITI.ExamSystem.Controllers.StudentController
{
    [ApiController]
    [Route("api/[controller]")]
    public class StudentProfileController : Controller
    {
        private IStudentRepositary userRepositary;
        private IMapper _map;
        private readonly OnlineExaminationDBContext db;

        public StudentProfileController(IStudentRepositary _userRepositary ,IMapper _map,OnlineExaminationDBContext _db ) {
        
        this.userRepositary = _userRepositary;
         this._map= _map;
            this.db = _db;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("{id}")]
        public IActionResult Profile(Guid id)
        {
            var _student = userRepositary.GetStudentProfile(id);
            if (_student == null)
            {
                return NotFound();

            }
            var dto = _map.Map<StudentProfileDTO>(_student);
          
            return View("~/Views/Student/studentProfile.cshtml", dto);
        }

        [HttpPost]
        public async Task<IActionResult> UploadProfileImage(IFormFile imageFile, Guid studentId)
        {
            if (imageFile != null && imageFile.Length > 0)
            {
                var extension = Path.GetExtension(imageFile.FileName).ToLower();
                if (!new[] { ".jpg", ".jpeg", ".png", ".gif" }.Contains(extension))
                {
                    // You can show a message or redirect with an error
                    TempData["Error"] = "Invalid image format. Please upload JPG, PNG, or GIF.";
                    return RedirectToAction("Profile", new { id = studentId });
                }

                var student = userRepositary.GetStudentProfile(studentId);

                // Unique filename
                var fileName = Guid.NewGuid().ToString() + extension;
                var imagePath = Path.Combine("wwwroot/images", fileName);

                // Save image to wwwroot/images
                using (var stream = new FileStream(imagePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }

                // Save image path to DB
                student.ProfileImagePath = "/images/" + fileName;
                db.SaveChanges();
            }
            return RedirectToAction("Profile", new { id = studentId });
        }
    }
}
