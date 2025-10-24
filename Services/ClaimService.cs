using ContractMontlyClaimSystemPOE.Models;
using System.Data.SqlClient;

namespace ContractMontlyClaimSystemPOE.Services
{
    public class ClaimService : IClaimService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public ClaimService(IWebHostEnvironment environment, IConfiguration configuration)
        {
            _environment = environment;
            _configuration = configuration;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<int> SubmitClaim(Claim claim, IFormFile supportingDocument)
        {
            try
            {
                string fileName = null;

                // File upload handling
                if (supportingDocument != null && supportingDocument.Length > 0)
                {
                    // Validate file size
                    var maxFileSize = 5 * 1024 * 1024; // 5MB
                    if (supportingDocument.Length > maxFileSize)
                        throw new Exception("File size exceeds the maximum limit of 5MB");

                    // Validate file type
                    var allowedExtensions = new[] { ".pdf", ".docx", ".xlsx" };
                    var fileExtension = Path.GetExtension(supportingDocument.FileName).ToLower();
                    if (!allowedExtensions.Contains(fileExtension))
                        throw new Exception("Only PDF, DOCX, and XLSX files are allowed");

                    // Save file
                    var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    fileName = $"{Guid.NewGuid()}_{Path.GetFileName(supportingDocument.FileName)}";
                    var filePath = Path.Combine(uploadsFolder, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await supportingDocument.CopyToAsync(stream);
                    }
                }

                // Insert claim using raw SQL
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    var query = @"
                        INSERT INTO Claims (
                            number_of_sessions, number_of_hours, amount_of_rate, 
                            module_name, faculty_name, supporting_documents, 
                            claim_status, additional_notes, creating_date, lecturerID
                        ) 
                        OUTPUT INSERTED.claimID
                        VALUES (
                            @Sessions, @Hours, @Rate, @Module, @Faculty, 
                            @Documents, 'Pending', @Notes, @Date, @LecturerID
                        )";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Sessions", claim.NumberOfSessions);
                        command.Parameters.AddWithValue("@Hours", claim.NumberOfHours);
                        command.Parameters.AddWithValue("@Rate", claim.AmountOfRate);
                        command.Parameters.AddWithValue("@Module", claim.ModuleName ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@Faculty", claim.FacultyName ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@Documents", fileName ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@Notes", claim.AdditionalNotes ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@Date", DateTime.Now);
                        command.Parameters.AddWithValue("@LecturerID", claim.LecturerID);

                        var claimId = (int)await command.ExecuteScalarAsync();
                        return claimId;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error submitting claim: {ex.Message}");
            }
        }

