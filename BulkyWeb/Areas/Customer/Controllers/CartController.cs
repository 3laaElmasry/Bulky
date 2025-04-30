using BulkyBook.DataAccess.Repostiory.IRepositroy;
using BulkyBook.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BulkyBookWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public ShoppingCartVM? ShoppingCartVM { get; set; }
        public CartController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            var userId = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            ShoppingCartVM = new ShoppingCartVM()
            {
                ShoppingCartList = await _unitOfWork.ShoppingCartRepo.GetAllAsync(c => c.ApplicationUserId == userId,"Product"),
            };
            return View(ShoppingCartVM);
        }
    }
}
