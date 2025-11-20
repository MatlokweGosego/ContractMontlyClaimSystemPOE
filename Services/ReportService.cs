namespace ContractMontlyClaimSystemPOE.Services
{
    public class ReportService : IReportService
    {
        public byte[] GenerateInvoicePdf(Claim claim)
        {
            // Use libraries like QuestPDF or iTextSharp
            return GeneratePdfInvoice(claim);
        }

        public List<Claim> GetMonthlyReport(DateTime month)
        {
            return GetClaimsForMonth(month);
        }
    }
