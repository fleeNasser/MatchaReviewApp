using MatchaReviewApp.Data;
using MatchaReviewApp.ViewModels;
using MatchaReviewApp.Interfaces;
using MatchaReviewApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MatchaReviewApp.Controllers
{
    [Authorize]
    public class ReviewController : Controller
    {
        private readonly IReviewService _reviewService;
        private readonly IStoreService _storeService;
        private readonly UserManager<User> _userManager;

        public ReviewController(
            IReviewService reviewService,
            IStoreService storeService,
            UserManager<User> userManager)
        {
            _reviewService = reviewService;
            _storeService = storeService;
            _userManager = userManager;
        }

        // GET: /Review/Create?storeId=5
        [HttpGet]
        public async Task<IActionResult> Create(int storeId)
        {
            var store = await _storeService.GetStoreByIdAsync(storeId);
            if (store == null) return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var hasReviewed = await _reviewService.UserHasReviewedStoreAsync(userId, storeId);

            if (hasReviewed)
            {
                TempData["Error"] = "You have already reviewed this store.";
                return RedirectToAction("Details", "Store", new { id = storeId });
            }

            ViewBag.Store = store;
            return View(new ReviewFormViewModel { StoreId = storeId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ReviewFormViewModel model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var store = await _storeService.GetStoreByIdAsync(model.StoreId);

            if (store == null)
            {
                ModelState.AddModelError("", "Store not found.");
            }

            if (string.IsNullOrEmpty(userId))
            {
                ModelState.AddModelError("", "User is not authenticated.");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Store = store ?? new Store();
                return View(model);
            }

            var hasReviewed = await _reviewService.UserHasReviewedStoreAsync(userId, model.StoreId);
            if (hasReviewed)
            {
                TempData["Error"] = "You have already reviewed this store.";
                return RedirectToAction("Details", "Store", new { id = model.StoreId });
            }

            try
            {
                var review = new Review
                {
                    StoreId = model.StoreId,
                    UserId = userId,
                    Rating = model.Rating,
                    Comment = model.Comment,
                    CreatedAt = DateTime.Now
                };

                await _reviewService.AddReviewAsync(review);
                TempData["Success"] = "Review submitted successfully!";
                return RedirectToAction("Details", "Store", new { id = model.StoreId });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while submitting your review: " + ex.Message);
                ViewBag.Store = store ?? new Store();
                return View(model);
            }
        }

        // GET: /Review/MyReviews
        public async Task<IActionResult> MyReviews()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);
            var reviews = await _reviewService.GetReviewsByUserAsync(userId);

            ViewBag.UserFullName = user?.FullName;
            return View(reviews);
        }
    }
}