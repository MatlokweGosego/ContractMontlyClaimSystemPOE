using ContractMontlyClaimSystemPOE.Data;

namespace ContractMontlyClaimSystemPOE.Services
{
    public interface IClaimService
    {

        Task<int> SubmitClaim(Claim claim, IFormFile supportingDocument);
        Task<List<Claim>> GetClaimsByLecturer(int lecturerId);
        Task<List<Claim>> GetPendingClaims();
        Task<bool> ApproveClaim(int claimId, string approvedBy);
        Task<bool> RejectClaim(int claimId, string rejectedBy);
        Task<Claim> GetClaimById(int claimId);

    }

    public class ClaimService : IClaimService
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly IConfiguration _configuration;

        public ClaimService(ApplicationDbContext context, IWebHostEnvironment environment, IConfiguration configuration)
        {
            _context = context;
            _environment = environment;
            _configuration = configuration;
        }

        public async Task<int> SubmitClaim(Claim claim, IFormFile supportingDocument)
        {
            try
            {
                if (supportingDocument != null && supportingDocument.Length > 0)
                {
                    // Validate file
                    var maxFileSize = _configuration.GetValue<long>("FileUpload:MaxFileSize", 5242880);
                    var allowedExtensions = _configuration.GetValue<string>("FileUpload:AllowedExtensions", ".pdf,.docx,.xlsx")?.Split(',');

                    if (supportingDocument.Length > maxFileSize)
                        throw new Exception($"File size exceeds the maximum limit of {maxFileSize / 1024 / 1024}MB");

                    var fileExtension = Path.GetExtension(supportingDocument.FileName).ToLower();
                    if (allowedExtensions != null && !allowedExtensions.Contains(fileExtension))
                        throw new Exception($"Only {string.Join(", ", allowedExtensions)} files are allowed");

                    // Save file
                    var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(supportingDocument.FileName)}";
                    var filePath = Path.Combine(uploadsFolder, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await supportingDocument.CopyToAsync(stream);
                    }

                    claim.SupportingDocuments = fileName;
                }

                claim.CreatingDate = DateTime.Now;
                claim.ClaimStatus = "Pending";

                _context.Claims.Add(claim);
                await _context.SaveChangesAsync();

                return claim.ClaimID;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error submitting claim: {ex.Message}");
            }
        }

        public async Task<List<Claim>> GetClaimsByLecturer(int lecturerId)
        {
            return await _context.Claims
                .Where(c => c.LecturerID == lecturerId)
                .OrderByDescending(c => c.CreatingDate)
                .ToListAsync();
        }

        public async Task<List<Claim>> GetPendingClaims()
        {
            return await _context.Claims
                .Where(c => c.ClaimStatus == "Pending" || c.ClaimStatus == "Pre-Approved")
                .Include(c => c.Lecturer)
                .OrderByDescending(c => c.CreatingDate)
                .ToListAsync();
        }

        public async Task<bool> ApproveClaim(int claimId, string approvedBy)
        {
            var claim = await _context.Claims.FindAsync(claimId);
            if (claim == null) return false;

            if (approvedBy == "Coordinator")
                claim.ClaimStatus = "Pre-Approved";
            else if (approvedBy == "Manager")
                claim.ClaimStatus = "Approved";

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RejectClaim(int claimId, string rejectedBy)
        {
            var claim = await _context.Claims.FindAsync(claimId);
            if (claim == null) return false;

            claim.ClaimStatus = "Rejected";
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Claim> GetClaimById(int claimId)
        {
            return await _context.Claims
                .Include(c => c.Lecturer)
                .FirstOrDefaultAsync(c => c.ClaimID == claimId);
        }
    }

}
