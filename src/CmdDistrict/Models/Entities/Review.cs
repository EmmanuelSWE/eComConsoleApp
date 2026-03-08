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

    public string   Id         => _id;
    public string   ProductId  => _productId;
    public string   CustomerId => _customerId;
    public int      Rating     => _rating;
    public string   Comment    => _comment;
    public DateTime CreatedAt  => _createdAt;
}