        public async Task<List<Claim>> GetClaimsByLecturer(int lecturerId)
        {
            var claims = new List<Claim>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var query = @"
                    SELECT * FROM Claims 
                    WHERE lecturerID = @LecturerID 
                    ORDER BY creating_date DESC";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@LecturerID", lecturerId);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            claims.Add(new Claim
                            {
                                ClaimID = reader.GetInt32(reader.GetOrdinal("claimID")),
                                NumberOfSessions = reader.GetInt32(reader.GetOrdinal("number_of_sessions")),
                                NumberOfHours = reader.GetInt32(reader.GetOrdinal("number_of_hours")),
                                AmountOfRate = reader.GetDecimal(reader.GetOrdinal("amount_of_rate")),
                                ModuleName = reader.IsDBNull(reader.GetOrdinal("module_name")) ? null : reader.GetString(reader.GetOrdinal("module_name")),
                                FacultyName = reader.IsDBNull(reader.GetOrdinal("faculty_name")) ? null : reader.GetString(reader.GetOrdinal("faculty_name")),
                                SupportingDocuments = reader.IsDBNull(reader.GetOrdinal("supporting_documents")) ? null : reader.GetString(reader.GetOrdinal("supporting_documents")),
                                ClaimStatus = reader.GetString(reader.GetOrdinal("claim_status")),
                                AdditionalNotes = reader.IsDBNull(reader.GetOrdinal("additional_notes")) ? null : reader.GetString(reader.GetOrdinal("additional_notes")),
                                CreatingDate = reader.GetDateTime(reader.GetOrdinal("creating_date")),
                                LecturerID = reader.GetInt32(reader.GetOrdinal("lecturerID"))
                            });
                        }
                    }
                }
            }

            return claims;
        }

        public async Task<List<Claim>> GetPendingClaims()
        {
            var claims = new List<Claim>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var query = @"
                    SELECT c.*, u.full_names as LecturerName 
                    FROM Claims c 
                    INNER JOIN Users u ON c.lecturerID = u.userID 
                    WHERE c.claim_status IN ('Pending', 'Pre-Approved') 
                    ORDER BY c.creating_date DESC";

                using (var command = new SqlCommand(query, connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var claim = new Claim
                            {
                                ClaimID = reader.GetInt32(reader.GetOrdinal("claimID")),
                                NumberOfSessions = reader.GetInt32(reader.GetOrdinal("number_of_sessions")),
                                NumberOfHours = reader.GetInt32(reader.GetOrdinal("number_of_hours")),
                                AmountOfRate = reader.GetDecimal(reader.GetOrdinal("amount_of_rate")),
                                ModuleName = reader.GetString(reader.GetOrdinal("module_name")),
                                FacultyName = reader.GetString(reader.GetOrdinal("faculty_name")),
                                SupportingDocuments = reader.IsDBNull(reader.GetOrdinal("supporting_documents")) ? null : reader.GetString(reader.GetOrdinal("supporting_documents")),
                                ClaimStatus = reader.GetString(reader.GetOrdinal("claim_status")),
                                CreatingDate = reader.GetDateTime(reader.GetOrdinal("creating_date")),
                                LecturerID = reader.GetInt32(reader.GetOrdinal("lecturerID"))
                            };

                            // Create a simple User object for the lecturer
                            claim.Lecturer = new User
                            {
                                FullNames = reader.GetString(reader.GetOrdinal("LecturerName"))
                            };

                            claims.Add(claim);
                        }
                    }
                }
            }

            return claims;
        }

        public async Task<bool> ApproveClaim(int claimId, string approvedBy)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string newStatus = approvedBy == "Coordinator" ? "Pre-Approved" : "Approved";

                var query = "UPDATE Claims SET claim_status = @Status WHERE claimID = @ClaimID";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Status", newStatus);
                    command.Parameters.AddWithValue("@ClaimID", claimId);

                    var rowsAffected = await command.ExecuteNonQueryAsync();
                    return rowsAffected > 0;
                }
            }
        }

        public async Task<bool> RejectClaim(int claimId, string rejectedBy)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var query = "UPDATE Claims SET claim_status = 'Rejected' WHERE claimID = @ClaimID";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ClaimID", claimId);

                    var rowsAffected = await command.ExecuteNonQueryAsync();
                    return rowsAffected > 0;
                }
            }
        }

        public async Task<Claim> GetClaimById(int claimId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var query = @"
                    SELECT c.*, u.full_names as LecturerName 
                    FROM Claims c 
                    INNER JOIN Users u ON c.lecturerID = u.userID 
                    WHERE c.claimID = @ClaimID";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ClaimID", claimId);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            var claim = new Claim
                            {
                                ClaimID = reader.GetInt32(reader.GetOrdinal("claimID")),
                                NumberOfSessions = reader.GetInt32(reader.GetOrdinal("number_of_sessions")),
                                NumberOfHours = reader.GetInt32(reader.GetOrdinal("number_of_hours")),
                                AmountOfRate = reader.GetDecimal(reader.GetOrdinal("amount_of_rate")),
                                ModuleName = reader.GetString(reader.GetOrdinal("module_name")),
                                FacultyName = reader.GetString(reader.GetOrdinal("faculty_name")),
                                SupportingDocuments = reader.IsDBNull(reader.GetOrdinal("supporting_documents")) ? null : reader.GetString(reader.GetOrdinal("supporting_documents")),
                                ClaimStatus = reader.GetString(reader.GetOrdinal("claim_status")),
                                AdditionalNotes = reader.IsDBNull(reader.GetOrdinal("additional_notes")) ? null : reader.GetString(reader.GetOrdinal("additional_notes")),
                                CreatingDate = reader.GetDateTime(reader.GetOrdinal("creating_date")),
                                LecturerID = reader.GetInt32(reader.GetOrdinal("lecturerID"))
                            };

                            claim.Lecturer = new User
                            {
                                FullNames = reader.GetString(reader.GetOrdinal("LecturerName"))
                            };

                            return claim;
                        }
                    }
                }
            }

            return null;
        }
    }
}
