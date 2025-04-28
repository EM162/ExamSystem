using System.Security.Cryptography;
using System.Text;
using ITI.ExamSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using X.PagedList;
using X.PagedList.Extensions;

namespace ITI.ExamSystem.Controllers
{
    public class AdminController : Controller
    {
        private readonly OnlineExaminationDBContext db;
        private const int AdminBranchID = 1;

        public AdminController(OnlineExaminationDBContext _db)
        {
            db = _db;
        }
        public IActionResult Dashboard()
        {
            return View();
        }

        //Students
        public IActionResult ReadStudents(int page=1, int pageSize=7)
        {
            var allStudents = db.Users
                .Include(u => u.Roles)
                .Include(u => u.IntakeBranchTrackUsers)
                  .ThenInclude(u => u.Intake)
                .Include(u => u.IntakeBranchTrackUsers)
                    .ThenInclude(u => u.Branch)
                .Include(u => u.IntakeBranchTrackUsers)
                    .ThenInclude(u => u.Track)
                .Where(u => u.Roles.Any(r => r.RoleName == "Student") && !u.IsDeleted)
                .ToList();

            var studentList = allStudents
                .Select(u => new StudnetViewModel
                {
                    UserID = u.UserID,
                    FullName = u.FullName,
                    Email = u.Email,
                    BranchName = u.IntakeBranchTrackUsers.FirstOrDefault()?.Branch?.BranchName ?? "N/A",
                    Tracks = string.Join(",", u.IntakeBranchTrackUsers.Select(x => x.Track?.TrackName).Distinct()),
                    Intakes = string.Join(",", u.IntakeBranchTrackUsers.Select(x => x.Intake?.IntakeName).Distinct()),
                    ExistingImagePath = u.ProfileImagePath
                })
                .ToList();

            var deletedStudents = db.Users
                .Include(u => u.Roles)
                .Where(u => u.IsDeleted && u.Roles.Any(r => r.RoleName == "Student"))
                .ToList();

            var paged = studentList
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var model = new StudentIndexViewModel
            {
                Students = paged,
                DeletedStudents = deletedStudents,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling((double)allStudents.Count() / pageSize)
            };

            return View(model);
        }

