using cmdDistrict.Common;

namespace cmdDistrict.Models.Entities;

public class Review
{
    private string   _id;
    private string   _productId;
    private string   _customerId;
    private int      _rating;
    private string   _comment;
    private DateTime _createdAt;

    public Review(string productId, string customerId, int rating, string comment)
    {
        _id         = Guid.NewGuid().ToString();
        _productId  = productId;
        _customerId = customerId;
        _rating     = rating;
        _comment    = comment;
        _createdAt  = DateProvider.UtcNow;
    }

    public string   Id         { get => _id;         set => _id         = value; }
    public string   ProductId  { get => _productId;  set => _productId  = value; }
    public string   CustomerId { get => _customerId; set => _customerId = value; }
    public int      Rating     { get => _rating;     set => _rating     = value; }
    public string   Comment    { get => _comment;    set => _comment    = value; }
    public DateTime CreatedAt  { get => _createdAt;  set => _createdAt  = value; }

    // ── Static actions ────────────────────────────────────────────────────

    /// <summary>Submits a product review (rating 1–5). One review per customer per product.</summary>
    public static bool Submit(string userId, string productId, int rating, string comment)
    {
        try
        {
            if (rating < 1 || rating > 5) return false;
            if (AppState.Products.All(p => p.Id != productId)) return false;
            // prevent duplicate reviews
            if (AppState.Reviews.Any(r => r.ProductId == productId && r.CustomerId == userId)) return false;

            var review = new Review(productId, userId, rating, comment);
            AppState.Reviews.Add(review);
            return true;
        }
        catch { return false; }
    }

    /// <summary>Returns all reviews for a product, newest first.</summary>
    public static List<Review> GetForProduct(string productId)
    {
        try
        {
            return AppState.Reviews
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
            var ratings = AppState.Reviews.Where(r => r.ProductId == productId).Select(r => r.Rating).ToList();
            return ratings.Any() ? ratings.Average() : 0.0;
        }
        catch { return 0.0; }
    }
}
