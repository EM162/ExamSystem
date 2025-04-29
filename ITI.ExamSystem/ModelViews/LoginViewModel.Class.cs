using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eF_Kres.ModelViews
{
    [NotMapped]
    public class LoginViewModel
    {
       // [Key]
       /// public string username { get; set; }

        public string Email { get; set; }

        [DataType(DataType.Password)]
        public string password { get; set; }

        [DataType(DataType.Password)]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}
