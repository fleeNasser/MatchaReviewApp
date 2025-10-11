using MatchaReviewApp.Models;

namespace MatchaReviewApp.Interfaces
{
    /// <summary>
    /// Strategy interface demonstrating polymorphism.
    /// Different sorting implementations can be swapped at runtime.
    /// </summary>
    public interface ISortStrategy
    {
        IEnumerable<Store> Sort(IEnumerable<Store> stores);
    }
}
