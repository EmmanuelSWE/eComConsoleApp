using cmdDistrict.DataAccess;
using cmdDistrict.Models.Entities;
namespace cmdDistrict.Infrastructure.Stores.Ef;

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
            if (!ctx.Users.Any(u => u.Id == userId && u.Role == "Administrator"))
            {
                Console.WriteLine($"Outcome : failed");
                return false;
            }
            ctx.Products.Add(product);
            ctx.SaveChanges();
            Console.WriteLine($"Outcome : passed");
            Console.WriteLine($"EntityMade : Product : {product.Id}");
            return true;
        }
        catch
        {
            Console.WriteLine($"Outcome : encountered an error");
            return false;
        }
    }

    /// <summary>
    /// Updates an existing product's fields (admin-only).
    /// </summary>
    public static bool Update(string userId, string id, string name, string description, decimal price, int stock)
    {
        try
        {
            using var ctx = new AppDbContext();
            if (!ctx.Users.Any(u => u.Id == userId && u.Role == "Administrator"))
            {
                Console.WriteLine($"Outcome : failed");
                return false;
            }
            var product = ctx.Products.SingleOrDefault(p => p.Id == id);
            if (product is null)
            {
                Console.WriteLine($"Outcome : failed");
                return false;
            }
            product.Name        = name;
            product.Description = description;
            product.Price       = price;
            product.Stock       = stock;
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

    /// <summary>
    /// Removes a product by id (admin-only).
    /// </summary>
    public static bool Delete(string userId, string id)
    {
        try
        {
            using var ctx = new AppDbContext();
            if (!ctx.Users.Any(u => u.Id == userId && u.Role == "Administrator"))
            {
                Console.WriteLine($"Outcome : failed");
                return false;
            }
            var product = ctx.Products.SingleOrDefault(p => p.Id == id);
            if (product is null)
            {
                Console.WriteLine($"Outcome : failed");
                return false;
            }
            ctx.Products.Remove(product);
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
            var result = ctx.Products.SingleOrDefault(p => p.Id == id);
            Console.WriteLine($"Outcome : {(result is null ? "failed" : "passed")}");
            return result;
        }
        catch
        {
            Console.WriteLine($"Outcome : encountered an error");
            return null;
        }
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
            var result = string.IsNullOrEmpty(q)
                ? ctx.Products.OrderBy(p => p.Name).ToList()
                : ctx.Products
                    .Where(p => p.Name.Contains(q))
                    .OrderBy(p => p.Name)
                    .ToList();
            Console.WriteLine($"Outcome : passed");
            return result;
        }
        catch
        {
            Console.WriteLine($"Outcome : encountered an error");
            return new List<Product>();
        }
    }
}
