using BulkyBook.DataAccess.Data;
using BulkyBook.Models;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace BulkyBook.DataAccess.DbInitializer
{
    public class DbInitializer : IDbInitializer
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _db;
        private readonly ILogger<DbInitializer> _logger;

        public DbInitializer(
            RoleManager<IdentityRole> roleManager,
            UserManager<IdentityUser> userManager,
            ApplicationDbContext db,
            ILogger<DbInitializer> logger)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _db = db;
            _logger = logger;
        }

        public async Task InitializeAsync()
        {
            // Apply migrations if pending
            try
            {
                var pendingMigrations = await _db.Database.GetPendingMigrationsAsync();
                if (pendingMigrations.Any())
                {
                    _logger.LogInformation("Applying pending migrations: {Migrations}", string.Join(", ", pendingMigrations));
                    await _db.Database.MigrateAsync();
                    _logger.LogInformation("Migrations applied successfully.");
                }
                else
                {
                    _logger.LogInformation("No pending migrations.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to apply migrations.");
                throw; // Rethrow to halt startup and allow debugging
            }

            // Create roles if they don't exist
            try
            {
                if (!await _roleManager.RoleExistsAsync(SD.Role_Customer))
                {
                    await _roleManager.CreateAsync(new IdentityRole(SD.Role_Customer));
                    await _roleManager.CreateAsync(new IdentityRole(SD.Role_Company));
                    await _roleManager.CreateAsync(new IdentityRole(SD.Role_Admin));
                    await _roleManager.CreateAsync(new IdentityRole(SD.Role_Employee));
                    _logger.LogInformation("Roles created: Customer, Company, Admin, Employee");

                    // Create admin user
                    var adminUser = new ApplicationUser
                    {
                        UserName = "3laaelmasry2005a@gmail.com",
                        Email = "3laaelmasry2005a@gmail.com",
                        Name = "Alaa Elmasry",
                        PhoneNumber = "+201080850238",
                        StreetAddress = "Port Said",
                        State = "Daqahlia",
                        PostalCode = "1234",
                        City = "Mit Ghamr",
                        EmailConfirmed = true // Avoid email confirmation for admin
                    };

                    var result = await _userManager.CreateAsync(adminUser, "9102005 elmasry");
                    if (result.Succeeded)
                    {
                        _logger.LogInformation("Admin user created: {Email}", adminUser.Email);
                        await _userManager.AddToRoleAsync(adminUser, SD.Role_Admin);
                        _logger.LogInformation("Admin user assigned to Admin role");
                    }
                    else
                    {
                        _logger.LogError("Failed to create admin user: {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
                        throw new InvalidOperationException("Failed to create admin user.");
                    }
                }
                else
                {
                    _logger.LogInformation("Roles already exist; skipping role and admin user creation.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to initialize roles or admin user.");
                throw; // Rethrow to halt startup
            }
        }
    }
}