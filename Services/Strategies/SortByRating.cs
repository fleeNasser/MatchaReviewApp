using MatchaReviewApp.Interfaces;
using MatchaReviewApp.Models;

namespace MatchaReviewApp.Services.Strategies

{
    public class SortByRating : ISortStrategy
    {
        public IEnumerable<Store> Sort(IEnumerable<Store> stores)
        {
            // Lambda expression
            return stores.OrderByDescending(s => s.Rating);
        }
    }
}
