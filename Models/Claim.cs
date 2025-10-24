using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContractMontlyClaimSystemPOE.Models
{
    public class Claim
    {
        public int ClaimID { get; set; }
        public int NumberOfSessions { get; set; }
        public int NumberOfHours { get; set; }
        public decimal AmountOfRate { get; set; }
        public string ModuleName { get; set; }
        public string FacultyName { get; set; }
        public string SupportingDocuments { get; set; }
        public string ClaimStatus { get; set; } = "Pending";
        public string AdditionalNotes { get; set; }
        public DateTime CreatingDate { get; set; } = DateTime.Now;
        public int LecturerID { get; set; }

        // Navigation property (optional)
        public User Lecturer { get; set; }

        // Calculated property
        public decimal TotalAmount => NumberOfHours * AmountOfRate;

    }

}
