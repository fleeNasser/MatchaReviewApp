namespace MatchaReviewApp.Models
{
    public class Store : BaseEntity
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public decimal Rating { get; set; }
        public string Description { get; set; }

        // Navigation property
        public virtual ICollection<Review> Reviews { get; set; }
    }
}
