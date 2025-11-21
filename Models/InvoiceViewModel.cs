using System.ComponentModel.DataAnnotations;

namespace ContractMontlyClaimSystemPOE.Models
{
    public class InvoiceViewModel
    {
        public Claim Claim { get; set; }

        [Display(Name = "Invoice Date")]
        public DateTime InvoiceDate { get; set; } = DateTime.Now;

        [Display(Name = "Invoice Number")]
        public string InvoiceNumber { get; set; }

        [Display(Name = "Due Date")]
        public DateTime DueDate { get; set; } = DateTime.Now.AddDays(30);

        [Display(Name = "Subtotal")]
        public decimal TotalAmount => Claim?.TotalAmount ?? 0;

        [Display(Name = "Tax (15%)")]
        public decimal TaxAmount => TotalAmount * 0.15m;

        [Display(Name = "Grand Total")]
        public decimal GrandTotal => TotalAmount + TaxAmount;

        [Display(Name = "Payment Method")]
        public string PaymentMethod { get; set; } = "Electronic Transfer";

        [Display(Name = "Payment Terms")]
        public string PaymentTerms { get; set; } = "Net 30 Days";
    }
}
