using cmdDistrict.DataAccess;
using Microsoft.EntityFrameworkCore;


namespace cmdDistrict.Models.Entities;

public class Customer : User
{
    private decimal _walletBalance;
    private string? _defaultShippingAddress;

    public Customer(string name, string email, string password)
        : base(name, email, password, "Customer")
    {
        _walletBalance          = 0m;
        _defaultShippingAddress = null;
    }

    public decimal WalletBalance
    {
        get => _walletBalance;
        set => _walletBalance = value;
    }

    public string? DefaultShippingAddress
    {
        get => _defaultShippingAddress;
        set => _defaultShippingAddress = value;
    }

    // ── Static actions ────────────────────────────────────────────────────────

    /// <summary>Deposits amount into the customer's wallet.</summary>
    public static bool Deposit(string userId, decimal amount)
    {
        try
        {
            if (amount <= 0) return false;
            using var ctx = new AppDbContext();
            var customer = ctx.Users.OfType<Customer>().SingleOrDefault(u => u.Id == userId);
            if (customer is null) return false;
            customer.WalletBalance += amount;
            ctx.SaveChanges();
            return true;
        }
        catch { return false; }
    }

    /// <summary>Returns all orders belonging to the customer, newest first.</summary>
    public static List<Order> ViewOrders(string userId)
    {
        try
        {
            using var ctx = new AppDbContext();
            return ctx.Orders
                .Where(o => o.CustomerId == userId)
                .OrderByDescending(o => o.CreatedAt)
                .ToList();
        }
        catch { return new List<Order>(); }
    }

    /// <summary>Returns the customer's cart, creating it if missing.</summary>
    public static Cart? ViewCart(string userId)
    {
        try
        {
            using var ctx = new AppDbContext();
            var cart = ctx.Carts.Include(c => c.Items).SingleOrDefault(c => c.CustomerId == userId);
            if (cart is null)
            {
                cart = new Cart(userId);
                ctx.Carts.Add(cart);
                ctx.SaveChanges();
            }
            return cart;
        }
        catch { return null; }
    }
}
