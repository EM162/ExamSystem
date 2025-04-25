using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eF_Kres.ModelViews
{
    [NotMapped]
    public class RoleviewModel
    {
        [Key]
        public string Name {set; get;}
    }
}
