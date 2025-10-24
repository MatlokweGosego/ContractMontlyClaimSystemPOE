using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContractMontlyClaimSystemPOE.Models
{
    public class Claim
    {

        [Key]
        public int ClaimID { get; set; }

        [Required]
        [Display(Name = "Number of Sessions")]
        public int NumberOfSessions { get; set; }

        [Required]
        [Display(Name = "Number of Hours")]
        public int NumberOfHours { get; set; }

        [Required]
        [Display(Name = "Hourly Rate")]
        [Range(1, double.MaxValue, ErrorMessage = "Hourly rate must be greater than 0")]
        public decimal AmountOfRate { get; set; }

        [Required]
        [Display(Name = "Module Name")]
        public string ModuleName { get; set; }

        [Required]
        [Display(Name = "Faculty Name")]
        public string FacultyName { get; set; }

        [Display(Name = "Supporting Documents")]
        public string SupportingDocuments { get; set; }

        [Display(Name = "Claim Status")]
        public string ClaimStatus { get; set; } = "Pending";

        [Display(Name = "Additional Notes")]
        public string AdditionalNotes { get; set; }

        public DateTime CreatingDate { get; set; } = DateTime.Now;

        [Required]
        public int LecturerID { get; set; }

        [ForeignKey("LecturerID")]
        public User Lecturer { get; set; }

        [NotMapped]
        public decimal TotalAmount => NumberOfHours * AmountOfRate;

    }
}
