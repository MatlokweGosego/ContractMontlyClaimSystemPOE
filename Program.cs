using ContractMontlyClaimSystemPOE.Services;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
// the entry point of the application
var builder = WebApplication.CreateBuilder();

// --- 1. Service Registration (Dependency Injection Container) ---
// Add essential framework services for MVC architecture.
builder.Services.AddControllersWithViews();

// Add session support (used for storing temporary user data like login status 
builder.Services.AddSession(options =>
{
    //Set the maximum time a session can be inactive before expiring.
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    // Prevents client-side script access to the session cookie for security.
    options.Cookie.HttpOnly = true;
    // Marks the cookie as essential for the application to function.
    options.Cookie.IsEssential = true;
});

// Register custom application services for Dependency Injection.
// IDatabaseService handles all database connection and operations.
builder.Services.AddScoped<IDatabaseService, DatabaseService>();
// IClaimService likely handles user authentication, permissions, and roles (claims).
builder.Services.AddScoped<IClaimService, ClaimService>();

// Add configuration
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

var app = builder.Build();

// Initialize database on startup
using (var scope = app.Services.CreateScope())
{
    var dbService = scope.ServiceProvider.GetRequiredService<IDatabaseService>();
    dbService.InitializeSystem();
}

// Configure HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
// Enforces redirecting HTTP requests to HTTPS.
app.UseHttpsRedirection();
app.UseStaticFiles();// Enables serving static files
app.UseRouting();// Adds routing capabilities,
app.UseAuthorization();// Enables user authorization checks
app.UseSession();// Enables the use of session state throughout the application.
// Defines the default route pattern for MVC controllers.
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
// Starts the application, listening for incoming HTTP requests.
app.Run();
