using cmdDistrict.DataAccess;
using cmdDistrict.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace cmdDistrict.Infrastructure.Stores.Ef;

/// <summary>
/// EF Core LINQ-backed cart store.
/// All public write methods return bool; GetTotal returns decimal.
/// EnsureExists auto-creates a cart row when missing (happy-path guard).
/// </summary>
public static class CartStoreEf
{
    // ── Private helper ────────────────────────────────────────────────────────

    /// <summary>Returns the customer's cart (with Items), creating one if absent.</summary>
    private static Cart EnsureExists(AppDbContext ctx, string userId)
    {
        var cart = ctx.Carts.Include(c => c.Items)
                             .SingleOrDefault(c => c.CustomerId == userId);
        if (cart is null)
        {
            cart = new Cart(userId);
            ctx.Carts.Add(cart);
            ctx.SaveChanges();
        }
        return cart;
    }

    // ── Write ─────────────────────────────────────────────────────────────────

    /// <summary>Adds qty of <paramref name="productId"/> to the user's cart, decrementing stock.</summary>
    public static bool AddItem(string userId, string productId, int quantity)
    {
        try
        {
            if (quantity <= 0)
            {
                Console.WriteLine($"Outcome : failed");
                return false;
            }
            using var ctx = new AppDbContext();
            var cart    = EnsureExists(ctx, userId);
            var product = ctx.Products.SingleOrDefault(p => p.Id == productId);
            if (product is null)
            {
                Console.WriteLine($"Outcome : failed");
                return false;
            }
            if (product.Stock < quantity)
            {
                Console.WriteLine($"Outcome : failed");
                return false;
            }

            product.Stock -= quantity;

            Administrator.AdjustInventory(productId, -quantity); // for demo visibility only; not required for correctness

            var existing = cart.Items.SingleOrDefault(i => i.ProductId == productId);
            if (existing is not null)
                existing.Quantity += quantity;
            else
                cart.Items.Add(new CartItem(productId, product.Name, product.Price, quantity));

            ctx.SaveChanges();
            Console.WriteLine($"Outcome : passed");
            return true;
        }
        catch
        {
            Console.WriteLine($"Outcome : encountered an error");
            return false;
        }
    }

    /// <summary>Removes an item from the cart and restores product stock.</summary>
    public static bool RemoveItem(string userId, string productId)
    {
        try
        {
            using var ctx = new AppDbContext();
            var cart = ctx.Carts.Include(c => c.Items)
                                 .SingleOrDefault(c => c.CustomerId == userId);
            if (cart is null)
            {
                Console.WriteLine($"Outcome : failed");
                return false;
            }
            var item = cart.Items.SingleOrDefault(i => i.ProductId == productId);
            if (item is null)
            {
                Console.WriteLine($"Outcome : failed");
                return false;
            }

            var product = ctx.Products.SingleOrDefault(p => p.Id == productId);
            if (product is not null) product.Stock += item.Quantity;

            ctx.CartItems.Remove(item);
            ctx.SaveChanges();
            Console.WriteLine($"Outcome : passed");
            return true;
        }
        catch
        {
            Console.WriteLine($"Outcome : encountered an error");
            return false;
        }
    }

    /// <summary>Removes all items from the cart, restoring stock for each.</summary>
    public static void Clear(string userId)
    {
        try
        {
            using var ctx = new AppDbContext();
            var cart = ctx.Carts.Include(c => c.Items)
                                 .SingleOrDefault(c => c.CustomerId == userId);
            if (cart is null)
            {
                Console.WriteLine($"Outcome : failed");
                return;
            }

            foreach (var item in cart.Items.ToList())
            {
                var product = ctx.Products.SingleOrDefault(p => p.Id == item.ProductId);
                if (product is not null) product.Stock += item.Quantity;
            }
            ctx.CartItems.RemoveRange(cart.Items);
            ctx.SaveChanges();
            Console.WriteLine($"Outcome : passed");
        }
        catch
        {
            Console.WriteLine($"Outcome : encountered an error");
            /* swallow */
        }
    }

    // ── Read ──────────────────────────────────────────────────────────────────

    /// <summary>Returns the price total for the user's cart (0 when no cart exists).</summary>
    public static decimal GetTotal(string userId)
    {
        try
        {
            using var ctx = new AppDbContext();
            var cart = ctx.Carts.Include(c => c.Items)
                                 .SingleOrDefault(c => c.CustomerId == userId);
            var result = cart?.Items.Sum(i => i.UnitPrice * i.Quantity) ?? 0m;
            Console.WriteLine($"Outcome : passed");
            return result;
        }
        catch
        {
            Console.WriteLine($"Outcome : encountered an error");
            return 0m;
        }
    }

    /// <summary>Returns the customer's cart with items, or null when none exists.</summary>
    public static Cart? GetByCustomerId(string userId)
    {
        try
        {
            using var ctx = new AppDbContext();
            var result = ctx.Carts.Include(c => c.Items)
                             .SingleOrDefault(c => c.CustomerId == userId);
            Console.WriteLine($"Outcome : {(result is null ? "failed" : "passed")}");
            return result;
        }
        catch
        {
            Console.WriteLine($"Outcome : encountered an error");
            return null;
        }
    }
}
