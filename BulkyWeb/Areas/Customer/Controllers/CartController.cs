using BulkyBook.DataAccess.Repostiory.IRepositroy;
using BulkyBook.Models;
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
                ShoppingCartList = await _unitOfWork.ShoppingCartRepo.GetAllAsync(c => c.ApplicationUserId == userId, "Product"),
                OrderHeader = new()
            };

            foreach (var cart in ShoppingCartVM.ShoppingCartList)
            {
                cart.Price = GetPriceBasedOnQuantity(cart);
                ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
            }
            return View(ShoppingCartVM);
        }


        private double GetPriceBasedOnQuantity(ShoppingCart shoppingCart)
        {
            if (shoppingCart.Count <= 50)
            {
                return shoppingCart.Product!.Price;
            }
            else
            {
                if (shoppingCart.Count <= 100)
                {
                    return shoppingCart.Product!.Price50;
                }
                else
                {
                    return shoppingCart.Product!.Price100;
                }
            }
        }


        [HttpGet]
        public async Task<IActionResult> Plus(int cartId)
        {
            var cartFromDb = await _unitOfWork.ShoppingCartRepo.GetAsync(c => c.Id == cartId,null,tracked:true);
            cartFromDb!.Count += 1;
            await _unitOfWork.Save();

            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> Minus(int cartId)
        {
            var cartFromDb = await _unitOfWork.ShoppingCartRepo.GetAsync(c => c.Id == cartId, null, tracked: true);
            if(cartFromDb!.Count <= 1)
            {
                //Remove
                _unitOfWork.ShoppingCartRepo.Remove(cartFromDb!);
            }
            else
            {
                cartFromDb!.Count -= 1;
            }
            await _unitOfWork.Save();

            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> Remove(int cartId)
        {
            var cartFromDb = await _unitOfWork.ShoppingCartRepo.GetAsync(c => c.Id == cartId, null);
             _unitOfWork.ShoppingCartRepo.Remove(cartFromDb!);
            await _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }


        [HttpGet]
        public async Task<IActionResult> Summary()
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            var userId = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            ShoppingCartVM = new ShoppingCartVM()
            {
                ShoppingCartList = await _unitOfWork.ShoppingCartRepo.GetAllAsync(c => c.ApplicationUserId == userId, "Product"),
                OrderHeader = new()
                {
                    ApplicationUserId = userId,

                    ApplicationUser = await _unitOfWork
                    .ApplicationUserRepo
                    .GetAsync(u => u.Id == userId,null),
                }
            };

            ShoppingCartVM.OrderHeader.Name = ShoppingCartVM.OrderHeader.ApplicationUser.Name;
            ShoppingCartVM.OrderHeader.StreetAddress = ShoppingCartVM.OrderHeader.ApplicationUser.StreetAddress;
            ShoppingCartVM.OrderHeader.City = ShoppingCartVM.OrderHeader.ApplicationUser.City;
            ShoppingCartVM.OrderHeader.State = ShoppingCartVM.OrderHeader.ApplicationUser.State;
            ShoppingCartVM.OrderHeader.PostalCode = ShoppingCartVM.OrderHeader.ApplicationUser.PostalCode;
            ShoppingCartVM.OrderHeader.PhoneNumber = ShoppingCartVM.OrderHeader.ApplicationUser.PhoneNumber;


            foreach (var cart in ShoppingCartVM.ShoppingCartList)
            {
                cart.Price = GetPriceBasedOnQuantity(cart);
                ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
            }
            return View(ShoppingCartVM);
        }
    }
}
