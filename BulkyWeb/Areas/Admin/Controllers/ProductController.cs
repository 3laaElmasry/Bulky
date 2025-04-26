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
    public class ProductController : Controller
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;


        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Index()
        {
            IEnumerable<Product> productList = await _unitOfWork.ProductRepo.GetAllAsync("Category");
            return View(productList);
        }

        [HttpGet]
        public async Task<IActionResult> UpSert(int? productId)//Update - Insert
        {
            IEnumerable<Category> categories = await _unitOfWork.CategoryRepo.GetAllAsync(null);

            IEnumerable<SelectListItem> CategoryList = categories
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name,
                });

            Product? product = null;
            product = await _unitOfWork.ProductRepo.GetAsync(c => c.Id == productId, null);

            if(product is null)
            {
                product = new Product();
            }
            ProductVM productVM = new()
            {
                CategoryList = CategoryList,
                
                Product = product
            };
           
            return View(productVM);
        }

        [HttpPost]
        public async Task<IActionResult> UpSert(ProductVM productVM,IFormFile? file)
        {

            if (ModelState.IsValid)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if(file is not null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string productPath = Path.Combine(wwwRootPath, @"Images/Product");

                    if(!String.IsNullOrEmpty(productVM.Product.ImgUrl))
                    {
                        string oldImgPath = Path.Combine(wwwRootPath, productVM.Product.ImgUrl.TrimStart('/'));
                        if(System.IO.File.Exists(oldImgPath))
                        {
                            System.IO.File.Delete(oldImgPath);
                        }
                    }

                    using (var fileStream = new FileStream(Path.Combine(productPath,fileName),FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    productVM.Product.ImgUrl = "/Images/Product/" + fileName;
                }
                string message = "";
                if (productVM.Product.Id == 0)
                {
                   await _unitOfWork.ProductRepo.AddAsync(productVM.Product);
                    message = "Created";
                }
                else
                {
                    _unitOfWork.ProductRepo.Update(productVM.Product);
                    message = "Edited";

                }
                await _unitOfWork.Save();
                TempData["Success"] = $"Product {message} Successfully";
                return RedirectToAction(nameof(Index));
            }
            return View(productVM.Product);
        }

        #region API Calls
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var productList = await _unitOfWork.ProductRepo.GetAllAsync("Category");

            var result = productList.Select(p => new
            {
                p.Id,
                p.Title,
                p.ISBN,
                p.Price,
                p.Author,
                CategoryName = p.Category?.Name // Ensures `Category.Name` isn't causing cycles
            });

            return Json(new { data = result });
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int? ProductId)
        {
            var Product = await _unitOfWork.ProductRepo.GetAsync(c => c.Id == ProductId, null);
            if (Product is null)
            {
                return Json(new { success = false, message = "Errorr While Deleting" });
            }
            if (!String.IsNullOrEmpty(Product.ImgUrl))
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                string oldImgPath = Path.Combine(wwwRootPath, Product.ImgUrl.TrimStart('/'));
                if (System.IO.File.Exists(oldImgPath))
                {
                    System.IO.File.Delete(oldImgPath);
                }
            }
            _unitOfWork.ProductRepo.Remove(Product);
            TempData["Success"] = "Delete Successfully";
            await _unitOfWork.Save();

            return Json(new { success = true, message = "Delete Success" });
        }


        #endregion
    }
}