        public IActionResult CreateStudent()
        {
            var student = new StudnetViewModel
            {
                AvailableTracks = db.Tracks
                    .Select(t => new StudnetViewModel.TrackItem
                    {
                        TrackID = t.TrackID,
                        TrackName = t.TrackName
                    }).ToList(),
                AvailableIntakes = db.Intakes
                    .Select(i => new StudnetViewModel.IntakeItem
                    {
                        IntakeID = i.IntakeID,
                        IntakeName = i.IntakeName
                    }).ToList()
            };
            return View(student);
        }
        [HttpPost]
        public IActionResult CreateStudent(StudnetViewModel student)
        {
            if (string.IsNullOrWhiteSpace(student.Password))
            {
                ModelState.AddModelError(nameof(student.Password), "Password is required when creating a new Student.");
            }

            if (ModelState.IsValid)
            {
                student.AvailableTracks = db.Tracks
                    .Select(t => new StudnetViewModel.TrackItem
                    {
                        TrackID = t.TrackID,
                        TrackName = t.TrackName
                    }).ToList();
                student.AvailableIntakes = db.Intakes
                    .Select(i => new StudnetViewModel.IntakeItem
                    {
                        IntakeID = i.IntakeID,
                        IntakeName = i.IntakeName
                    }).ToList();
                return View(student);
            }
            var newStudent = new User
            {
                FullName = student.FullName,
                Email = student.Email,
                PasswordHash = HashPassword(student.Password),
                RegistrationDate = DateTime.Now
            };

            if(student.ProfileImage != null)
            {
                newStudent.ProfileImagePath = SaveImage(student.ProfileImage);
            }
            var role = db.Roles.FirstOrDefault(r => r.RoleName == "Student");
            if (role != null)
            {
                newStudent.Roles.Add(role);
            }
            db.Users.Add(newStudent);
            db.SaveChanges();

            foreach (var intakeId in student.SelectedIntakeIDs)
            {
                foreach (var trackId in student.SelectedTrackIDs)
                {
                    db.IntakeBranchTrackUsers.Add(new IntakeBranchTrackUser
                    {
                        UserID = newStudent.UserID,
                        BranchID = AdminBranchID,
                        IntakeID = intakeId,
                        TrackID = trackId
                    });
                }
            }
            db.SaveChanges();
            return RedirectToAction("ReadStudents");

        }
        public IActionResult EditStudent(int id)
        {
            var student = db.Users.FirstOrDefault(u => u.UserID == id);
            if (student == null) return NotFound();

            var selected = db.IntakeBranchTrackUsers
                .Where(x => x.UserID == id && x.BranchID == AdminBranchID)
                .ToList();

            var model = new StudnetViewModel
            {
                UserID = student.UserID,
                FullName = student.FullName,
                Email = student.Email,
                Password = student.PasswordHash,
                ExistingImagePath = student.ProfileImagePath,
                AvailableTracks = db.Tracks
                    .Select(t => new StudnetViewModel.TrackItem { TrackID = t.TrackID, TrackName = t.TrackName })
                    .ToList(),
                AvailableIntakes = db.Intakes
                    .Select(i => new StudnetViewModel.IntakeItem { IntakeID = i.IntakeID, IntakeName = i.IntakeName })
                    .ToList(),
                SelectedIntakeIDs = selected.Select(x => x.IntakeID).Distinct().ToList(),
                SelectedTrackIDs = selected.Select(x => x.TrackID).Distinct().ToList(),
                //ExistingImagePath = instructor.ProfileImagePath
            };

            return View(model);
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
   
        public IActionResult DeleteStudent(int id) {
            var student = db.Users.FirstOrDefault(s =>  s.UserID == id && !s.IsDeleted);
            if (student == null)
            {
                return NotFound();
            }
            return View(student);
        }


        [HttpPost, ActionName("DeleteStudent")]
        public IActionResult DeleteStudentConfirmed(int id)
        {
            var student = db.Users.
                Include(u => u.Roles)
                .FirstOrDefault(s => s.UserID == id && !s.IsDeleted);

            if (student == null)
            {
                return NotFound();
            }

            if (student != null)
            {
                student.IsDeleted = true;
                db.Update(student);
                db.SaveChanges();
            }
            return RedirectToAction("ReadStudents");
        }


        public IActionResult SearchStudents(string searchTerm,int? page)
        {
            int pageSize = 7; // Number of students per page
            int pageNumber = page ?? 1; // Current page number
            var studentsQuery = db.Users
                        .Where(u => u.Roles.Any(r => r.RoleName == "Student") &&
                       (string.IsNullOrEmpty(searchTerm) || u.FullName.ToLower().Contains(searchTerm.ToLower())))
                    .ToList();
            var students = studentsQuery.ToPagedList(pageNumber, pageSize);
            var model = new StudentIndexViewModel
            {
                Students = students.Select(u => new StudnetViewModel
                {
                    UserID = u.UserID,
                    FullName = u.FullName,
                    Email = u.Email,
                    BranchName = u.IntakeBranchTrackUsers.FirstOrDefault()?.Branch?.BranchName ?? "N/A",
                    Tracks = string.Join(",", u.IntakeBranchTrackUsers.Select(x => x.Track?.TrackName).Distinct()),
                    Intakes = string.Join(",", u.IntakeBranchTrackUsers.Select(x => x.Intake?.IntakeName).Distinct()),
                    ExistingImagePath = u.ProfileImagePath
                }).ToList(),
                CurrentPage = pageNumber,
                TotalPages = (int)Math.Ceiling((double)studentsQuery.Count() / pageSize),
                DeletedStudents = new List<User>(),
            };
            ViewBag.SearchTerm = searchTerm; // Pass the search term to the view
            return View("ReadStudents", model);
        }

        //Instructors
        public IActionResult ReadInstructors(int page = 1, int pageSize = 7)
        {
            var instructorRoleId = db.Roles.FirstOrDefault(r => r.RoleName == "Instructor")?.RoleID;

            var instructorsQuery = db.Users
                .Include(u => u.Roles)
                .Include(u => u.IntakeBranchTrackUsers)
                    .ThenInclude(x => x.Intake)
                .Include(u => u.IntakeBranchTrackUsers)
                    .ThenInclude(x => x.Track)
                .Include(u => u.IntakeBranchTrackUsers)
                    .ThenInclude(x => x.Branch)
                .Where(u => !u.IsDeleted && u.Roles.Any(r => r.RoleID == instructorRoleId))
                .ToList();

            var instructorList = instructorsQuery
                .Select(u => new InstructorViewModel
                {
                    UserID = u.UserID,
                    FullName = u.FullName,
                    Email = u.Email,
                    BranchName = u.IntakeBranchTrackUsers.FirstOrDefault()?.Branch?.BranchName ?? "N/A",
                    Tracks = string.Join(",", u.IntakeBranchTrackUsers.Select(x => x.Track?.TrackName).Distinct()),
                    Intakes = string.Join(",", u.IntakeBranchTrackUsers.Select(x => x.Intake?.IntakeName).Distinct()),
                    ExistingImagePath = u.ProfileImagePath
                })
                .ToList();

            var paged = instructorList
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var deleted = db.Users
                .Include(u => u.Roles)
                .Where(u => u.IsDeleted && u.Roles.Any(r => r.RoleName == "Instructor"))
                .ToList();

            var model = new InstructorIndexViewModel
            {
                Instructors = paged,
                DeletedInstructors = deleted,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling((double)instructorList.Count / pageSize)
            };

            return View(model);
        }

        public IActionResult CreateInstructor()
        {
            var model = new InstructorViewModel
            {
                AvailableTracks = db.Tracks
                    .Select(t => new TrackItem { TrackID = t.TrackID, TrackName = t.TrackName })
                    .ToList(),
                AvailableIntakes = db.Intakes
                    .Select(i => new IntakeItem { IntakeID = i.IntakeID, IntakeName = i.IntakeName })
                    .ToList()
            };

            return View(model);
        }
        [HttpPost]
        public IActionResult CreateInstructor(InstructorViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Password))
            {
                ModelState.AddModelError("Password", "Password is required.");
            }

