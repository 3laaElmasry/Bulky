using BulkyBook.DataAccess.Repostiory.IRepositroy;
using BulkyBook.Models;
using Microsoft.AspNetCore.Mvc;


namespace BulkyBookWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {

        private readonly IUnitOfWork _unitOfWork;

        public ProductController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            List<Product> categoriesList = _unitOfWork.ProductRepo.GetAll().ToList();
            return View(categoriesList);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Product Product)
        {

            if (ModelState.IsValid)
            {

                _unitOfWork.ProductRepo.Add(Product);
                _unitOfWork.Save();
                TempData["Success"] = "Product Created Successfully";
                return RedirectToAction(nameof(Index));
            }
            return View(Product);
        }


        [HttpGet]
        public IActionResult Edit(int? ProductId)
        {
            if (ProductId is null or 0)
            {
                return NotFound();
            }

            var Product = _unitOfWork.ProductRepo.Get(c => c.Id == ProductId);
            if (Product is null)
            {
                return NotFound();
            }
            return View(Product);
        }

        [HttpPost]
        public IActionResult Edit(Product Product)
        {

            if (ModelState.IsValid)
            {
                _unitOfWork.ProductRepo.Update(Product);
                _unitOfWork.Save();
                TempData["Success"] = "Product Updated Successfully";

                return RedirectToAction(nameof(Index));
            }
            return View(Product);
        }


        [HttpGet]
        public IActionResult Delete(int? ProductId)
        {
            if (ProductId is null or 0)
            {
                return NotFound();
            }

            var Product = _unitOfWork.ProductRepo.Get(c => c.Id == ProductId);
            if (Product is null)
            {
                return NotFound();
            }
            return View(Product);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePost(int? ProductId)
        {
            var Product = _unitOfWork.ProductRepo.Get(c => c.Id == ProductId);
            if (Product is null)
            {
                return NotFound();
            }
            _unitOfWork.ProductRepo.Remove(Product);
            _unitOfWork.Save();
            TempData["Success"] = "Product Deleted Successfully";
            return RedirectToAction(nameof(Index));
        }
    }
}
