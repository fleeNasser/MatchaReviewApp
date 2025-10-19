using System.Diagnostics;
using MatchaReviewApp.Models;
using MatchaReviewApp.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MatchaReviewApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IStoreService _storeService;

        public HomeController(ILogger<HomeController> logger, IStoreService storeService)
        {
            _logger = logger;
            _storeService = storeService;
        }

        public async Task<IActionResult> Index()
        {
            // Get all stores and pick top 3 by rating (tie-breaker: newest)
            var stores = await _storeService.GetAllStoresAsync();
            var top = stores
                .OrderByDescending(s => s.Rating)
                .ThenByDescending(s => s.CreatedAt)
                .Take(3)
                .ToList();

            return View(top);
        }

        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult Stores()
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
