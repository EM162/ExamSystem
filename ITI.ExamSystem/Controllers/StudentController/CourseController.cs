using AutoMapper;
using ITI.ExamSystem.DTOs.StudentDTO;
using ITI.ExamSystem.Models;
using ITI.ExamSystem.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;

namespace ITI.ExamSystem.Controllers.StudentController
{

    [Route("student/course")]
    public class CourseController : Controller
    {
        private IStudentRepositary stdRepositary;
        private readonly IMapper _map;
        private readonly OnlineExaminationDBContext db;
        private readonly IWebHostEnvironment _env;

        public CourseController( IStudentRepositary stdRepositary , IMapper _map, IWebHostEnvironment env,OnlineExaminationDBContext db ) {
        this.stdRepositary = stdRepositary ;
        this ._map = _map ;
        this.db = db ;
            this._env = env ;    
        }

        // GET: /student/course/{studentID}
        [HttpGet("{studentID:guid}")]
        public async Task< IActionResult> StudentCourse(Guid studentID)
        
        {
            if (studentID == null) return BadRequest();

            var courses =  await stdRepositary.GetCoursesByStudentId(studentID);
            var courseDto = _map.Map<List <StudentCoursesDTO>>(courses);
            Console.WriteLine("Number of courses: " + courses.Count());
            foreach (var c in courseDto)
            {
                Console.WriteLine("ImagePath: " + c.CourseImagePath);
            }

            return View("~/Views/Student/StudentCourse.cshtml", courseDto);
        }

        // GET: /student/course/{courseID}/topics
        [HttpGet("{courseID:guid}/topics")]
        public async Task<IActionResult> MyTopics (int courseID)
        {
            if(courseID == null) return BadRequest();
            var topics = await stdRepositary.GetTopicsByCourseId(courseID);
            var topicsDto = _map.Map<List <StudentTopicsDTO>>(topics);  
            return View("~/Views/Student/StudentTopics.cshtml", topicsDto);

        }

        // GET: /student/course/image/create
        [HttpGet("image/create")]
        public IActionResult CreateImage()
        {
            var courses = db.Courses.Select(c => new { c.CourseID, c.Name }).ToList();
            ViewBag.Courses = new SelectList(courses, "CourseID", "Name");
            return View();
        }

        // POST: /student/course/image/create
        [HttpPost("image/create")]
        public async Task<IActionResult> CreateImage(IFormFile imageFile, int CourseID)
        {
            if (imageFile != null && imageFile.Length > 0)
            {
                var extension = Path.GetExtension(imageFile.FileName).ToLower();
                if (!new[] { ".jpg", ".jpeg", ".png", ".gif" }.Contains(extension))
                {
                    TempData["Error"] = "Invalid image format. Please upload JPG, PNG, or GIF.";
                    return RedirectToAction(" ",new { id = CourseID });
                }

                var course = stdRepositary.GetCourseById (CourseID);
                if (course == null)
                {
                    TempData["Error"] = "Course not found.";
                    return RedirectToAction(" ", new { id = CourseID });
                }
                var fileName = Guid.NewGuid().ToString() + extension;
                var imagePath = Path.Combine("wwwroot/images", fileName);
                using (var stream = new FileStream(imagePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }
                course.CourseImagePath = "/images/" + fileName;

                db.SaveChanges();
            }
            return RedirectToAction ("createImage" , new {id = CourseID});
        }

        [HttpGet("index")]
        public IActionResult Index()
        {
            return View();
        }


    }
}
