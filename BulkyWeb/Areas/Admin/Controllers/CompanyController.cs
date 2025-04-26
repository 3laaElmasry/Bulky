using BulkyBook.DataAccess.Repostiory.IRepositroy;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace BulkyBookWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = $"{SD.Role_Admin}")]
    public class CompanyController : Controller
    {

        private readonly IUnitOfWork _unitOfWork;


        public CompanyController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {
            IEnumerable<Company> companyList = await _unitOfWork.CompanyRepo.GetAllAsync("Category");
            return View(companyList);
        }

        [HttpGet]
        public async Task<IActionResult> UpSert(int? CompanyId)//Update - Insert
        {
            if(CompanyId is null or < 1)
            {
                return View(new Company());
            }

            Company? Company = await _unitOfWork.CompanyRepo.GetAsync(c => c.Id == CompanyId, null);

            if(Company is null)
            {
                Company = new Company();
            }
           
            return View(Company);
        }

        [HttpPost]
        public async Task<IActionResult> UpSert(Company company)
        {
            if (ModelState.IsValid)
            {
                string message = "";
                if (company.Id == 0)
                {
                   await _unitOfWork.CompanyRepo.AddAsync(company);
                    message = "Created";
                }
                else
                {
                    _unitOfWork.CompanyRepo.Update(company);
                    message = "Edited";

                }
                await _unitOfWork.Save();
                TempData["Success"] = $"Company {message} Successfully";
                return RedirectToAction(nameof(Index));
            }
            return View(company);
        }

        #region API Calls
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var CompanyList = await _unitOfWork.CompanyRepo.GetAllAsync("Category");

            var result = CompanyList.Select(c => new
            {
                c.Id,
                c.Name,
                c.StreetAddress,
                c.PhoneNumber,
                c.City,
                c.State // Ensures `Category.Name` isn't causing cycles
            });

            return Json(new { data = result });
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int? CompanyId)
        {
            var Company = await _unitOfWork.CompanyRepo.GetAsync(c => c.Id == CompanyId, null);
            if (Company is null)
            {
                return Json(new { success = false, message = "Errorr While Deleting" });
            }
            _unitOfWork.CompanyRepo.Remove(Company);
            TempData["Success"] = "Delete Successfully";
            await _unitOfWork.Save();

            return Json(new { success = true, message = "Delete Success" });
        }


        #endregion
    }
}
