using cmdDistrict.Common;

namespace cmdDistrict.Models.Entities;

public class Product
{
    private string  _id;
    private string  _name;
    private string  _description;
    private decimal _price;
    private int     _stock;

    public Product(string name, string description, decimal price, int stock)
    {
        _id          = Guid.NewGuid().ToString();
        _name        = name;
        _description = description;
        _price       = price;
        _stock       = stock;
    }

    public string  Id          { get => _id;          set => _id          = value; }
    public string  Name        { get => _name;        set => _name        = value; }
    public string  Description { get => _description; set => _description = value; }
    public decimal Price       { get => _price;       set => _price       = value; }
    public int     Stock       { get => _stock;       set => _stock       = value; }

    // ── Static actions ────────────────────────────────────────────────────────

    /// <summary>Creates and stores a new product. Returns null on failure.</summary>
    public static Product? Create(string userId, string name, string description, decimal price, int stock)
    {
        try
        {
            if (!AppState.Users.Any(u => u.Id == userId && u.Role == "Administrator")) return null;
            if (string.IsNullOrWhiteSpace(name) || price < 0 || stock < 0) return null;
            var product = new Product(name, description, price, stock);
            AppState.Products.Add(product);
            return product;
        }
        catch { return null; }
    }

    /// <summary>Updates an existing product's fields. Returns false on failure.</summary>
    public static bool Update(string userId, string id, string name, string description, decimal price, int stock)
    {
        try
        {
            if (!AppState.Users.Any(u => u.Id == userId && u.Role == "Administrator")) return false;
            var product = AppState.Products.SingleOrDefault(p => p.Id == id);
            if (product is null) return false;
            if (string.IsNullOrWhiteSpace(name) || price < 0 || stock < 0) return false;
            product.Name        = name;
            product.Description = description;
            product.Price       = price;
            product.Stock       = stock;
            return true;
        }
        catch { return false; }
    }

    /// <summary>Removes a product by id. Returns false on failure.</summary>
    public static bool Delete(string userId, string id)
    {
        try
        {
            if (!AppState.Users.Any(u => u.Id == userId && u.Role == "Administrator")) return false;
            var product = AppState.Products.SingleOrDefault(p => p.Id == id);
            if (product is null) return false;
            AppState.Products.Remove(product);
            return true;
        }
        catch { return false; }
    }

    /// <summary>Finds a product by id. Returns null if not found.</summary>
    public static Product? FindById(string id)
    {
        try { return AppState.Products.SingleOrDefault(p => p.Id == id); }
        catch { return null; }
    }

    /// <summary>Returns all products whose name contains the query (case-insensitive). Empty query returns all.</summary>
    public static List<Product> SearchByName(string query)
    {
        try
        {
            var q = query?.Trim() ?? "";
            return string.IsNullOrEmpty(q)
                ? AppState.Products.OrderBy(p => p.Name).ToList()
                : AppState.Products
                    .Where(p => p.Name.Contains(q, StringComparison.OrdinalIgnoreCase))
                    .OrderBy(p => p.Name)
                    .ToList();
        }
        catch { return new List<Product>(); }
    }
}
