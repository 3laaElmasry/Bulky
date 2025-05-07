using BulkyBook.DataAccess.Data;
using BulkyBook.DataAccess.Repostiory.IRepositroy;
using BulkyBook.Models;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = $"{SD.Role_Admin}")]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _db;

        public UserController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            return View();
        }

        

        #region API Calls
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var UsersList = _db.ApplicationUsers.Include(u => u.Company).ToList();

            var userRoles = await _db.UserRoles.ToListAsync();
            var roles = await _db.Roles.ToListAsync();


            foreach (var user in UsersList)
            {
                var roleId = userRoles.FirstOrDefault(u => u.UserId == user.Id)?.RoleId;
                var role = roles.FirstOrDefault(r => r.Id == roleId);
                user.Role = role?.Name;
            }

            var result = UsersList.Select(u => new
            {
                id = u.Id,
                Name = u.Name,
                Email = u.Email,
                PhoneNumber = u.PhoneNumber,
                CompanyName = u.Company == null? "" : u.Company.Name,
                Role = u.Role,
            });

            return Json(new { data = result });
        }

        [HttpPost]
        public async Task<IActionResult> LockUnLock([FromBody]string? userId)
        {

            var userFromDb = await _db.ApplicationUsers.FirstOrDefaultAsync(u => u.Id == userId);
            if(userFromDb == null)
            {
                return Json(new { success = true, message = "Error While Locking / UnLocking" });

            }

            if(userFromDb.LockoutEnd != null && userFromDb.LockoutEnd > DateTime.Now)
            {
                userFromDb.LockoutEnd = DateTime.Now;
                TempData["Success"] = "User is Un Locked Succefully";
            }
            else
            {
                userFromDb.LockoutEnd = DateTime.Now.AddYears(1);
                TempData["Success"] = "User is Locked For 1 Year Succefully";

            }
            _db.SaveChanges();
            return Json(new { success = true, message = "Delete Success" });
        }


        #endregion
    }
}
