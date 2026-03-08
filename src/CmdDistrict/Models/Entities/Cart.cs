using cmdDistrict.Infrastructure.StoresEf;

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

    public string         Id         { get => _id;         set => _id         = value; }
    public string         CustomerId { get => _customerId; set => _customerId = value; }
    public List<CartItem> Items      { get => _items;      set => _items      = value; }

    // ── Static actions ────────────────────────────────────────────────────

    /// <summary>Adds qty of the given product to the user's cart, decrementing stock immediately.</summary>
    public static bool AddItem(string userId, string productId, int quantity)
        => CartStoreEf.AddItem(userId, productId, quantity);

    /// <summary>Removes an item from the cart and restores product stock.</summary>
    public static bool RemoveItem(string userId, string productId)
        => CartStoreEf.RemoveItem(userId, productId);

    /// <summary>Removes all items from the cart and restores their stock.</summary>
    public static void Clear(string userId)
        => CartStoreEf.Clear(userId);

    /// <summary>Returns the sum of UnitPrice * Quantity for the user's cart.</summary>
    public static decimal GetTotal(string userId)
        => CartStoreEf.GetTotal(userId);
}
