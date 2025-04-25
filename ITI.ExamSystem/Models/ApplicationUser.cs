using Microsoft.AspNetCore.Identity;

namespace ITI.ExamSystem.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
        public DateTime RegistrationDate { get; set; }

        public bool MustChangePassword { get; set; } = true;
    }

}
