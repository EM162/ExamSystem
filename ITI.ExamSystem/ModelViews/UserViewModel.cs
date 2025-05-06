using System.ComponentModel.DataAnnotations;

namespace ITI.ExamSystem.ModelViews
{
    public class UserViewModel
    {
        public int UserID { get; set; }

        [Required]
        [StringLength(100)]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [StringLength(255)]
        public string ProfileImagePath { get; set; }

        [Display(Name = "Registration Date")]
        public DateTime RegistrationDate { get; set; }

        public bool IsDeleted { get; set; }
        public List<RoleCheckbox> AvailableRoles { get; set; } = new();
    }

    public class RoleCheckbox
    {
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public bool IsSelected { get; set; }
    }
}
