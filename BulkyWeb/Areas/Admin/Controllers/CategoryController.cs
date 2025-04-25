using BulkyBook.DataAccess.Repostiory.IRepositroy;
using BulkyBook.Models;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace BulkyBookWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = $"{SD.Role_Admin}")]
    public class CategoryController : Controller
    {

        private readonly IUnitOfWork _unitOfWork;

        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {
            IEnumerable<Category> categoriesList = await _unitOfWork.CategoryRepo.GetAllAsync(null);
            return View(categoriesList);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Category category)
        {

            if (category.Name is not null &&
                category.Name == category.DisplayOrder.ToString())
            {

                ModelState.AddModelError("", "The Name can't be equal to Dispaly Order");
            }

            if (ModelState.IsValid)
            {

                await _unitOfWork.CategoryRepo.AddAsync(category);
                await _unitOfWork.Save();
                TempData["Success"] = "Category Created Successfully";
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }


        [HttpGet]
        public async Task<IActionResult> Edit(int? categoryId)
        {
            if (categoryId is null or 0)
            {
                return NotFound();
            }

            var category = await _unitOfWork.CategoryRepo.GetAsync(c => c.Id == categoryId,null);
            if (category is null)
            {
                return NotFound();
            }
            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Category category)
        {

            if (category.Name is not null &&
                category.Name == category.DisplayOrder.ToString())
            {

                ModelState.AddModelError("", "The Name can't be equal to Dispaly Order");
            }

            if (ModelState.IsValid)
            {
                _unitOfWork.CategoryRepo.Update(category);
                await _unitOfWork.Save();
                TempData["Success"] = "Category Updated Successfully";

                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }


        [HttpGet]
        public async Task<IActionResult> Delete(int? categoryId)
        {
            if (categoryId is null or 0)
            {
                return NotFound();
            }

            var category = await _unitOfWork.CategoryRepo.GetAsync(c => c.Id == categoryId, null);
            if (category is null)
            {
                return NotFound();
            }
            return View(category);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeletePost(int? categoryId)
        {
            var category = await _unitOfWork.CategoryRepo.GetAsync(c => c.Id == categoryId, null);
            if (category is null)
            {
                return NotFound();
            }
            _unitOfWork.CategoryRepo.Remove(category);
            await _unitOfWork.Save();
            TempData["Success"] = "Category Deleted Successfully";
            return RedirectToAction(nameof(Index));
        }
    }
}
