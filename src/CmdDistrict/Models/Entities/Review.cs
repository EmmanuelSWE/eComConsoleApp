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

    // ── Static actions ────────────────────────────────────────────────────────

    /// <summary>Submits a review for a product. Rating must be 1–5.</summary>
    public static bool Submit(string userId, string productId, int rating, string comment)
    {
        try
        {
            if (rating < 1 || rating > 5) return false;
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

    /// <summary>Returns the average rating for a product, or 0 if no reviews.</summary>
    public static double AverageRating(string productId)
    {
        try
        {
            var ratings = AppState.Reviews
                .Where(r => r.ProductId == productId)
                .Select(r => r.Rating)
                .ToList();
            return ratings.Count > 0 ? ratings.Average() : 0.0;
        }
        catch { return 0.0; }
    }
}
