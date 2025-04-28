using System.ComponentModel.DataAnnotations;

namespace eF_Kres.ModelViews
{
    public class ForceChangePasswordViewModel
    {
        [Required]
        public string UserId { get; set; }

        [Required, DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Required, DataType(DataType.Password)]
        [Compare("NewPassword")]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; }
    }
}
