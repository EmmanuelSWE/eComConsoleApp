using cmdDistrict.DataAccess;
using cmdDistrict.Models.Entities;

namespace cmdDistrict.Infrastructure.StoresEf;

/// <summary>
/// EF Core LINQ-backed product store.
/// </summary>
public static class ProductStoreEf
{
    // ── Write ─────────────────────────────────────────────────────────────────

    /// <summary>
    /// Persists a pre-built <paramref name="product"/> (admin-only).
    /// Returns false when the caller is not an admin or on any error.
    /// </summary>
    public static bool Create(string userId, Product product)
    {
        try
        {
            using var ctx = new AppDbContext();
            if (!ctx.Users.Any(u => u.Id == userId && u.Role == "Administrator")) return false;
            ctx.Products.Add(product);
            ctx.SaveChanges();
            return true;
        }
        catch { return false; }
    }

    /// <summary>
    /// Updates an existing product's fields (admin-only).
    /// </summary>
    public static bool Update(string userId, string id, string name, string description, decimal price, int stock)
    {
        try
        {
            using var ctx = new AppDbContext();
            if (!ctx.Users.Any(u => u.Id == userId && u.Role == "Administrator")) return false;
            var product = ctx.Products.SingleOrDefault(p => p.Id == id);
            if (product is null) return false;
            product.Name        = name;
            product.Description = description;
            product.Price       = price;
            product.Stock       = stock;
            ctx.SaveChanges();
            return true;
        }
        catch { return false; }
    }

    /// <summary>
    /// Removes a product by id (admin-only).
    /// </summary>
    public static bool Delete(string userId, string id)
    {
        try
        {
            using var ctx = new AppDbContext();
            if (!ctx.Users.Any(u => u.Id == userId && u.Role == "Administrator")) return false;
            var product = ctx.Products.SingleOrDefault(p => p.Id == id);
            if (product is null) return false;
            ctx.Products.Remove(product);
            ctx.SaveChanges();
            return true;
        }
        catch { return false; }
    }

    // ── Read ──────────────────────────────────────────────────────────────────
    // ── Read ──────────────────────────────────────────────────────────────────

    /// <summary>
    /// Returns the product with the given id, or null if not found.
    /// </summary>
    public static Product? FindById(string id)
    {
        try
        {
            using var ctx = new AppDbContext();
            return ctx.Products.SingleOrDefault(p => p.Id == id);
        }
        catch { return null; }
    }

    /// <summary>
    /// Returns all products whose name contains <paramref name="query"/> (case-insensitive).
    /// An empty/null query returns every product ordered by name.
    /// </summary>
    public static List<Product> SearchByName(string query)
    {
        try
        {
            using var ctx = new AppDbContext();
            var q = query?.Trim() ?? "";
            return string.IsNullOrEmpty(q)
                ? ctx.Products.OrderBy(p => p.Name).ToList()
                : ctx.Products
                    .Where(p => p.Name.Contains(q))
                    .OrderBy(p => p.Name)
                    .ToList();
        }
        catch { return new List<Product>(); }
    }
}
