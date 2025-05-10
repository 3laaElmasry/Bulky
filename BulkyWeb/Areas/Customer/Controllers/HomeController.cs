using System.Diagnostics;
using System.Security.Claims;
using BulkyBook.DataAccess.Repostiory.IRepositroy;
using BulkyBook.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BulkyBook.Utility;

namespace BulkyBookWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {

            IEnumerable<Product> productList = await _unitOfWork.ProductRepo
                .GetAllAsync(includeProperties: "Category,ProductImages");

            return View(productList);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int productId)
        {

            ShoppingCart shoppingCart = new ShoppingCart
            {
                Id = 1,
                Product = await _unitOfWork.ProductRepo.GetAsync(u => u.Id == productId, "Category,ProductImages"),
                ProductId = productId
            };

            

            return View(shoppingCart);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Details(ShoppingCart shoppingCart)
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;

            var userId = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            shoppingCart.ApplicationUserId = userId;

            var cartFromDb = await _unitOfWork.ShoppingCartRepo.GetAsync(c => c.ApplicationUserId == userId
            && c.ProductId == shoppingCart.ProductId,null);

            if (cartFromDb != null)
            {
                cartFromDb.Count += shoppingCart.Count;
                _unitOfWork.ShoppingCartRepo.Update(cartFromDb);
                await _unitOfWork.Save();
                TempData["Success"] = "Cart Updated Successfully";
            }
            else
            {
                await _unitOfWork.ShoppingCartRepo.AddAsync(shoppingCart);
                await _unitOfWork.Save();
                TempData["Success"] = "Cart Created Successfully";

                IEnumerable<ShoppingCart> carts = await _unitOfWork.ShoppingCartRepo
                .GetAllAsync(c => c.ApplicationUserId == userId);
                HttpContext.Session.SetInt32(SD.SessionCart, carts.Count());
            }
            return RedirectToAction(nameof(Index));

        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
