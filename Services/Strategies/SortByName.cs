using MatchaReviewApp.Interfaces;
using MatchaReviewApp.Models;
namespace MatchaReviewApp.Services.Strategies
{
    public class SortByName : ISortStrategy
    {
        public IEnumerable<Store> Sort(IEnumerable<Store> stores)
        {
            return stores.OrderBy(s => s.Name);
        }
    }
}
