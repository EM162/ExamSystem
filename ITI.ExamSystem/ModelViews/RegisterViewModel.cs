using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eF_Kres.ModelViews
{
    [NotMapped]
    public class RegisterViewModel
    {
        [Required]
        public string FullName { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required, DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Required, Compare("Password")]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; }

        public string? Role { get; set; }

        public string? SelectedRole { get; set; }
        //public List<SelectListItem> Roles { get; set; } = new();
        public List<RoleviewModel> Roles { get; set; } = new();
    }
}
