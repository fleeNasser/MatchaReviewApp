namespace MatchaReviewApp.Models
{
    public class Review : BaseEntity
    {
        public int StoreId { get; set; }
        public string UserId { get; set; }
        public string Comment { get; set; }
        public int Rating { get; set; }

        // Navigation properties
        public virtual Store Store { get; set; }
        public virtual User User { get; set; }
    }
}
