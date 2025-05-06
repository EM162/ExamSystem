using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace ITI.ExamSystem.Models
{
    public class InstructorViewModel : IValidatableObject
    {
        public int? UserID { get; set; }

        [Required]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [StringLength(100)]
        [EmailAddress]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
       ErrorMessage = "Invalid email address format.")]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        [ValidateNever]
        public string Password { get; set; }

        public int BranchID { get; set; }

        public string BranchName { get; set; }
        public string Tracks { get; set; }
        public string Intakes { get; set; }

        [Display(Name = "Profile Image")]
        [ValidateNever]
        public IFormFile ProfileImage { get; set; }
        [ValidateNever]
        public string ExistingImagePath { get; set; }  // For edit view



        [Display(Name = "Assigned Tracks")]
        public List<int> SelectedTrackIDs { get; set; } = new();

        [Display(Name = "Assigned Intakes")]
        public List<int> SelectedIntakeIDs { get; set; } = new();

        public List<TrackItem> AvailableTracks { get; set; } = new();
        public List<IntakeItem> AvailableIntakes { get; set; } = new();

        // Custom validation logic
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (UserID == null && string.IsNullOrWhiteSpace(Password))
            {
                yield return new ValidationResult(
                    "Password is required when creating a new instructor.",
                    new[] { nameof(Password) });
            }

            if (UserID == null && ProfileImage == null)
            {
                yield return new ValidationResult(
                    "Profile image is required when creating a new instructor.",
                    new[] { nameof(ProfileImage) });
            }
        }

    }

    public class TrackItem
    {
        public int TrackID { get; set; }
        public string TrackName { get; set; }
    }

    public class IntakeItem
    {
        public int IntakeID { get; set; }
        public string IntakeName { get; set; }
    }
}
