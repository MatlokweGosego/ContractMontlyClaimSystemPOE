using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContractMontlyClaimSystemPOE.Models
{
    public class Claim
    {
        public int ClaimID { get; set; }

        [Range(1, 50, ErrorMessage = "Sessions must be between 1 and 50")]
        public int NumberOfSessions { get; set; }


        [Range(1, 160, ErrorMessage = "Hours must be between 1 and 160")]
        public int NumberOfHours { get; set; }

        [Range(1, 1000, ErrorMessage = "Rate must be between R1 and R1000")]
        public decimal AmountOfRate { get; set; }

        [Required(ErrorMessage = "Module name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Module name must be 2-100 characters")]
        public string ModuleName { get; set; }
        public string FacultyName { get; set; }
        public string SupportingDocuments { get; set; }
        public string ClaimStatus { get; set; } = "Pending";
        public string AdditionalNotes { get; set; }
        public DateTime CreatingDate { get; set; } = DateTime.Now;
        public int LecturerID { get; set; }

        // Navigation property 
        public User Lecturer { get; set; }

        // Calculated property
        public decimal TotalAmount => NumberOfHours * AmountOfRate;

    }

}
