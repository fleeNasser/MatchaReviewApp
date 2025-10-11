using MatchaReviewApp.Data;
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
            // Pass a new Review object with StoreId set for the form
            return View(new Review { StoreId = storeId });
        }

        // POST: /Review/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Review review)
        {
            // Set the current user ID
            review.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Defensive: Ensure StoreId is valid and user hasn't already reviewed
            var store = await _storeService.GetStoreByIdAsync(review.StoreId);
            if (store == null)
            {
                ModelState.AddModelError("", "Store not found.");
                return View(review);
            }

            var hasReviewed = await _reviewService.UserHasReviewedStoreAsync(review.UserId, review.StoreId);
            if (hasReviewed)
            {
                TempData["Error"] = "You have already reviewed this store.";
                return RedirectToAction("Details", "Store", new { id = review.StoreId });
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Store = store;
                return View(review);
            }

            try
            {
                review.CreatedAt = DateTime.Now;
                await _reviewService.AddReviewAsync(review);
                TempData["Success"] = "Review submitted successfully!";
                return RedirectToAction("Details", "Store", new { id = review.StoreId });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while submitting your review: " + ex.Message);
                ViewBag.Store = store;
                return View(review);
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