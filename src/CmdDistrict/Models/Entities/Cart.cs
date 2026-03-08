using cmdDistrict.Common;

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
    {
        try
        {
            if (quantity <= 0) return false;
            var cart = AppState.Carts.SingleOrDefault(c => c.CustomerId == userId);
            if (cart is null) return false;
            var product = AppState.Products.SingleOrDefault(p => p.Id == productId);
            if (product is null) return false;
            if (product.Stock < quantity) return false;

            product.Stock -= quantity;

            var existing = cart.Items.SingleOrDefault(i => i.ProductId == productId);
            if (existing is not null)
                existing.Quantity += quantity;
            else
                cart.Items.Add(new CartItem(productId, product.Name, product.Price, quantity));

            return true;
        }
        catch { return false; }
    }

    /// <summary>Removes an item from the cart and restores product stock.</summary>
    public static bool RemoveItem(string userId, string productId)
    {
        try
        {
            var cart = AppState.Carts.SingleOrDefault(c => c.CustomerId == userId);
            if (cart is null) return false;
            var item = cart.Items.SingleOrDefault(i => i.ProductId == productId);
            if (item is null) return false;

            var product = AppState.Products.SingleOrDefault(p => p.Id == productId);
            if (product is not null) product.Stock += item.Quantity;

            cart.Items.Remove(item);
            return true;
        }
        catch { return false; }
    }

    /// <summary>Removes all items from the cart and restores their stock.</summary>
    public static void Clear(string userId)
    {
        try
        {
            var cart = AppState.Carts.SingleOrDefault(c => c.CustomerId == userId);
            if (cart is null) return;
            foreach (var item in cart.Items.ToList())
            {
                var product = AppState.Products.SingleOrDefault(p => p.Id == item.ProductId);
                if (product is not null) product.Stock += item.Quantity;
                cart.Items.Remove(item);
                Console.WriteLine($"  Removed {item.ProductName} from cart.");
            }
        }
        catch { /* swallow */ }
    }

    /// <summary>Returns the sum of UnitPrice * Quantity for the user's cart.</summary>
    public static decimal GetTotal(string userId)
    {
        try
        {
            var cart = AppState.Carts.SingleOrDefault(c => c.CustomerId == userId);
            return cart?.Items.Sum(i => i.UnitPrice * i.Quantity) ?? 0m;
        }
        catch { return 0m; }
    }
}
