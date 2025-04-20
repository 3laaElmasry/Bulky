using BulkyWeb.Data;
using BulkyWeb.Models;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata.Ecma335;

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
           
            if(category.Name == category.DisplayOrder.ToString())
            {
                
                ModelState.AddModelError("name", "The Name can't be equal to Dispaly Order");
            }

            if(ModelState.IsValid)
            {
                
                _db.Add(category);
                _db.SaveChanges();
                return RedirectToAction(nameof(CategoryController.Index));
            }
            return View(category);
        }
    }
}
