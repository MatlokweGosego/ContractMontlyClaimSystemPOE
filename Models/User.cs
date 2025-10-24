using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Claims;


namespace ContractMontlyClaimSystemPOE.Models
{
    public class User
    {

        [Key]
        public int UserID { get; set; }

        [Required]
        [Display(Name = "Full Names")]
        public string FullNames { get; set; }

        [Required]
        public string Surname { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Role { get; set; } // "Lecturer", "Coordinator", "Manager"

        public string Gender { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public DateTime Date { get; set; } = DateTime.Now;

        public ICollection<Claim> Claims { get; set; }
    }

}

