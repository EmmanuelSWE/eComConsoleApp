using cmdDistrict.DataAccess;
using cmdDistrict.Models.Entities;

namespace cmdDistrict.Infrastructure.StoresEf;

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
            if (rating < 1 || rating > 5) return false;
            using var ctx = new AppDbContext();
            if (!ctx.Products.Any(p => p.Id == productId)) return false;
            if (ctx.Reviews.Any(r => r.ProductId == productId && r.CustomerId == userId)) return false;

            ctx.Reviews.Add(new Review(productId, userId, rating, comment));
            ctx.SaveChanges();
            return true;
        }
        catch { return false; }
    }

    // ── Read ──────────────────────────────────────────────────────────────────

    /// <summary>Returns all reviews for a product, newest first.</summary>
    public static List<Review> GetForProduct(string productId)
    {
        try
        {
            using var ctx = new AppDbContext();
            return ctx.Reviews
                .Where(r => r.ProductId == productId)
                .OrderByDescending(r => r.CreatedAt)
                .ToList();
        }
        catch { return new List<Review>(); }
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
            return ratings.Count > 0 ? ratings.Average() : 0.0;
        }
        catch { return 0.0; }
    }
}
