using BulkyBook.DataAccess.Repostiory.IRepositroy;
using BulkyBook.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class OrderController : Controller
    {

        private readonly IUnitOfWork _unitOfWork;

        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            return View();
        }


        #region API Calls
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            IEnumerable<OrderHeader> orderHeaderList = await _unitOfWork.OrderHeaderRepo
                .GetAllAsync(includeProperties: "ApplicationUser");

           

            return Json(new { data = orderHeaderList });
        }
        

        /*
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
        */


        #endregion
    }
}
