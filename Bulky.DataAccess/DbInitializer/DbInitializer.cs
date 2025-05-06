


using BulkyBook.DataAccess.Data;
using BulkyBook.DataAccess.DbInitializer;
using BulkyBook.Models;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace BulkyBook.DataAccess.DBIntilaizer
{
    public class DbInitializer : IDbInitializer
    {

        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _db;

        public DbInitializer(RoleManager<IdentityRole> roleManager,
            UserManager<IdentityUser> userManager,
            ApplicationDbContext db)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _db = db;
        }

        public async Task Initialize()
        {
            //Migration if not applied

            try
            {
                IEnumerable<string> pendingMigrations = await _db.Database.GetPendingMigrationsAsync();
                if (pendingMigrations.Count() > 0)
                {
                   await _db.Database.MigrateAsync();
                } 
            }
            catch (Exception ex)
            {

            }

            if (!await _roleManager.RoleExistsAsync(SD.Role_Customer))
            {

                //Create Roles if not Created
                await _roleManager.CreateAsync(new IdentityRole { Name = SD.Role_Customer });
                await _roleManager.CreateAsync(new IdentityRole { Name = SD.Role_Company });
                await _roleManager.CreateAsync(new IdentityRole { Name = SD.Role_Admin });
                await _roleManager.CreateAsync(new IdentityRole { Name = SD.Role_Employee });


                //if roles are not created, then we will create admin user as will
                await _userManager.CreateAsync(new ApplicationUser
                {
                    UserName = "3laaelmasry2005a@gmail.com",
                    Email = "3laaelmasry2005a@gmail.com",
                    Name = "Alaa Elmasry",
                    PhoneNumber = "+20 1080850238",
                    StreetAddress = "Port Said",
                    State = "Daqahlia",
                    PostalCode = "1234",
                    City = "Mit Ghamr",

                }, "9102005 elmasry");

                ApplicationUser? user = await _db.ApplicationUsers
                    .FirstOrDefaultAsync(u => u.Email == "3laaelmasry2005a@gmail.com");

                await _userManager.AddToRoleAsync(user!, SD.Role_Admin);

            }

             await _db.SaveChangesAsync();
 
        }
    }
}
