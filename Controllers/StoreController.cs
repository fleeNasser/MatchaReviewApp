using MatchaReviewApp.Interfaces;
using MatchaReviewApp.Models;
using MatchaReviewApp.Services.Strategies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace MatchaReviewApp.Controllers
{
    /// <summary>
    /// Store controller handling HTTP requests.
    /// Demonstrates low coupling - depends only on service interfaces.
    /// </summary>
    public class StoreController : Controller
    {
        private readonly IStoreService _storeService;
        private readonly IReviewService _reviewService;

        // Constructor Injection (Dependency Injection)
        public StoreController(
            IStoreService storeService,
            IReviewService reviewService)
        {
            _storeService = storeService;
            _reviewService = reviewService;
        }

        // GET: /Store
        public async Task<IActionResult> Index(string sortBy)
        {
            var stores = await _storeService.GetAllStoresAsync();

            // Polymorphism - select strategy at runtime
            ISortStrategy strategy = sortBy switch
            {
                "rating" => new SortByRating(),
                "name" => new SortByName(),
                "newest" => new SortByNewest(),
                _ => new SortByRating()
            };

            var sortedStores = _storeService.ApplySortStrategy(stores, strategy);

            ViewBag.CurrentSort = sortBy;
            return View(sortedStores);
        }

        // GET: /Store/Search?term=matcha
        public async Task<IActionResult> Search(string term)
        {
            var stores = await _storeService.SearchStoresAsync(term);
            ViewBag.SearchTerm = term;
            return View("Index", stores);
        }

        // GET: /Store/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var store = await _storeService.GetStoreByIdAsync(id);
            if (store == null) return NotFound();

            var reviews = await _reviewService.GetReviewsByStoreAsync(id);

            ViewBag.Reviews = reviews;
            return View(store);
        }

        // GET: /Store/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Store/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(Store store)
        {
            if (!ModelState.IsValid)
                return View(store);

            try
            {
                await _storeService.CreateStoreAsync(store);
                TempData["Success"] = "Store created successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(store);
            }
        }

        // GET: /Store/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var store = await _storeService.GetStoreByIdAsync(id);
            if (store == null) return NotFound();

            return View(store);
        }

        // POST: /Store/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, Store store)
        {
            if (id != store.Id) return BadRequest();

            if (!ModelState.IsValid)
                return View(store);

            try
            {
                var result = await _storeService.UpdateStoreAsync(store);
                if (!result) return NotFound();

                TempData["Success"] = "Store updated successfully!";
                
                return RedirectToAction(nameof(Details), new { id });

            }
            catch (ArgumentException ex)
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    ModelState.AddModelError("", error.ErrorMessage);
                }
                return View(store);
            }
        }

        // POST: /Store/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _storeService.DeleteStoreAsync(id);
            if (!result) return NotFound();

            TempData["Success"] = "Store deleted successfully!";
            return RedirectToAction(nameof(Index));
        }
    }
}
