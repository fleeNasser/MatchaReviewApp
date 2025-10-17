using MatchaReviewApp.Interfaces;
using MatchaReviewApp.Models;
using MatchaReviewApp.Services.Strategies;
using MatchaReviewApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.Scaffolding.Shared.Project;
namespace MatchaReviewApp.Controllers
{
    /// Store controller handling HTTP requests.
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
            return View(new StoreFormViewModel());
        }

        // POST: /Store/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(StoreFormViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var store = new Store
                {
                    Name = model.Name.Trim(),
                    Address = model.Address.Trim(),
                    Description = model.Description.Trim(),
                    Rating = model.Rating,
                    CreatedAt = DateTime.UtcNow

                };  
                await _storeService.CreateStoreAsync(store);
                TempData["Success"] = "Store created successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
        }

        // GET: /Store/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var store = await _storeService.GetStoreByIdAsync(id);
            if (store == null) return NotFound();

            var vm = new StoreFormViewModel
            {
                Id = store.Id,
                Name = store.Name,
                Address = store.Address,
                Description = store.Description,
                Rating = store.Rating,
                CreatedAt = store.CreatedAt
            };

            return View(vm);
        }

        // POST: /Store/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, StoreFormViewModel model)
        {
            if (id != model.Id) return BadRequest();

            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var store = await _storeService.GetStoreByIdAsync(id);
                if (store == null) return NotFound();

                //Apply view model values to the store
                store.Name = model.Name.Trim();
                store.Address = model.Address.Trim();
                store.Description = model.Description.Trim();
                store.Rating = model.Rating;
                if(model.CreatedAt.HasValue)
                {
                    store.CreatedAt = model.CreatedAt.Value;
                }   
                var result = await _storeService.UpdateStoreAsync(store);
                if(!result) return NotFound();
                TempData["Success"] = "Store updated successfully!";
                
                return RedirectToAction(nameof(Details), new { id });

            }
            catch (ArgumentException)
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    ModelState.AddModelError("", error.ErrorMessage);
                }
                return View(model);
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
