namespace cmdDistrict.Models.Entities;

public class Cart
{
    private string         _id;
    private string         _customerId;
    private List<CartItem> _items;

    public Cart(string customerId)
    {
        _id         = Guid.NewGuid().ToString();
        _customerId = customerId;
        _items      = new List<CartItem>();
    }

    public string         Id         => _id;
    public string         CustomerId => _customerId;
    public List<CartItem> Items      => _items;
}
