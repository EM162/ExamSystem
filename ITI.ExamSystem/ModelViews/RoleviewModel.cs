using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eF_Kres.ModelViews
{
    [NotMapped]
    public class RoleviewModel
    {
        [Key]
        public string Name {set; get;}

        public int id {set; get;}

        public bool IsSelected {set; get;}

        public string? RoleId { get; set; }

        
        [StringLength(50)]
        public string? RoleName { get; set; }

        
        public string? Description { get; set; }

    }
}
