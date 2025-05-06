using ITI.ExamSystem.Models;
using ITI.ExamSystem.ModelViews;
using ITI.ExamSystem.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ITI.ExamSystem.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    public class SpAdminController : Controller
    {
        SpAdmin_Repo SpAdmin_Repo { get; set; }

        private readonly OnlineExaminationDBContext _context;


        public SpAdminController(SpAdmin_Repo spAdmin_Repo, OnlineExaminationDBContext context)
        {
            this.SpAdmin_Repo = spAdmin_Repo;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult GetAll()
        {
            var branches = SpAdmin_Repo.GetAllBranches();
          
            return View("ListBranches", branches);
        }

        public IActionResult Detail(int id)
        {
            var result = SpAdmin_Repo.GetBranch_ByID(id);

            if (result != null)
                return Json(result);
            else
                return NoContent();
        }

        public IActionResult Add()
        {
            return PartialView("CreateBranch");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Add(Branch branch)
        {
            if (branch == null)
                return BadRequest("Branch is null");

            if (ModelState.IsValid)
            {
                SpAdmin_Repo.AddBranch(branch);
                return Json(new { success = true });
            }

            ModelState.AddModelError("", "An error occurred while saving the branch.");
            return PartialView("CreateBranch", branch);
        }

        [HttpPost]
        public IActionResult Edit(int id)
        {
            SpAdmin_Repo.Update(id);
            return Ok();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            SpAdmin_Repo.Delete(id);
            return RedirectToAction("GetAll");
        }

        [HttpPost]
        public IActionResult DeleteUser(int id)
        {
            try
            {
                SpAdmin_Repo.DeleteUser(id);
                return Ok();
            }
            catch (InvalidOperationException ex)
            {
                return NotFound("User not found.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An unexpected error occurred.");
            }
        }

        [HttpGet]
        public IActionResult StudentProgress()
        {
            var students = _context.Users
        .Where(u => u.Roles.Any(r => r.RoleName == "Student") && !u.IsDeleted)
        .Include(u => u.UserExams)
            .ThenInclude(ue => ue.Exam)
                .ThenInclude(e => e.Course)  
        .Include(u => u.UserExams)
            .ThenInclude(ue => ue.Exam)
                .ThenInclude(e => e.Questions)
        .ToList();

            var progressData = students.Select(student => new StudentProgressViewModel
            {
                UserID = student.UserID,
                FullName = student.FullName,
                Email = student.Email,
                AverageGrade = student.UserExams.Any() ?
                    student.UserExams.Average(ue => ue.Grade) : 0,
                ExamsTaken = student.UserExams.Count,
                SubjectAverages = student.UserExams
                    .GroupBy(ue => ue.Exam.Course.Name) 
                    .ToDictionary(
                        g => g.Key,
                        g => g.Average(ue => ue.Grade)
                    ),
                ExamHistory = student.UserExams
                    .OrderBy(ue => ue.Exam.ExamDate)
                    .Select(ue => new StudentProgressViewModel.ExamResult
                    {
                        ExamTitle = $"Exam #{ue.Exam.ExamID} ({ue.Exam.ExamType})",
                        ExamDate = ue.Exam.ExamDate.Value,
                        Grade = ue.Grade,
                        Subject = ue.Exam.Course.Name,
                        QuestionCount = ue.Exam.MCQQuestionCount + ue.Exam.TrueFalseQuestionCount,
                        Duration = ue.Exam.Duration
                    }).ToList()
            }).ToList();

            return View(progressData);
        }
    }


}

