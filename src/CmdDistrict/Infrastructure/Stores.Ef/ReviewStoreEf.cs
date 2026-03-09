using cmdDistrict.DataAccess;
using cmdDistrict.Models.Entities;

namespace cmdDistrict.Infrastructure.Stores.Ef;

/// <summary>
/// EF Core LINQ-backed review store.
/// Submit is bool-only; GetForProduct and AverageRating are read-only queries.
/// </summary>
public static class ReviewStoreEf
{
    // ── Write ─────────────────────────────────────────────────────────────────

    /// <summary>Persists a new review (rating 1–5, one per customer per product).</summary>
    public static bool Submit(string userId, string productId, int rating, string comment)
    {
        try
        {
            if (rating < 1 || rating > 5)
            {
                Console.WriteLine($"Outcome : failed");
                return false;
            }
            using var ctx = new AppDbContext();
            if (!ctx.Products.Any(p => p.Id == productId))
            {
                Console.WriteLine($"Outcome : failed");
                return false;
            }
            if (ctx.Reviews.Any(r => r.ProductId == productId && r.CustomerId == userId))
            {
                Console.WriteLine($"Outcome : failed");
                return false;
            }

            var review = new Review(productId, userId, rating, comment);
            ctx.Reviews.Add(review);
            ctx.SaveChanges();
            Console.WriteLine($"Outcome : passed");
            Console.WriteLine($"EntityMade : Review : {review.Id}");
            return true;
        }
        catch
        {
            Console.WriteLine($"Outcome : encountered an error");
            return false;
        }
    }

    // ── Read ──────────────────────────────────────────────────────────────────

    /// <summary>Returns all reviews for a product, newest first.</summary>
    public static List<Review> GetForProduct(string productId)
    {
        try
        {
            using var ctx = new AppDbContext();
            var result = ctx.Reviews
                .Where(r => r.ProductId == productId)
                .OrderByDescending(r => r.CreatedAt)
                .ToList();
            Console.WriteLine($"Outcome : passed");
            return result;
        }
        catch
        {
            Console.WriteLine($"Outcome : encountered an error");
            return new List<Review>();
        }
    }

    /// <summary>Returns the average rating for a product (0 if none).</summary>
    public static double AverageRating(string productId)
    {
        try
        {
            using var ctx = new AppDbContext();
            var ratings = ctx.Reviews
                .Where(r => r.ProductId == productId)
                .Select(r => r.Rating)
                .ToList();
            var result = ratings.Count > 0 ? ratings.Average() : 0.0;
            Console.WriteLine($"Outcome : passed");
            return result;
        }
        catch
        {
            Console.WriteLine($"Outcome : encountered an error");
            return 0.0;
        }
    }
}
