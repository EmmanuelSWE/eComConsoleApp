using cmdDistrict.DataAccess;
using cmdDistrict.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace cmdDistrict.Infrastructure.StoresEf;

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
            if (quantity <= 0) return false;
            using var ctx = new AppDbContext();
            var cart    = EnsureExists(ctx, userId);
            var product = ctx.Products.SingleOrDefault(p => p.Id == productId);
            if (product is null) return false;
            if (product.Stock < quantity) return false;

            product.Stock -= quantity;

            var existing = cart.Items.SingleOrDefault(i => i.ProductId == productId);
            if (existing is not null)
                existing.Quantity += quantity;
            else
                cart.Items.Add(new CartItem(productId, product.Name, product.Price, quantity));

            ctx.SaveChanges();
            return true;
        }
        catch { return false; }
    }

    /// <summary>Removes an item from the cart and restores product stock.</summary>
    public static bool RemoveItem(string userId, string productId)
    {
        try
        {
            using var ctx = new AppDbContext();
            var cart = ctx.Carts.Include(c => c.Items)
                                 .SingleOrDefault(c => c.CustomerId == userId);
            if (cart is null) return false;
            var item = cart.Items.SingleOrDefault(i => i.ProductId == productId);
            if (item is null) return false;

            var product = ctx.Products.SingleOrDefault(p => p.Id == productId);
            if (product is not null) product.Stock += item.Quantity;

            ctx.CartItems.Remove(item);
            ctx.SaveChanges();
            return true;
        }
        catch { return false; }
    }

    /// <summary>Removes all items from the cart, restoring stock for each.</summary>
    public static void Clear(string userId)
    {
        try
        {
            using var ctx = new AppDbContext();
            var cart = ctx.Carts.Include(c => c.Items)
                                 .SingleOrDefault(c => c.CustomerId == userId);
            if (cart is null) return;

            foreach (var item in cart.Items.ToList())
            {
                var product = ctx.Products.SingleOrDefault(p => p.Id == item.ProductId);
                if (product is not null) product.Stock += item.Quantity;
            }
            ctx.CartItems.RemoveRange(cart.Items);
            ctx.SaveChanges();
        }
        catch { /* swallow */ }
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
            return cart?.Items.Sum(i => i.UnitPrice * i.Quantity) ?? 0m;
        }
        catch { return 0m; }
    }

    /// <summary>Returns the customer's cart with items, or null when none exists.</summary>
    public static Cart? GetByCustomerId(string userId)
    {
        try
        {
            using var ctx = new AppDbContext();
            return ctx.Carts.Include(c => c.Items)
                             .SingleOrDefault(c => c.CustomerId == userId);
        }
        catch { return null; }
    }
}
