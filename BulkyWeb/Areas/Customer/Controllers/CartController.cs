using BulkyBook.DataAccess.Repostiory.IRepositroy;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
using System.Diagnostics;
using System.Security.Claims;
using static System.Net.WebRequestMethods;

namespace BulkyBookWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        [BindProperty]
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
                await _unitOfWork.Save();

                IEnumerable<ShoppingCart> carts = await _unitOfWork.ShoppingCartRepo
                .GetAllAsync(c => c.ApplicationUserId == cartFromDb.ApplicationUserId);

                HttpContext.Session.SetInt32(SD.SessionCart, carts.Count());

            }
            else
            {
                cartFromDb!.Count -= 1;
                await _unitOfWork.Save();

            }

            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> Remove(int cartId)
        {
            var cartFromDb = await _unitOfWork.ShoppingCartRepo.GetAsync(c => c.Id == cartId, null);

            string? userId = cartFromDb?.ApplicationUserId;
            _unitOfWork.ShoppingCartRepo.Remove(cartFromDb!);
            await _unitOfWork.Save();

            IEnumerable<ShoppingCart> carts = await _unitOfWork.ShoppingCartRepo
               .GetAllAsync(c => c.ApplicationUserId == userId);
            HttpContext.Session.SetInt32(SD.SessionCart, carts.Count());

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


        [HttpPost,ActionName("Summary")]
        public async Task<IActionResult> SummaryPOST()
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            var userId = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            ShoppingCartVM!.ShoppingCartList = await _unitOfWork.ShoppingCartRepo
                .GetAllAsync(c => c.ApplicationUserId == userId, "Product");

            ShoppingCartVM.OrderHeader.OrderDate = DateTime.Now;
            ShoppingCartVM.OrderHeader.ApplicationUserId = userId;

            ApplicationUser? applicationUser = await _unitOfWork.ApplicationUserRepo.GetAsync(u => u.Id == userId, null);
            
            foreach (var cart in ShoppingCartVM.ShoppingCartList)
            {
                cart.Price = GetPriceBasedOnQuantity(cart);
                ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
            }

            

            if(applicationUser!.CompanyId.GetValueOrDefault() == 0)
            {
                ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
                ShoppingCartVM.OrderHeader.OrderStatus = SD.StatusPending;
            }
            else
            {
                //it is a company user soo he can pay after 30 days
                ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusDelayedPayment;
                ShoppingCartVM.OrderHeader.OrderStatus = SD.StatusApproved;
            }

            await _unitOfWork.OrderHeaderRepo.AddAsync(ShoppingCartVM.OrderHeader);
            await _unitOfWork.Save();

            foreach(var cart in ShoppingCartVM.ShoppingCartList)
            {
                OrderDetail orderDetail = new OrderDetail()
                {
                    ProductId = cart.ProductId,
                    Product = cart.Product,
                    OrderHeader = ShoppingCartVM.OrderHeader,
                    OrderHeaderId = ShoppingCartVM.OrderHeader.Id,
                    Price = cart.Price,
                    Count = cart.Count,

                };
                await _unitOfWork.OrderDetailRepo.AddAsync(orderDetail);
                await _unitOfWork.Save();
            }

            if (applicationUser!.CompanyId.GetValueOrDefault() == 0)
            {
                string domain = "https://localhost:7180/";
                var options = new Stripe.Checkout.SessionCreateOptions
                {

                    SuccessUrl = domain + $"customer/cart/OrderConfirmation?id={ShoppingCartVM.OrderHeader.Id}",
                    CancelUrl = domain + $"customer/cart/index",
                    Mode = "payment",
                    LineItems = new List<Stripe.Checkout.SessionLineItemOptions>()
                };

                foreach(var item in ShoppingCartVM.ShoppingCartList)
                {
                    var sessionLineItem = new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)(item.Price * 100),
                            Currency = "usd",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = item.Product!.Title,

                            }
                        },
                        Quantity = item.Count
                        
                    };
                    options.LineItems.Add(sessionLineItem);
                }

                var service = new SessionService();
                Session session = service.Create(options);

                await _unitOfWork.OrderHeaderRepo.UpdateStripePaymentId
                    (ShoppingCartVM.OrderHeader.Id,session.Id,session.PaymentIntentId );

                await _unitOfWork.Save();

                Response.Headers.Add("Location", session.Url);

                return new StatusCodeResult(StatusCodes.Status303SeeOther);
            }
            return RedirectToAction(nameof(OrderConfirmation), new { id = ShoppingCartVM.OrderHeader.Id });
        }

        public async Task<IActionResult> OrderConfirmation(int id)
        {

            OrderHeader orderHeader = await _unitOfWork.OrderHeaderRepo.GetAsync(o => o.Id == id,null);
            if(orderHeader.PaymentStatus != SD.PaymentStatusDelayedPayment)
            {
                //Customer User

                var service = new SessionService();

                Session session = service.Get(orderHeader.SessionId);

                if(session.PaymentStatus.ToLower() == "paid")
                {
                    await _unitOfWork.OrderHeaderRepo.UpdateStripePaymentId(id,session.Id,session.PaymentIntentId);
                    await _unitOfWork.OrderHeaderRepo.UpdateStatus(id,SD.StatusApproved, SD.PaymentStatusApproved);
                    await _unitOfWork.Save();
                }

                List<ShoppingCart> shoppingCarts = _unitOfWork.ShoppingCartRepo
                    .GetAllAsync(c => c.ApplicationUserId == orderHeader.ApplicationUserId).Result.ToList();

                _unitOfWork.ShoppingCartRepo.RemoveRange(shoppingCarts);
                await _unitOfWork.Save();

            }
            return View(id);
        }

    }
}
