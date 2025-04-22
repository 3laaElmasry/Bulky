using BulkyBook.DataAccess.Repostiory.IRepositroy;
using BulkyBook.Models;
using Microsoft.AspNetCore.Mvc;


namespace BulkyBookWeb.Controllers
{
    public class CategoryController : Controller
    {

        private readonly ICategoryRepository _categoryRepo;

        public CategoryController(ICategoryRepository categoryRepo)
        {
            _categoryRepo = categoryRepo;
        }

        public IActionResult Index()
        {
            List<Category> categoriesList = _categoryRepo.GetAll().ToList();
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
           
            if(category.Name is not null && 
                category.Name == category.DisplayOrder.ToString())
            {
                
                ModelState.AddModelError("", "The Name can't be equal to Dispaly Order");
            }

            if (ModelState.IsValid)
            {

                _categoryRepo.Add(category);
                _categoryRepo.Save();
                TempData["Success"] = "Category Created Successfully";
                return RedirectToAction(nameof(CategoryController.Index));
            }
            return View(category);
        }


        [HttpGet]
        public IActionResult Edit(int? categoryId)
        {
            if(categoryId is null or 0)
            {
                return NotFound();
            }

            var category = _categoryRepo.Get(c => c.Id == categoryId);
            if(category is null)
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
                _categoryRepo.Update(category);
                _categoryRepo.Save();
                TempData["Success"] = "Category Updated Successfully";

                return RedirectToAction(nameof(CategoryController.Index));
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

            var category = _categoryRepo.Get(c => c.Id == categoryId);
            if (category is null)
            {
                return NotFound();
            }
            return View(category);
        }

        [HttpPost,ActionName("Delete")]
        public IActionResult DeletePost(int? categoryId)
        {
            var category = _categoryRepo.Get(c => c.Id == categoryId);
            if (category is null)
            {
                return NotFound();
            }
            _categoryRepo.Remove(category);
            _categoryRepo.Save();
            TempData["Success"] = "Category Deleted Successfully";
            return RedirectToAction(nameof(CategoryController.Index));
        }
    }
}
