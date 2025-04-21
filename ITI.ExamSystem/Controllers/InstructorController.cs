using ITI.ExamSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace ITI.ExamSystem.Controllers
{
    public class InstructorController : Controller
    {
        private readonly OnlineExaminationDBContext _db;

        // Static admin BranchID for testing (replace with claims later)
        private const int AdminBranchID = 1;
        private const int PageSize = 10;

        public InstructorController(OnlineExaminationDBContext db)
        {
            _db = db;
        }

        public IActionResult Index(int page = 1, int pageSize = 10)
        {
            var all = _db.IntakeBranchTrackUsers
                .Where(x => x.BranchID == AdminBranchID && !x.User.IsDeleted)
                .Include(x => x.User)
                .Include(x => x.Track)
                .Include(x => x.Intake)
                .Include(x => x.Branch)
                .ToList();

            var deleted = _db.Users
                .Where(u => u.IsDeleted)
                .ToList();

            var paged = all.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            var model = new InstructorIndexViewModel
            {
                ActiveInstructors = paged,
                DeletedInstructors = deleted,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling((double)all.Count / pageSize)
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
                        BranchID = AdminBranchID,
                        IntakeID = intakeId,
                        TrackID = trackId
                    });
                }
            }

            _db.SaveChanges();
            return RedirectToAction("Index");
        }


        public IActionResult Edit(int id)
        {
            var instructor = _db.Users.FirstOrDefault(u => u.UserID == id);
            if (instructor == null) return NotFound();

            var selected = _db.IntakeBranchTrackUsers
                .Where(x => x.UserID == id && x.BranchID == AdminBranchID)
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
                .Where(x => x.UserID == model.UserID && x.BranchID == AdminBranchID)
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
                        BranchID = AdminBranchID,
                        IntakeID = intakeId,
                        TrackID = trackId
                    });
                }
            }

            _db.SaveChanges();
            return RedirectToAction("Index");
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

            return RedirectToAction("Index");
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
                .Where(x => x.UserID == id && x.BranchID == AdminBranchID)
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
                        BranchID = AdminBranchID
                    });
                }
            }

            _db.SaveChanges();

            return RedirectToAction("Index");
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
