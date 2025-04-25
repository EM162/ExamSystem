using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eF_Kres.ModelViews
{
    [NotMapped]
    public class RegisterUser
    {

        [Key]
        public string UserName { get; set; }

        [DataType(DataType.Password)]
        public string Password { get; set; }
        public string Email { get; set; }

        public string address { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password")]
        public string confirmPassword { get; set; }

        [Required]
        [Display(Name = "Role")]
        [NotMapped]
        public string SelectedRole { get; set; }
        public List<SelectListItem> Roles { get; set; } = new();

        //list of roles 
        [NotMapped]
        public List<SelectListItem> AvailableRoles { get; set; } = new();
    }
}
