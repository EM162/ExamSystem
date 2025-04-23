using AutoMapper;
using ITI.ExamSystem.DTOs.StudentDTO;
using ITI.ExamSystem.Models;
using ITI.ExamSystem.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ITI.ExamSystem.Controllers.StudentController
{

    
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

        [HttpGet("student/course/{studentID}")]

        public async Task< IActionResult> StudentCourse(int studentID)
        
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


        [HttpGet("student/topics/{courseID}")]
        public async Task<IActionResult> MyTopics (int courseID)
        {
            if(courseID == null) return BadRequest();
            var topics = await stdRepositary.GetTopicsByCourseId(courseID);
            var topicsDto = _map.Map<List <StudentTopicsDTO>>(topics);  
            return View("~/Views/Student/StudentTopics.cshtml", topicsDto);

        }


        // upload images 


        [HttpGet("/course/createImage")]
        public IActionResult CreateImage()
        {
            var courses = db.Courses.Select(c => new { c.CourseID, c.Name }).ToList();
            ViewBag.Courses = new SelectList(courses, "CourseID", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
      
        public async Task<IActionResult> CreateImage(int CourseID, IFormFile ImageFile)
        {
            var course = await db.Courses.FindAsync(CourseID);
            if (course == null)
            {
       
                ViewBag.Courses = new SelectList(db.Courses.ToList(), "Id", "Name");
                return View();
            }
            if (ModelState.IsValid)
            {
                if (ImageFile != null && ImageFile.Length > 0)
                {
                    string uploadDir = Path.Combine(_env.WebRootPath, "images");
                    if (!Directory.Exists(uploadDir))
                        Directory.CreateDirectory(uploadDir);

                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(ImageFile.FileName);
                    string filePath = Path.Combine(uploadDir, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await ImageFile.CopyToAsync(stream);
                    }

                   
                    course.CourseImagePath = "/images/" + uniqueFileName;
                }

              
                await db.SaveChangesAsync();
                return RedirectToAction("CreateImage");
            }
            ViewBag.Courses = new SelectList(db.Courses.ToList(), "CourseID", "Name");
            return View();
          
        }




        public IActionResult Index()
        {
            return View();
        }


    }
}
