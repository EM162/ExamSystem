using ITI.ExamSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using X.PagedList.Extensions;

namespace ITI.ExamSystem.Controllers
{
    public class InstructorController : Controller
    {
        private readonly OnlineExaminationDBContext _db;

        // Static admin BranchID for testing (replace with claims later)
        //private const int AdminBranchID = 1;
        private const int PageSize = 10;

        public InstructorController(OnlineExaminationDBContext db)
        {
            _db = db;
        }

        private int GetCurrentUserId()
        {
            var identityId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(identityId))
                throw new Exception("User not authenticated");

            var user = _db.Users.FirstOrDefault(u => u.IdentityUserId == identityId);
            if (user == null)
                throw new Exception("User not found in system");

            return user.UserID;
        }


        private int GetAdminBranchId()
        {
            var userId = GetCurrentUserId();
            var branchId = _db.IntakeBranchTrackUsers
                              .Where(x => x.UserID == userId)
                              .Select(x => x.BranchID)
                              .FirstOrDefault();

            if (branchId == 0)
                throw new InvalidOperationException("Branch not found for current admin.");
            return branchId;
        }

        [Authorize(Roles = "Admin,Instructor,SuperAdmin")]
        public IActionResult IndexInst(int page = 1, int pageSize = 7)
        {
            var instructorRoleId = _db.Roles.FirstOrDefault(r => r.RoleName == "Instructor")?.RoleID;

            var instructorsQuery = _db.Users
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

            var deleted = _db.Users
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




        public IActionResult Create()
        {
            var model = new InstructorViewModel
            {
                AvailableTracks = _db.Tracks
                    .Select(t => new TrackItem { TrackID = t.TrackID, TrackName = t.TrackName })
                    .ToList(),
                AvailableIntakes = _db.Intakes
                    .Select(i => new IntakeItem { IntakeID = i.IntakeID, IntakeName = i.IntakeName })
                    .ToList()
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult Create(InstructorViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Password))
            {
                ModelState.AddModelError("Password", "Password is required.");
            }

            // ✅ EARLY RETURN if model is invalid
            if (!ModelState.IsValid)
            {
                model.AvailableTracks = _db.Tracks
                    .Select(t => new TrackItem { TrackID = t.TrackID, TrackName = t.TrackName })
                    .ToList();
                model.AvailableIntakes = _db.Intakes
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

            var instructorRole = _db.Roles.FirstOrDefault(r => r.RoleName == "Instructor");
            if (instructorRole != null)
            {
                instructor.Roles.Add(instructorRole);
            }

            _db.Users.Add(instructor);
            _db.SaveChanges();

            foreach (var intakeId in model.SelectedIntakeIDs)
            {
                foreach (var trackId in model.SelectedTrackIDs)
                {
                    _db.IntakeBranchTrackUsers.Add(new IntakeBranchTrackUser
                    {
                        UserID = instructor.UserID,
                        BranchID = GetAdminBranchId(),
                        IntakeID = intakeId,
                        TrackID = trackId
                    });
                }
            }

            _db.SaveChanges();
            return RedirectToAction("IndexInst");
        }


        public IActionResult Edit(int id)
        {
            var instructor = _db.Users.FirstOrDefault(u => u.UserID == id);
            if (instructor == null) return NotFound();

            var selected = _db.IntakeBranchTrackUsers
                .Where(x => x.UserID == id && x.BranchID == GetAdminBranchId())
                .ToList();

            var model = new InstructorViewModel
            {
                UserID = instructor.UserID,
                FullName = instructor.FullName,
                Email = instructor.Email,
                Password = instructor.PasswordHash,
                ExistingImagePath = instructor.ProfileImagePath,
                AvailableTracks = _db.Tracks
                    .Select(t => new TrackItem { TrackID = t.TrackID, TrackName = t.TrackName })
                    .ToList(),
                AvailableIntakes = _db.Intakes
                    .Select(i => new IntakeItem { IntakeID = i.IntakeID, IntakeName = i.IntakeName })
                    .ToList(),
                SelectedIntakeIDs = selected.Select(x => x.IntakeID).Distinct().ToList(),
                SelectedTrackIDs = selected.Select(x => x.TrackID).Distinct().ToList(),
                //ExistingImagePath = instructor.ProfileImagePath
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult Edit(InstructorViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.AvailableTracks = _db.Tracks
                    .Select(t => new TrackItem { TrackID = t.TrackID, TrackName = t.TrackName })
                    .ToList();
                model.AvailableIntakes = _db.Intakes
                    .Select(i => new IntakeItem { IntakeID = i.IntakeID, IntakeName = i.IntakeName })
                    .ToList();
                return View(model);
            }

            var instructor = _db.Users
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

            var instructorRole = _db.Roles.FirstOrDefault(r => r.RoleName == "Instructor");
            if (instructorRole != null && !instructor.Roles.Any(r => r.RoleID == instructorRole.RoleID))
            {
                instructor.Roles.Add(instructorRole);
            }

            _db.Users.Update(instructor);
            _db.SaveChanges();

            var existingAssignments = _db.IntakeBranchTrackUsers
                .Where(x => x.UserID == model.UserID && x.BranchID == GetAdminBranchId())
                .ToList();

            _db.IntakeBranchTrackUsers.RemoveRange(existingAssignments);
            _db.SaveChanges();

            foreach (var intakeId in model.SelectedIntakeIDs)
            {
                foreach (var trackId in model.SelectedTrackIDs)
                {
                    _db.IntakeBranchTrackUsers.Add(new IntakeBranchTrackUser
                    {
                        UserID = model.UserID,
                        BranchID = GetAdminBranchId(),
                        IntakeID = intakeId,
                        TrackID = trackId
                    });
                }
            }

            _db.SaveChanges();
            return RedirectToAction("IndexInst");
        }

        public IActionResult Delete(int id)
        {
            var instructor = _db.Users.FirstOrDefault(u => u.UserID == id && !u.IsDeleted);
            if (instructor == null) return NotFound();
            return View(instructor);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var instructor = _db.Users
                .Include(u => u.Roles)
                .FirstOrDefault(u => u.UserID == id && !u.IsDeleted);

            if (instructor == null) return NotFound();

            instructor.IsDeleted = true;
            _db.Users.Update(instructor);
            _db.SaveChanges();

            return RedirectToAction("IndexInst");
        }

        [HttpPost]
        public IActionResult Reactivate(int id)
        {
            var instructor = _db.Users
                .Include(u => u.Roles)  // if needed
                .FirstOrDefault(u => u.UserID == id);

            if (instructor == null) return NotFound();

            instructor.IsDeleted = false;

            // Re-assign "Instructor" role if needed
            var instructorRole = _db.Roles
                .AsNoTracking()  // avoids potential reader conflicts
                .FirstOrDefault(r => r.RoleName == "Instructor");

            if (instructorRole != null && !instructor.Roles.Any(r => r.RoleID == instructorRole.RoleID))
            {
                instructor.Roles.Add(instructorRole);
            }

            // Re-assign intake/track if needed — make sure they are materialized
            var latestAssignments = _db.IntakeBranchTrackUsers
                .Where(x => x.UserID == id && x.BranchID == GetAdminBranchId())
                .ToList();  // ✅ forces query execution

            if (!latestAssignments.Any())
            {
                // Sample reassignment (e.g., assign to default intake/track)
                var defaultIntake = _db.Intakes.FirstOrDefault();
                var defaultTrack = _db.Tracks.FirstOrDefault();

                if (defaultIntake != null && defaultTrack != null)
                {
                    _db.IntakeBranchTrackUsers.Add(new IntakeBranchTrackUser
                    {
                        UserID = id,
                        IntakeID = defaultIntake.IntakeID,
                        TrackID = defaultTrack.TrackID,
                        BranchID = GetAdminBranchId()
                    });
                }
            }

            _db.SaveChanges();

            return RedirectToAction("IndexInst");
        }

          public IActionResult SearchInstructors(string searchTerm, int? page)
        {
            int pageSize = 7; // Number of students per page
            int pageNumber = page ?? 1; // Current page number
            var instructorsQuery = _db.Users
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
            return View("IndexInst", model);
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
