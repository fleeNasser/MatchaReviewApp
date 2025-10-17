using System;
using System.ComponentModel.DataAnnotations;
using MatchaReviewApp.Models;

namespace MatchaReviewApp.ViewModels
{
    /// ViewModel for create/edit Store. Performs validation and provides conversion helpers
    /// so controllers only deal with the VM and service calls.
    public class StoreFormViewModel
    {
        public int? Id { get; set; }

        [Required(ErrorMessage = "Store name is required.")]
        [StringLength(200, ErrorMessage = "Store name cannot exceed 200 characters.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Address is required.")]
        [StringLength(300, ErrorMessage = "Address cannot exceed 300 characters.")]
        public string Address { get; set; } = string.Empty;

        [Range(0, 5, ErrorMessage = "Rating must be between 0 and 5.")]
        public decimal Rating { get; set; } = 0m;

        [StringLength(2000, ErrorMessage = "Description cannot exceed 2000 characters.")]
        public string Description { get; set; } = string.Empty;

        // Optional: keep CreatedAt for edit scenarios so views can display/preserve it
        public DateTime? CreatedAt { get; set; }

        // Build a new Store entity from the VM (use for Create)
        public Store ToModel()
        {
            return new Store
            {
                Id = Id ?? 0,
                Name = Name?.Trim() ?? string.Empty,
                Address = Address?.Trim() ?? string.Empty,
                Rating = Rating,
                Description = Description?.Trim() ?? string.Empty,
                CreatedAt = CreatedAt ?? DateTime.UtcNow
            };
        }

        // Apply VM values to an existing Store entity (use for Edit)
        public void ApplyTo(Store store)
        {
            if (store == null) throw new ArgumentNullException(nameof(store));

            store.Name = Name?.Trim() ?? store.Name;
            store.Address = Address?.Trim() ?? store.Address;
            store.Rating = Rating;
            store.Description = Description?.Trim() ?? store.Description;

            if (CreatedAt.HasValue)
            {
                store.CreatedAt = CreatedAt.Value;
            }
        }

        // Create a populated VM from an existing Store (use in Edit GET)
        public static StoreFormViewModel FromModel(Store store)
        {
            if (store == null) throw new ArgumentNullException(nameof(store));

            return new StoreFormViewModel
            {
                Id = store.Id,
                Name = store.Name,
                Address = store.Address,
                Rating = store.Rating,
                Description = store.Description,
                CreatedAt = store.CreatedAt
            };
        }
    }
}
