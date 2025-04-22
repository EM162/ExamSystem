using ITI.ExamSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Rendering;
using X.PagedList;
using X.PagedList.Extensions;

namespace ITI.ExamSystem.Controllers
{
    public class AdminController : Controller
    {
        private readonly OnlineExaminationDBContext db;

        public AdminController(OnlineExaminationDBContext _db)
        {
            db = _db;
        }
        public IActionResult Dashboard()
        {
            return View();
        }

        //Students
        public IActionResult ReadStudents(int? page)
        {
            int pageSize = 7; // Number of students per page
            int pageNumber =page ?? 1; // Current page number
            var studentsQuery = db.Users.Where(u => u.Roles.Any(r => r.RoleName == "Student")).ToList();
            var students = studentsQuery.ToPagedList(pageNumber, pageSize);
            return View(students);
        }

        public IActionResult CreateStudent()
        {
            return View();
        }
        [HttpPost]
        public IActionResult CreateStudent(User student)
        {
            if (ModelState.IsValid)
            {
                var studentRole = db.Roles.FirstOrDefault(r => r.RoleName == "Student");
                if (studentRole != null)
                {
                    db.Users.Add(student);
                    db.SaveChanges();
                    student.Roles.Add(studentRole);
                    db.SaveChanges();
                }
               
                return RedirectToAction("ReadStudents");
            }
            return View(student);
        }
        public IActionResult EditStudent(int id)
        {
            var student = db.Users.Find(id);
            if (student == null)
            {
                return NotFound();
            }
            return View(student);
        }
        [HttpPost]
        public IActionResult EditStudent(User student)
        {
            if (ModelState.IsValid)
            {
                db.Users.Update(student);
                db.SaveChanges();
                return RedirectToAction("ReadStudents");
            }
            return View(student);
        }
        public IActionResult DeleteStudent(int id)
        {
            var student = db.Users.Find(id);
            if (student == null)
            {
                return NotFound();
            }
            return View(student);
        }
        //[HttpPost, ActionName("DeleteStudent")]
        //public IActionResult DeleteStudentConfirmed(int id)
        //{
        //    var student = db.Users.Find(id);
        //    if (student != null)
        //    {
        //        var userRoles = db.Roles.Where(ur => ur.UserId == student.UserID).ToList();
        //        db.Users.Remove(student);
        //        db.SaveChanges();
        //    }
        //    return RedirectToAction("ReadStudents");
        //}

      
        public IActionResult SearchStudents(string searchTerm,int? page)
        {
            int pageSize = 7; // Number of students per page
            int pageNumber = page ?? 1; // Current page number
            var studentsQuery = db.Users
                        .Where(u => u.Roles.Any(r => r.RoleName == "Student") &&
                       (string.IsNullOrEmpty(searchTerm) || u.FullName.ToLower().Contains(searchTerm.ToLower())))
                    .ToList();
            var students = studentsQuery.ToPagedList(pageNumber, pageSize);
            ViewBag.SearchTerm = searchTerm; // Pass the search term to the view
            return View("ReadStudents", students);
        }
    }
}
