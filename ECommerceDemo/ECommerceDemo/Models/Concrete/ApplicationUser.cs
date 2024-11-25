using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerceDemo.Models.Concrete
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public string FullName { get; set; }

        [StringLength(255, ErrorMessage = "You can not enter more than 255 charachter!")]
        [Column(TypeName = "varchar(255)")]
        public string Address { get; set; } = string.Empty;

        [Required]
        public List<CustomerRole> Roles { get; set; }
        public bool IsActive { get; set; } = false;
    }
}