            // ✅ EARLY RETURN if model is invalid
            if (!ModelState.IsValid)
            {
                model.AvailableTracks = db.Tracks
                    .Select(t => new TrackItem { TrackID = t.TrackID, TrackName = t.TrackName })
                    .ToList();
                model.AvailableIntakes = db.Intakes
                    .Select(i => new IntakeItem { IntakeID = i.IntakeID, IntakeName = i.IntakeName })
                    .ToList();
                return View(model);
            }

            // ✅ Only run this block if model is valid
            var instructor = new User
            {
                FullName = model.FullName,
                Email = model.Email,
                PasswordHash = HashPassword(model.Password),
                RegistrationDate = DateTime.Now
            };

            if (model.ProfileImage != null)
            {
                instructor.ProfileImagePath = SaveImage(model.ProfileImage);
            }

            var instructorRole = db.Roles.FirstOrDefault(r => r.RoleName == "Instructor");
            if (instructorRole != null)
            {
                instructor.Roles.Add(instructorRole);
            }

            db.Users.Add(instructor);
            db.SaveChanges();

            foreach (var intakeId in model.SelectedIntakeIDs)
            {
                foreach (var trackId in model.SelectedTrackIDs)
                {
                    db.IntakeBranchTrackUsers.Add(new IntakeBranchTrackUser
                    {
                        UserID = instructor.UserID,
                        BranchID = AdminBranchID,
                        IntakeID = intakeId,
                        TrackID = trackId
                    });
                }
            }

