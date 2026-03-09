using cmdDistrict.Common;
using cmdDistrict.Infrastructure.Stores.Ef;

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
            if (string.IsNullOrWhiteSpace(name) || price < 0 || stock < 0) return null;
            var product = new Product(name, description, price, stock);
            return ProductStoreEf.Create(userId, product) ? product : null;
        }
        catch { return null; }
    }

    /// <summary>Updates an existing product's fields. Returns false on failure.</summary>
    public static bool Update(string userId, string id, string name, string description, decimal price, int stock)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(name) || price < 0 || stock < 0) return false;
            return ProductStoreEf.Update(userId, id, name, description, price, stock);
        }
        catch { return false; }
    }

    /// <summary>Removes a product by id. Returns false on failure.</summary>
    public static bool Delete(string userId, string id)
    {
        try { return ProductStoreEf.Delete(userId, id); }
        catch { return false; }
    }

    /// <summary>Finds a product by id. Returns null if not found.</summary>
    public static Product? FindById(string id)
    {
        try { return ProductStoreEf.FindById(id); }
        catch { return null; }
    }

    /// <summary>Returns all products whose name contains the query (case-insensitive). Empty query returns all.</summary>
    public static List<Product> SearchByName(string query)
    {
        try { return ProductStoreEf.SearchByName(query); }
        catch { return new List<Product>(); }
    }
}
