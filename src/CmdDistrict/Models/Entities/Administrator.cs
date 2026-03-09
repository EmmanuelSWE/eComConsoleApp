using cmdDistrict.DataAccess;
using Microsoft.EntityFrameworkCore;


namespace cmdDistrict.Models.Entities;

public class Administrator : User
{
    private string _permissionLevel;

    public Administrator(string name, string email, string password,
                         string permissionLevel = "standard")
        : base(name, email, password, "Administrator")
    {
        _permissionLevel = permissionLevel;
    }

    public string PermissionLevel
    {
        get => _permissionLevel;
        set => _permissionLevel = value;
    }

    // ── Static actions ────────────────────────────────────────────────────────

    /// <summary>Adjusts a product's stock by delta. Returns true on success.</summary>
    public static bool AdjustInventory(string userId, string productId, int delta)
    {
        try
        {
            using var ctx = new AppDbContext();
            if (!ctx.Users.Any(u => u.Id == userId && u.Role == "Administrator")) return false;
            var product = ctx.Products.SingleOrDefault(p => p.Id == productId);
            if (product is null) return false;
            var newStock = product.Stock + delta;
            if (newStock < 0) return false;
            product.Stock = newStock;
            ctx.SaveChanges();
            return true;
        }
        catch { return false; }
    }


        public static bool AdjustInventory(string productId, int delta)
    {
        try
        {
            using var ctx = new AppDbContext();
            var product = ctx.Products.SingleOrDefault(p => p.Id == productId);
            if (product is null) return false;
            var newStock = product.Stock + delta;
            if (newStock < 0) return false;
            product.Stock = newStock;
            ctx.SaveChanges();
            return true;
        }
        catch { return false; }
    }

    /// <summary>Returns all orders newest-first. Empty list for non-admins.</summary>
    public static List<Order> ListAllOrders(string userId)
    {
        try
        {
            using var ctx = new AppDbContext();
            if (!ctx.Users.Any(u => u.Id == userId && u.Role == "Administrator"))
                return new List<Order>();
            return ctx.Orders.OrderByDescending(o => o.CreatedAt).ToList();
        }
        catch { return new List<Order>(); }
    }

    /// <summary>Generates a text sales report for the given date range.</summary>
    public static string GenerateReport(string userId, DateTime from, DateTime to)
    {
        try
        {
            using var ctx = new AppDbContext();
            if (!ctx.Users.Any(u => u.Id == userId && u.Role == "Administrator"))
                return "Forbidden";
            if (from > to) return "Invalid date range";

            var orders   = ctx.Orders.Include(o => o.Items).Where(o => o.CreatedAt >= from && o.CreatedAt <= to).ToList();
            var products = ctx.Products.ToList();
            var users    = ctx.Users.ToList();

            var totalRevenue   = orders.Sum(o => o.Total);
            var avgOrderValue  = orders.Count > 0 ? orders.Average(o => (double)o.Total) : 0.0;
            var avgPrice       = products.Count > 0 ? products.Average(p => (double)p.Price) : 0.0;
            var totalCustomers = users.Count(u => u.Role == "Customer");

            var topProducts = orders
                .SelectMany(o => o.Items)
                .GroupBy(i => i.ProductName)
                .Select(g => new { Name = g.Key, Qty = g.Sum(i => i.Quantity), Revenue = g.Sum(i => i.LineTotal) })
                .OrderByDescending(x => x.Revenue)
                .Take(5)
                .ToList();

            var sb = new System.Text.StringBuilder();
            sb.AppendLine($"  Sales Report : {from:yyyy-MM-dd} to {to:yyyy-MM-dd}");
            sb.AppendLine($"  {new string('-', 44)}");
            sb.AppendLine($"  Orders        : {orders.Count}");
            sb.AppendLine($"  Total Revenue : {totalRevenue:C}");
            sb.AppendLine($"  Avg Order     : {avgOrderValue:C}");
            sb.AppendLine($"  Customers     : {totalCustomers}");
            sb.AppendLine($"  Products      : {products.Count}  (avg price {avgPrice:C})");
            if (topProducts.Any())
            {
                sb.AppendLine("  Top Products :");
                foreach (var p in topProducts)
                    sb.AppendLine($"    • {p.Name,-24} qty={p.Qty,4}  rev={p.Revenue:C}");
            }
            return sb.ToString();
        }
        catch (Exception ex) { return $"Report error: {ex.Message}"; }
    }
}
