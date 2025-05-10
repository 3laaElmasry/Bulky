using BulkyBook.DataAccess.Data;
using BulkyBook.DataAccess.Repostiory.IRepositroy;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using System.Text.Json;
using BulkyBook.DataAccess.Repostiory;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = $"{SD.Role_Admin}")]
    public class UserController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;


        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;


        public UserController(IUnitOfWork unitOfWork,
            UserManager<IdentityUser> userManager
            , RoleManager<IdentityRole> roleManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public IActionResult Index()
        {
            return View();
        }



        #region API Calls
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var UsersList = await _unitOfWork.ApplicationUserRepo
                .GetAllAsync(includeProperties: "Company");

            foreach (var user in UsersList)
            {

                var userRoles = await _userManager.GetRolesAsync(user);

                user.Role = userRoles.FirstOrDefault();

                if (user.Company is null)
                {
                    user.Company = new Company { Name = "" };
                }
            }

            
            return Json(new { data = UsersList });
        }

        [HttpPost]
        public async Task<IActionResult> LockUnLock([FromBody] string? id)
        {

            var userFromDb = await _unitOfWork.ApplicationUserRepo.GetAsync(u => u.Id == id,null);
            if (userFromDb == null)
            {
                return Json(new { success = true, message = "Error While Locking / UnLocking" });

            }

            if (userFromDb.LockoutEnd != null && userFromDb.LockoutEnd > DateTime.Now)
            {
                userFromDb.LockoutEnd = DateTime.Now;
                _unitOfWork.ApplicationUserRepo.Update(userFromDb);
                TempData["Success"] = "User is un locked successfully";
            }
            else
            {
                userFromDb.LockoutEnd = DateTime.Now.AddYears(1);
                _unitOfWork.ApplicationUserRepo.Update(userFromDb);
                TempData["Success"] = "User is locked for 1 year successfully";

            }
            await _unitOfWork.Save();
            return Json(new { success = true, message = "Delete Success" });
        }

        [HttpGet]
        public async Task<IActionResult> RoleManagment(string userId)
        {
            IEnumerable<Company> companies = await _unitOfWork.CompanyRepo.GetAllAsync();

            RoleManagmentVM RoleVM = new RoleManagmentVM()
            {
                ApplicationUser = await _unitOfWork.ApplicationUserRepo.GetAsync(u => u.Id == userId, "Company"),
                RoleList = _roleManager.Roles.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Name
                }),
                CompanyList = companies.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                }),
            };


            var userRoles = await _userManager.GetRolesAsync(RoleVM.ApplicationUser!);

            RoleVM.ApplicationUser.Role = userRoles.FirstOrDefault();

            return View(RoleVM);
        }


        [HttpPost]
        public async Task<IActionResult> RoleManagment(RoleManagmentVM roleManagmentVM)
        {
            var userRoles = await _userManager.GetRolesAsync(roleManagmentVM.ApplicationUser);
            string oldRole = userRoles.FirstOrDefault()!;

            if (roleManagmentVM.ApplicationUser.Role is not null && !(roleManagmentVM.ApplicationUser.Role == oldRole))
            {
                //a role was updated
                ApplicationUser? applicationUser = await _unitOfWork.ApplicationUserRepo
                    .GetAsync(u => u.Id == roleManagmentVM.ApplicationUser.Id, null);
                if (applicationUser != null)
                {
                    if (roleManagmentVM.ApplicationUser.Role == SD.Role_Company)
                    {
                        applicationUser.CompanyId = roleManagmentVM.ApplicationUser.CompanyId;
                    }
                    if (oldRole == SD.Role_Company)
                    {
                        applicationUser.CompanyId = null;
                    }
                    _unitOfWork.ApplicationUserRepo.Update(applicationUser);
                    await _unitOfWork.Save();
                    await _userManager.RemoveFromRoleAsync(applicationUser, oldRole);
                    await _userManager.AddToRoleAsync(applicationUser, roleManagmentVM.ApplicationUser.Role);

                }
            }

            return RedirectToAction("Index");
        }
        #endregion
    }
}
