using BulkyBook.DataAccess.Repostiory.IRepositroy;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace BulkyBookWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;


        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            List<Product> categoriesList = _unitOfWork.ProductRepo.GetAll(includeProprties : true).ToList();
            return View(categoriesList);
        }

        [HttpGet]
        public IActionResult UpSert(int? ProductId)//Update - Insert
        {
            IEnumerable<SelectListItem> CategoryList = _unitOfWork.CategoryRepo.GetAll()
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name,
                });
            ProductVM productVM = new()
            {
                CategoryList = CategoryList,
                Product = (ProductId is not null and > 0)? _unitOfWork.ProductRepo.Get(c => c.Id == ProductId)! :  new Product()
            };
           
            return View(productVM);
        }

        [HttpPost]
        public IActionResult UpSert(ProductVM productVM,IFormFile? file)
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
                if (productVM.Product.Id == 0)
                {
                    _unitOfWork.ProductRepo.Add(productVM.Product);
                }
                else
                {
                    _unitOfWork.ProductRepo.Update(productVM.Product);
                }
                _unitOfWork.Save();
                TempData["Success"] = "Product Created Successfully";
                return RedirectToAction(nameof(Index));
            }
            return View(productVM.Product);
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

            IEnumerable<SelectListItem> CategoryList = _unitOfWork.CategoryRepo.GetAll()
              .Select(c => new SelectListItem
              {
                  Value = c.Id.ToString(),
                  Text = c.Name,
              });

            ProductVM productVM = new()
            {
                CategoryList = CategoryList,
                Product = Product
            };
            return View(productVM);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePost(int? ProductId)
        {
            var Product = _unitOfWork.ProductRepo.Get(c => c.Id == ProductId);
            if (Product is null)
            {
                return NotFound();
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
            _unitOfWork.Save();
            TempData["Success"] = "Product Deleted Successfully";
            return RedirectToAction(nameof(Index));
        }
    }
}
