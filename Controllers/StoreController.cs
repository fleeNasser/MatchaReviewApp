using MatchaReviewApp.Interfaces;
using MatchaReviewApp.Models;
using MatchaReviewApp.Services.Strategies;
using MatchaReviewApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Linq;

namespace MatchaReviewApp.Controllers
{
    /// Store controller handling HTTP requests.
    public class StoreController : Controller
    {
        private readonly IStoreService _storeService;
        private readonly IReviewService _reviewService;
        private readonly IWebHostEnvironment _env;

        // Constructor Injection (Dependency Injection)
        public StoreController(
            IStoreService storeService,
            IReviewService reviewService,
            IWebHostEnvironment env)
        {
            _storeService = storeService;
            _reviewService = reviewService;
            _env = env;
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

                // Handle image upload if provided
                if (model.Image != null && model.Image.Length > 0)
                {
                    var savedPath = await SaveImageAsync(model.Image);
                    if (savedPath == null)
                    {
                        ModelState.AddModelError("Image", "Failed to save image.");
                        return View(model);
                    }
                    store.ImagePath = savedPath;
                }

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
                CreatedAt = store.CreatedAt,
                ExistingImagePath = store.ImagePath
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

                var oldImagePath = store.ImagePath;

                // Apply view model values to the store
                store.Name = model.Name.Trim();
                store.Address = model.Address.Trim();
                store.Description = model.Description.Trim();
                store.Rating = model.Rating;
                if (model.CreatedAt.HasValue)
                {
                    store.CreatedAt = model.CreatedAt.Value;
                }

                // If a new image was uploaded -> save and replace
                if (model.Image != null && model.Image.Length > 0)
                {
                    var savedPath = await SaveImageAsync(model.Image);
                    if (savedPath == null)
                    {
                        ModelState.AddModelError("Image", "Failed to save image.");
                        return View(model);
                    }
                    store.ImagePath = savedPath;
                }
                else if (model.RemoveImage)
                {
                    // remove existing reference
                    store.ImagePath = null;
                }

                var result = await _storeService.UpdateStoreAsync(store);
                if (!result) return NotFound();

                // If update succeeded, delete old image file if it was replaced or removed
                if (!string.IsNullOrEmpty(oldImagePath))
                {
                    // deleted when replaced or explicitly removed
                    if ((store.ImagePath == null) || (store.ImagePath != null && store.ImagePath != oldImagePath))
                    {
                        TryDeleteFileQuietly(oldImagePath);
                    }
                }

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
            // fetch the store first to know image to delete after successful DB delete
            var store = await _storeService.GetStoreByIdAsync(id);
            if (store == null) return NotFound();

            var result = await _storeService.DeleteStoreAsync(id);
            if (!result) return NotFound();

            // Attempt to delete associated image file (non-fatal)
            if (!string.IsNullOrEmpty(store.ImagePath))
            {
                TryDeleteFileQuietly(store.ImagePath);
            }

            TempData["Success"] = "Store deleted successfully!";
            return RedirectToAction(nameof(Index));
        }

        // Saves uploaded image to wwwroot/uploads/stores and returns relative URL or null on failure
        private async Task<string?> SaveImageAsync(IFormFile image)
        {
            var allowed = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            var ext = Path.GetExtension(image.FileName).ToLowerInvariant();
            const long maxBytes = 2 * 1024 * 1024; // 2 MB

            if (!allowed.Contains(ext))
                return null;

            if (image.Length > maxBytes)
                return null;

            var uploadsFolder = Path.Combine(_env.WebRootPath ?? "wwwroot", "uploads", "stores");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var fileName = $"{Guid.NewGuid()}{ext}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            try
            {
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                }

                // return a web-relative path
                return $"/uploads/stores/{fileName}";
            }
            catch
            {
                return null;
            }
        }

        // Try delete file quietly (non-throwing)
        private void TryDeleteFileQuietly(string relativeUrlPath)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(relativeUrlPath)) return;

                // ensure relative path points inside wwwroot
                var trimmed = relativeUrlPath.TrimStart('~').TrimStart('/');
                var physical = Path.Combine(_env.WebRootPath ?? "wwwroot", trimmed.Replace('/', Path.DirectorySeparatorChar));
                if (System.IO.File.Exists(physical))
                {
                    System.IO.File.Delete(physical);
                }
            }
            catch
            {
                // swallow - non-fatal cleanup
            }
        }
    }
}