using BulkyBook.DataAccess.Repostiory.IRepositroy;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
using System.Diagnostics;
using System.Security.Claims;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class OrderController : Controller
    {

        [BindProperty]
        public OrderVM orderVM { get; set; }

        private readonly IUnitOfWork _unitOfWork;

        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }


        [HttpGet]
        public async Task<IActionResult> Details(int orderId)
        {
            orderVM = new OrderVM()
            {
                OrderHeader = await _unitOfWork.OrderHeaderRepo
                .GetAsync(o => o.Id == orderId, includeProperties:"ApplicationUser"),
                OrderDetail = await _unitOfWork.OrderDetailRepo
                .GetAllAsync(o => o.OrderHeaderId == orderId, includeProperties:"Product"),
            };
            return View(orderVM);
        }

        [HttpPost]
        [Authorize(Roles =$"{SD.Role_Admin},{SD.Role_Employee}")]
        public async Task<IActionResult> UpdateOrderDetials()
        {
            OrderHeader? orderHeaderFromdb = await _unitOfWork.OrderHeaderRepo
                .GetAsync(o => o.Id == orderVM.OrderHeader.Id, null);

            if (orderHeaderFromdb is not null)
            {
                orderHeaderFromdb.Name = orderVM.OrderHeader.Name;
                orderHeaderFromdb.PhoneNumber = orderVM.OrderHeader.PhoneNumber;
                orderHeaderFromdb.StreetAddress = orderVM.OrderHeader.StreetAddress;
                orderHeaderFromdb.City = orderVM.OrderHeader.City;
                orderHeaderFromdb.State = orderVM.OrderHeader.State;
                orderHeaderFromdb.PostalCode = orderVM.OrderHeader.PostalCode;

                if(!String.IsNullOrEmpty(orderHeaderFromdb.Carrier))
                {
                    orderHeaderFromdb.Carrier = orderVM.OrderHeader.Carrier;
                }

                if(!String.IsNullOrEmpty(orderHeaderFromdb.TrackingNumber))
                {
                    orderHeaderFromdb.TrackingNumber = orderVM.OrderHeader.TrackingNumber;
                }
                _unitOfWork.OrderHeaderRepo.Update(orderHeaderFromdb);
                await _unitOfWork.Save();
                TempData["Success"] = "Order Details Updated Successfully.";
            }

            return RedirectToAction(nameof(Details), new {orderId = orderHeaderFromdb?.Id});
        }


        [HttpPost]
        [Authorize(Roles = $"{SD.Role_Admin},{SD.Role_Employee}")]

        public async Task<IActionResult> StartProccesing()
        {
            await _unitOfWork.OrderHeaderRepo.UpdateStatus(orderVM.OrderHeader.Id,SD.StatusInProcess);
            await _unitOfWork.Save();
            TempData["Success"] = "Order Details Updated Successfully.";
            return RedirectToAction(nameof(Details), new { orderId = orderVM.OrderHeader.Id });
        }


        [HttpPost]
        [Authorize(Roles = $"{SD.Role_Admin},{SD.Role_Employee}")]

        public async Task<IActionResult> ShipOrder()
        {
            var orderHeaderFromDb = await _unitOfWork.OrderHeaderRepo
                .GetAsync(o => o.Id == orderVM.OrderHeader.Id, null);


            if (orderHeaderFromDb is not null)
            {
                orderHeaderFromDb.TrackingNumber = orderVM.OrderHeader.TrackingNumber;  
                orderHeaderFromDb.Carrier = orderVM.OrderHeader.Carrier;
                orderHeaderFromDb.OrderStatus = SD.StatusShipped;
                orderHeaderFromDb.ShippingDate = DateTime.Now;
                if(orderHeaderFromDb.PaymentStatus == SD.PaymentStatusDelayedPayment)
                {
                    orderHeaderFromDb.OrderStatus = SD.StatusShipped;
                    orderHeaderFromDb.PaymentDueDate = DateOnly.FromDateTime(DateTime.Now.AddDays(30));

                }
                _unitOfWork.OrderHeaderRepo.Update(orderHeaderFromDb);
                await _unitOfWork.Save();
                TempData["Success"] = "Order Shipped Successfully.";

            }
            return RedirectToAction(nameof(Details), new { orderId = orderVM.OrderHeader.Id });
        }

        [HttpPost]
        [Authorize(Roles = $"{SD.Role_Admin},{SD.Role_Employee}")]
        public async Task<IActionResult> CancelOrder()
        {
            var orderHeaderFromDb = await _unitOfWork.OrderHeaderRepo
                .GetAsync(o => o.Id == orderVM.OrderHeader.Id, null);


            if (orderHeaderFromDb is not null)
            {
                if (orderHeaderFromDb.PaymentStatus == SD.StatusApproved)
                {
                    var options = new RefundCreateOptions()
                    {
                        Reason = RefundReasons.RequestedByCustomer,
                        PaymentIntent = orderHeaderFromDb.PaymentIntentedId,
                    };

                    var service = new RefundService();
                    Refund refund = await service.CreateAsync(options);

                    await _unitOfWork.OrderHeaderRepo
                        .UpdateStatus(orderHeaderFromDb.Id, SD.StatusCancelled, SD.StatusRefunded);


                }
                else
                {
                    await _unitOfWork.OrderHeaderRepo
                      .UpdateStatus(orderHeaderFromDb.Id, SD.StatusCancelled,SD.StatusCancelled);
                }
                
                await _unitOfWork.Save();
                TempData["Success"] = "Order Cancelled Successfully.";

            }
            return RedirectToAction(nameof(Details), new { orderId = orderVM.OrderHeader.Id });
        }


        [HttpPost,ActionName("Details")]
        public async Task<IActionResult> Details_Pay_Now()
        {

            orderVM.OrderHeader = await _unitOfWork.OrderHeaderRepo
                .GetAsync(o => o.Id == orderVM.OrderHeader.Id,"ApplicationUser");

            orderVM.OrderDetail = await _unitOfWork.OrderDetailRepo
                .GetAllAsync(o => o.OrderHeaderId == orderVM.OrderHeader.Id);



            string domain = Request.Scheme + "//" + Request.Host.Value + "/";
            var options = new Stripe.Checkout.SessionCreateOptions
            {

                SuccessUrl = domain + $"admin/order/PaymentConfirmation?orderHeaderId={orderVM.OrderHeader.Id}",
                CancelUrl = domain + $"admin/order/Details?orderId{orderVM.OrderHeader.Id}",
                Mode = "payment",
                LineItems = new List<Stripe.Checkout.SessionLineItemOptions>()
            };

            foreach (var item in orderVM.OrderDetail)
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
                (orderVM.OrderHeader.Id, session.Id, session.PaymentIntentId);

            await _unitOfWork.Save();

            Response.Headers.Add("Location", session.Url);

            return new StatusCodeResult(StatusCodes.Status303SeeOther);
        }


        public async Task<IActionResult> PaymentConfirmation(int orderHeaderId)
        {

            OrderHeader orderHeader = await _unitOfWork.OrderHeaderRepo.GetAsync(o => o.Id == orderHeaderId, null);
            if (orderHeader.PaymentStatus == SD.PaymentStatusDelayedPayment)
            {
                //Company User

                var service = new SessionService();

                Session session = service.Get(orderHeader.SessionId);

                if (session.PaymentStatus.ToLower() == "paid")
                {
                    await _unitOfWork.OrderHeaderRepo.UpdateStripePaymentId(orderHeaderId, session.Id, session.PaymentIntentId);
                    await _unitOfWork.OrderHeaderRepo.UpdateStatus(orderHeaderId, orderHeader.OrderStatus, SD.PaymentStatusApproved);
                    await _unitOfWork.Save();
                }
            }
            return View(orderHeaderId);
        }


        #region API Calls
        [HttpGet]
        public async Task<IActionResult> GetAll(string status)
        {
            IEnumerable<OrderHeader> orderHeaderList;

            if (User.IsInRole(SD.Role_Admin) || User.IsInRole(SD.Role_Employee))
            {
                orderHeaderList = await _unitOfWork.OrderHeaderRepo
                .GetAllAsync(includeProperties: "ApplicationUser");
            }
            else
            {
                var claimsIdentity = User.Identity as ClaimsIdentity;
                var userId = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier)!.Value;
                orderHeaderList = await _unitOfWork.OrderHeaderRepo
                .GetAllAsync(includeProperties: "ApplicationUser",filter: o => o.ApplicationUserId == userId);
                
            }
            switch (status)
            {
                case "pending":
                    orderHeaderList = orderHeaderList.Where(o => o.PaymentStatus == SD.PaymentStatusPending);
                    break;
                case "inprocess":
                    orderHeaderList = orderHeaderList.Where(o => o.OrderStatus == SD.StatusInProcess);
                    break;
                case "completed":
                    orderHeaderList = orderHeaderList.Where(o => o.OrderStatus == SD.StatusShipped);
                    break;
                case "approved":
                    orderHeaderList = orderHeaderList.Where(o => o.PaymentStatus == SD.PaymentStatusApproved);
                    break;
                default:
                    break;
            }
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
