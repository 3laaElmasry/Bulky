using BulkyBook.DataAccess.Repostiory.IRepositroy;
using BulkyBook.Models;
using Microsoft.AspNetCore.Mvc;


namespace BulkyBookWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {

        private readonly IUnitOfWork _unitOfWork;

        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            List<Category> categoriesList = _unitOfWork.CategoryRepo.GetAll().ToList();
            return View(categoriesList);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Category category)
        {

            if (category.Name is not null &&
                category.Name == category.DisplayOrder.ToString())
            {

                ModelState.AddModelError("", "The Name can't be equal to Dispaly Order");
            }

            if (ModelState.IsValid)
            {

                _unitOfWork.CategoryRepo.Add(category);
                _unitOfWork.Save();
                TempData["Success"] = "Category Created Successfully";
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }


        [HttpGet]
        public IActionResult Edit(int? categoryId)
        {
            if (categoryId is null or 0)
            {
                return NotFound();
            }

            var category = _unitOfWork.CategoryRepo.Get(c => c.Id == categoryId);
            if (category is null)
            {
                return NotFound();
            }
            return View(category);
        }

        [HttpPost]
        public IActionResult Edit(Category category)
        {

            if (category.Name is not null &&
                category.Name == category.DisplayOrder.ToString())
            {

                ModelState.AddModelError("", "The Name can't be equal to Dispaly Order");
            }

            if (ModelState.IsValid)
            {
                _unitOfWork.CategoryRepo.Update(category);
                _unitOfWork.Save();
                TempData["Success"] = "Category Updated Successfully";

                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }


        [HttpGet]
        public IActionResult Delete(int? categoryId)
        {
            if (categoryId is null or 0)
            {
                return NotFound();
            }

            var category = _unitOfWork.CategoryRepo.Get(c => c.Id == categoryId);
            if (category is null)
            {
                return NotFound();
            }
            return View(category);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePost(int? categoryId)
        {
            var category = _unitOfWork.CategoryRepo.Get(c => c.Id == categoryId);
            if (category is null)
            {
                return NotFound();
            }
            _unitOfWork.CategoryRepo.Remove(category);
            _unitOfWork.Save();
            TempData["Success"] = "Category Deleted Successfully";
            return RedirectToAction(nameof(Index));
        }
    }
}
