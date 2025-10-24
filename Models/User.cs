using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Claims;


namespace ContractMontlyClaimSystemPOE.Models
{
    public class User
    {
        public int UserID { get; set; }
        public string FullNames { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string Role { get; set; } // "Lecturer", "Coordinator", "Manager"
        public string Gender { get; set; }
        public string Password { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
    }
}