            db.SaveChanges();
            return RedirectToAction("ReadInstructors");
        }
        public IActionResult EditInstructor(int id)
        {
            var instructor = db.Users.FirstOrDefault(u => u.UserID == id);
            if (instructor == null) return NotFound();

            var selected = db.IntakeBranchTrackUsers
                .Where(x => x.UserID == id && x.BranchID == AdminBranchID)
                .ToList();

            var model = new InstructorViewModel
            {
                UserID = instructor.UserID,
                FullName = instructor.FullName,
                Email = instructor.Email,
                Password = instructor.PasswordHash,
                ExistingImagePath = instructor.ProfileImagePath,
                AvailableTracks = db.Tracks
                    .Select(t => new TrackItem { TrackID = t.TrackID, TrackName = t.TrackName })
                    .ToList(),
                AvailableIntakes = db.Intakes
                    .Select(i => new IntakeItem { IntakeID = i.IntakeID, IntakeName = i.IntakeName })
                    .ToList(),
                SelectedIntakeIDs = selected.Select(x => x.IntakeID).Distinct().ToList(),
                SelectedTrackIDs = selected.Select(x => x.TrackID).Distinct().ToList(),
                //ExistingImagePath = instructor.ProfileImagePath
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult EditInstructor(InstructorViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.AvailableTracks = db.Tracks
                    .Select(t => new TrackItem { TrackID = t.TrackID, TrackName = t.TrackName })
                    .ToList();
                model.AvailableIntakes = db.Intakes
                    .Select(i => new IntakeItem { IntakeID = i.IntakeID, IntakeName = i.IntakeName })
                    .ToList();
                return View(model);
            }

            var instructor = db.Users
                .Include(u => u.Roles)
                .FirstOrDefault(u => u.UserID == model.UserID);
            if (instructor == null) return NotFound();

            instructor.FullName = model.FullName;
            instructor.Email = model.Email;
            if (!string.IsNullOrEmpty(model.Password))
            {
                instructor.PasswordHash = HashPassword(model.Password);
            }

            if (model.ProfileImage != null)
            {
                if (!string.IsNullOrEmpty(instructor.ProfileImagePath))
                {
                    var oldPath = Path.Combine("wwwroot/images/profiles", instructor.ProfileImagePath);
                    if (System.IO.File.Exists(oldPath)) System.IO.File.Delete(oldPath);
                }

                instructor.ProfileImagePath = SaveImage(model.ProfileImage);
            }

            var instructorRole = db.Roles.FirstOrDefault(r => r.RoleName == "Instructor");
            if (instructorRole != null && !instructor.Roles.Any(r => r.RoleID == instructorRole.RoleID))
            {
                instructor.Roles.Add(instructorRole);
            }

            db.Users.Update(instructor);
            db.SaveChanges();

            var existingAssignments = db.IntakeBranchTrackUsers
                .Where(x => x.UserID == model.UserID && x.BranchID == AdminBranchID)
                .ToList();

            db.IntakeBranchTrackUsers.RemoveRange(existingAssignments);
            db.SaveChanges();

            foreach (var intakeId in model.SelectedIntakeIDs)
            {
                foreach (var trackId in model.SelectedTrackIDs)
                {
                    db.IntakeBranchTrackUsers.Add(new IntakeBranchTrackUser
                    {
                        UserID = model.UserID,
                        BranchID = AdminBranchID,
                        IntakeID = intakeId,
                        TrackID = trackId
                    });
                }
            }

            db.SaveChanges();
            return RedirectToAction("ReadInstructors");
        }

        public IActionResult DeleteInstructor(int id)
        {
            var instructor = db.Users.FirstOrDefault(u => u.UserID == id && !u.IsDeleted);
            if (instructor == null) return NotFound();
            return View(instructor);
        }

        [HttpPost, ActionName("DeleteInstructor")]
        public IActionResult DeleteInstructorConfirmed(int id)
        {
            var instructor = db.Users
                .Include(u => u.Roles)
                .FirstOrDefault(u => u.UserID == id && !u.IsDeleted);

            if (instructor == null) return NotFound();

            instructor.IsDeleted = true;
            db.Users.Update(instructor);
            db.SaveChanges();

            return RedirectToAction("ReadInstructors");
        }

        public IActionResult SearchInstructors(string searchTerm, int? page)
        {
            int pageSize = 7; // Number of students per page
            int pageNumber = page ?? 1; // Current page number
            var instructorsQuery = db.Users
                        .Where(u => u.Roles.Any(r => r.RoleName == "Instructor") &&
                       (string.IsNullOrEmpty(searchTerm) || u.FullName.ToLower().Contains(searchTerm.ToLower())))
                    .ToList();
            var instructors = instructorsQuery.ToPagedList(pageNumber, pageSize);

            var model = new InstructorIndexViewModel
            {
                Instructors = instructors.Select(u => new InstructorViewModel
                {
                    UserID = u.UserID,
                    FullName = u.FullName,
                    Email = u.Email,
                    BranchName = u.IntakeBranchTrackUsers.FirstOrDefault()?.Branch?.BranchName ?? "N/A",
                    Tracks = string.Join(",", u.IntakeBranchTrackUsers.Select(x => x.Track?.TrackName).Distinct()),
                    Intakes = string.Join(",", u.IntakeBranchTrackUsers.Select(x => x.Intake?.IntakeName).Distinct()),
                    ExistingImagePath = u.ProfileImagePath
                }).ToList(),
                CurrentPage = pageNumber,
                TotalPages = (int)Math.Ceiling((double)instructorsQuery.Count() / pageSize),
                DeletedInstructors = new List<User>(),
            };

            ViewBag.SearchTerm = searchTerm; // Pass the search term to the view
            return View("ReadInstructors", model);
        }

        private string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        private string SaveImage(IFormFile file)
        {
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "profiles");

            // Ensure the folder exists
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            return fileName;
        }

    }
}
