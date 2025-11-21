using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Claims;


using System.ComponentModel.DataAnnotations;

namespace ContractMontlyClaimSystemPOE.Models
{
    public class User
    {
        public int UserID { get; set; }

        [Required(ErrorMessage = "Full names are required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Full names must be 2-100 characters")]
        public string FullNames { get; set; }

        [Required(ErrorMessage = "Surname is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Surname must be 2-50 characters")]
        public string Surname { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Role is required")]
        [RegularExpression("(Lecturer|Coordinator|Manager|HR)", ErrorMessage = "Role must be Lecturer, Coordinator, Manager, or HR")]
        public string Role { get; set; } // "Lecturer", "Coordinator", "Manager", "HR"

        public string Gender { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
        public string Password { get; set; }

        public DateTime Date { get; set; } = DateTime.Now;
    }
}