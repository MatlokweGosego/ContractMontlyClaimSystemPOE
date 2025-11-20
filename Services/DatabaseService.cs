using System.Data.SqlClient;
using System.Diagnostics;

namespace ContractMontlyClaimSystemPOE.Services
{
    public class DatabaseService : IDatabaseService
    {
        private string instanceName = "claim_system";
        private string databaseName = "claims_database";
        private string connectionStringToInstance => $@"Server=(localdb)\{instanceName};Integrated Security=true;";
        private string connectionStringToDatabase => $@"Server=(localdb)\{instanceName};Database={databaseName};Integrated Security=true;";

        public void InitializeSystem()
        {
            try
            {
                // Check and create LocalDB instance
                CreateClaimSystemInstance();

                // Check and create Database
                CreateDatabase();

                // Check and create Tables
                CreateTables();

                Console.WriteLine("LocalDB instance, database, and tables verified successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error initializing system: {ex.Message}");
            }
        }

        // -----------------------------
        // LocalDB Instance Handling
        // -----------------------------
        private void CreateClaimSystemInstance()
        {
            if (CheckInstanceExists())
            {
                Console.WriteLine($"LocalDB instance '{instanceName}' already exists.");
                return;
            }

            var processStartInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = $"/c sqllocaldb create \"{instanceName}\"",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            using (var process = new Process { StartInfo = processStartInfo })
            {
                process.Start();
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();
                process.WaitForExit();

                if (process.ExitCode == 0)
                    Console.WriteLine($"LocalDB instance '{instanceName}' created successfully!");
                else
                    Console.WriteLine($"Error creating instance: {error}");
            }
        }

        private bool CheckInstanceExists()
        {
            var processStartInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = $"/c sqllocaldb info \"{instanceName}\"",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            using (var process = new Process { StartInfo = processStartInfo })
            {
                process.Start();
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();
                process.WaitForExit();

                if (!string.IsNullOrWhiteSpace(error) &&
                    error.Contains($"LocalDB instance \"{instanceName}\" doesn't exist", StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }

                return !string.IsNullOrWhiteSpace(output)
                    && !output.Contains("doesn't exist", StringComparison.OrdinalIgnoreCase);
            }
        }

        // -----------------------------
        // Database Handling
        // -----------------------------
        private void CreateDatabase()
        {
            string createDbQuery = $@"
                IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = '{databaseName}')
                BEGIN
                    CREATE DATABASE [{databaseName}];
                END";

            using (var connection = new SqlConnection(connectionStringToInstance))
            {
                connection.Open();
                using (var command = new SqlCommand(createDbQuery, connection))
                {
                    command.ExecuteNonQuery();
                }
            }

            Console.WriteLine($"Database '{databaseName}' verified or created.");
        }

        // -----------------------------
        // Table Handling
        // -----------------------------
        private void CreateTables()
        {
            string createUsersTable = @"
                IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Users' AND xtype='U')
                BEGIN
                    CREATE TABLE Users (
                        userID INT PRIMARY KEY IDENTITY(1,1),
                        full_names VARCHAR(100),
                        surname VARCHAR(100),
                        email VARCHAR(100),
                        role VARCHAR(100),
                        gender VARCHAR(100),
                        password VARCHAR(100),
                        date DATE
                    );
                END";

            string createClaimsTable = @"
                IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Claims' AND xtype='U')
                BEGIN
                    CREATE TABLE Claims (
                        claimID INT PRIMARY KEY IDENTITY(1,1),
                        number_of_sessions INT,
                        number_of_hours INT,
                        amount_of_rate DECIMAL(18,2),
                        module_name VARCHAR(100),
                        faculty_name VARCHAR(100),
                        supporting_documents VARCHAR(500),
                        claim_status VARCHAR(100),
                        additional_notes VARCHAR(1000),
                        creating_date DATE,
                        lecturerID INT,
                        FOREIGN KEY (lecturerID) REFERENCES Users(userID)
                    );
                END";

            using (var connection = new SqlConnection(connectionStringToDatabase))
            {
                connection.Open();

                using (var cmd = new SqlCommand(createUsersTable, connection))
                    cmd.ExecuteNonQuery();

                using (var cmd = new SqlCommand(createClaimsTable, connection))
                    cmd.ExecuteNonQuery();
            }

            Console.WriteLine("Tables 'Users' and 'Claims' verified or created.");
        }

        private void InsertSampleData()
        {
            using (var connection = new SqlConnection(connectionStringToDatabase))
            {
                connection.Open();

                // Insert sample users if none exist
                var checkUsersQuery = "SELECT COUNT(*) FROM Users";
                using (var checkCmd = new SqlCommand(checkUsersQuery, connection))
                {
                    var userCount = (int)checkCmd.ExecuteScalar();
                    if (userCount == 0)
                    {
                        var insertUsersQuery = @"
                            INSERT INTO Users (full_names, surname, email, role, password, date) VALUES
                            ('Gosego Katleho'Matlokwe', 'GKMatlokwe@university.com', 'Lecturer', 'password', GETDATE()),
                            ('Amyoli Khumoetsile', 'Molefe', 'AmyoliKMolefe@university.com', 'Coordinator', 'password', GETDATE()),
                            ('Chulumanco Kgosietsile', 'Tinise', 'chulumanco@university.com', 'Manager', 'password', GETDATE()),
                            ('Qhamani Barulagani', 'Ngwane', 'ngwane@university.com', 'HR', 'password', GETDATE())";

                        using (var insertCmd = new SqlCommand(insertUsersQuery, connection))
                        {
                            insertCmd.ExecuteNonQuery();
                        }
                        Console.WriteLine("Sample users inserted successfully!");
                    }
                }

                // Insert sample claims if none exist
                var checkClaimsQuery = "SELECT COUNT(*) FROM Claims";
                using (var checkCmd = new SqlCommand(checkClaimsQuery, connection))
                {
                    var claimCount = (int)checkCmd.ExecuteScalar();
                    if (claimCount == 0)
                    {
                        var insertClaimsQuery = @"
                            INSERT INTO Claims (number_of_sessions, number_of_hours, amount_of_rate, module_name, faculty_name, claim_status, creating_date, lecturerID) VALUES
                            (10, 40, 250.00, 'Computer Science 101', 'Faculty of Science', 'Pending', GETDATE(), 1),
                            (8, 32, 300.00, 'Mathematics 202', 'Faculty of Science', 'Pre-Approved', GETDATE(), 1),
                            (12, 48, 275.00, 'Business Management', 'Faculty of Commerce', 'Approved', GETDATE(), 1)";

                        using (var insertCmd = new SqlCommand(insertClaimsQuery, connection))
                        {
                            insertCmd.ExecuteNonQuery();
                        }
                        Console.WriteLine("Sample claims inserted successfully!");
                    }
                }
            }
        }
    }

}
