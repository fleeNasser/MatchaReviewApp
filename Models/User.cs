using Microsoft.AspNetCore.Identity;

namespace MatchaReviewApp.Models
{
    public class User : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime JoinDate { get; set; }

        // Navigation property for one-to-many relationship
        public virtual ICollection<Review> Reviews { get; set; }

        // Computed property for display
        public string FullName => $"{FirstName} {LastName}";
    }
}
