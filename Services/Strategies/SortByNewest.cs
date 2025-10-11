using MatchaReviewApp.Interfaces;
using MatchaReviewApp.Models;
namespace MatchaReviewApp.Services.Strategies
{
    public class SortByNewest : ISortStrategy
    {
        public IEnumerable<Store> Sort(IEnumerable<Store> stores)
        {
            // Lambda expression
            return stores.OrderByDescending(s => s.CreatedAt);
        }
    }
}
