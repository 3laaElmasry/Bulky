using BulkyBook.DataAccess.Repostiory.IRepositroy;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.IdentityModel.Tokens;


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
            IEnumerable<Product> productList = await _unitOfWork.ProductRepo.GetAllAsync(includeProperties: "Category");
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
            product = await _unitOfWork.ProductRepo.GetAsync(c => c.Id == productId,includeProperties: "ProductImages");

            if (product is null)
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
        public async Task<IActionResult> UpSert(ProductVM productVM, List<IFormFile> files)
        {

            if (ModelState.IsValid )
            {
               
                string message = "";
                if (productVM.Product.Id == 0)
                {
                    await _unitOfWork.ProductRepo.AddAsync(productVM.Product);
                    await _unitOfWork.Save();
                    message = "Created";
                }
                else
                {
                    message = "Edited";

                }


                string wwwRootPath = _webHostEnvironment.WebRootPath;
                string productPath = @"Images/Products/Product-" + productVM.Product.Id.ToString();

                string finalPath = Path.Combine(wwwRootPath, productPath);

                var allowedExtensions = new[] { ".jpg", ".png", ".jpeg" };
                long maxFileSize = 10 * 1024 * 1024; // 10 MB
                var errors = new List<string>();

                
                    // Create directory if it doesn't exist
                    Directory.CreateDirectory(finalPath);
                if (!files.IsNullOrEmpty())
                {
                    foreach (var file in files)
                    {
                        // Validate file size
                        if (file.Length > maxFileSize)
                        {
                            errors.Add($"File {file.FileName} is too large. Maximum size is 10 MB.");
                            continue; // Skip to next file
                        }

                        // Validate file extension
                        string extension = Path.GetExtension(file.FileName).ToLower();
                        if (!allowedExtensions.Contains(extension))
                        {
                            errors.Add($"File {file.FileName} has an invalid extension ({extension}). Allowed extensions are: {string.Join(", ", allowedExtensions)}.");
                            continue; // Skip to next file
                        }

                        // Generate unique file name and save file
                        string fileName = Guid.NewGuid().ToString() + extension;
                        string filePath = Path.Combine(finalPath, fileName);
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(fileStream);
                        }

                        ProductImage imageToSave = new ProductImage
                        {
                            ImageUrl = @"/" + productPath + @"/" + fileName,
                            ProductId = productVM.Product.Id,
                        };

                        if (productVM.Product.ProductImages is null)
                            productVM.Product.ProductImages = new List<ProductImage>();

                        productVM.Product.ProductImages.Add(imageToSave);
                    }
                    _unitOfWork.ProductRepo.Update(productVM.Product);
                    await _unitOfWork.Save();
                    // If there are errors, add them to ModelState and return the view
                    if (errors.Any())
                    {
                        foreach (var error in errors)
                        {
                            ModelState.AddModelError("FileUploadError", error);
                        }
                        return View(productVM);
                    }
                }


                TempData["Success"] = $"Product {message} Successfully";
                return RedirectToAction(nameof(Index));
            }


            
            return View(productVM);
        }

        public async Task<IActionResult> DeleteImg(int imgId)
        {

            ProductImage? productImageFromDb = await _unitOfWork.ProductImageRepo
                .GetAsync(i => i.Id == imgId, null);
            var productId = productImageFromDb?.ProductId;
            if (productImageFromDb is not null)
            {

                if (!String.IsNullOrEmpty(productImageFromDb.ImageUrl))
                {

                    string wwwRootPath = _webHostEnvironment.WebRootPath;
                    string oldImgPath = Path.Combine(wwwRootPath, productImageFromDb.ImageUrl.TrimStart('/'));
                    if (System.IO.File.Exists(oldImgPath))
                    {
                        System.IO.File.Delete(oldImgPath);
                    }

                }
                _unitOfWork.ProductImageRepo.Remove(productImageFromDb);
                await _unitOfWork.Save();
                TempData["Success"] = $"Deleted Succefully";
                return RedirectToAction(nameof(UpSert), new { productId = productId});
            }
            TempData["Error"] = $"Error While Deleting";

            return RedirectToAction(nameof(Index));
        }

        #region API Calls
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var productList = await _unitOfWork.ProductRepo.GetAllAsync(includeProperties: "Category");

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

            
            //if (!String.IsNullOrEmpty(Product.ImgUrl))
            //{
            //    string wwwRootPath = _webHostEnvironment.WebRootPath;
            //    string oldImgPath = Path.Combine(wwwRootPath, Product.ImgUrl.TrimStart('/'));
            //    if (System.IO.File.Exists(oldImgPath))
            //    {
            //        System.IO.File.Delete(oldImgPath);
            //    }
            //}
            _unitOfWork.ProductRepo.Remove(Product);
            TempData["Success"] = "Delete Successfully";
            await _unitOfWork.Save();

            return Json(new { success = true, message = "Delete Success" });
        }


        #endregion
    }
}
