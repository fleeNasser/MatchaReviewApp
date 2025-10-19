using MatchaReviewApp.Interfaces;
using MatchaReviewApp.Models;

namespace MatchaReviewApp.Services.Strategies

{
    public class SortByRating : ISortStrategy
    {
        public IEnumerable<Store> Sort(IEnumerable<Store> stores)
        {
            return stores.OrderByDescending(s => s.Rating);
        }
    }
}
