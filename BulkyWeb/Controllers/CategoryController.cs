
using Bulky.DataAccess.Data;
using Bulky.Models;
using Microsoft.AspNetCore.Mvc;


namespace BulkyWeb.Controllers
{
    public class CategoryController : Controller
    {

        private readonly ApplicationDbContext _db;

        public CategoryController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            List<Category> categoriesList = _db.Categories.ToList();
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

                _db.Add(category);
                _db.SaveChanges();
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

            var category = _db.Categories.FirstOrDefault(c => c.Id == categoryId);
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
                _db.Update(category);
                _db.SaveChanges();
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

            var category = _db.Categories.FirstOrDefault(c => c.Id == categoryId);
            if (category is null)
            {
                return NotFound();
            }
            return View(category);
        }

        [HttpPost,ActionName("Delete")]
        public IActionResult DeletePost(int? categoryId)
        {
            var category = _db.Categories.FirstOrDefault(c => c.Id == categoryId);
            if (category is null)
            {
                return NotFound();
            }
            _db.Categories.Remove(category);
            _db.SaveChanges();
            TempData["Success"] = "Category Deleted Successfully";
            return RedirectToAction(nameof(CategoryController.Index));
        }
    }
}
