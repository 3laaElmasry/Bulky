using System.Diagnostics;
using BulkyBook.DataAccess.Repostiory.IRepositroy;
using BulkyBook.Models;
using Microsoft.AspNetCore.Mvc;

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
                .GetAllAsync(includeProperties: "Category");



            return View(productList);
        }

        public async Task<IActionResult> Details(int id)
        {
            Product? product = await _unitOfWork.ProductRepo
                .GetAsync(p => p.Id == id,includeProperties: "Category");

            if(product is null)
            {
                return NotFound();
            }

            return View(product);
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
