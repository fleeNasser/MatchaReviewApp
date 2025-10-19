using MatchaReviewApp.Models;

namespace MatchaReviewApp.Interfaces
{
    /// Strategy interface demonstrating polymorphism.
    /// Different sorting implementations can be swapped at runtime.
    public interface ISortStrategy
    {
        IEnumerable<Store> Sort(IEnumerable<Store> stores);
    }
}
