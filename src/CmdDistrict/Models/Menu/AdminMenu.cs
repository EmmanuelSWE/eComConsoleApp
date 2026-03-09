using cmdDistrict.Models.Entities;

namespace cmdDistrict.Models;

public class AdminMenu : Menu
{
    public AdminMenu() { Key = "admin"; Title = "Administrator Menu"; }

    public override void PrintOptions()
    {
        Console.WriteLine("  1)  Add Product");
        Console.WriteLine("  2)  Update Product");
        Console.WriteLine("  3)  Delete Product");
        Console.WriteLine("  4)  Restock Product  (Adjust Inventory)");
        Console.WriteLine("  5)  View All Products");
        Console.WriteLine("  6)  View All Orders");
        Console.WriteLine("  7)  Update Order Status");
        Console.WriteLine("  8)  View Low-Stock Products  (stock < 5)");
        Console.WriteLine("  9)  Generate Sales Report");
        Console.WriteLine("  10) Logout");
    }

    public override bool HandleSelection(string input)
    {
        switch (input)
        {
            case "1":  DoAddProduct();    return true;
            case "2":  DoUpdateProduct(); return true;
            case "3":  DoDeleteProduct(); return true;
            case "4":  DoRestock();       return true;
            case "5":  DoViewProducts();  return true;
            case "6":  DoViewOrders();    return true;
            case "7":  DoUpdateStatus();  return true;
            case "8":  DoLowStock();      return true;
            case "9":  DoReport();        return true;
            case "10": DoLogout();        return true;
            default:   return false;
        }
    }

    // ── Actions ───────────────────────────────────────────────────────────────

    private void DoAddProduct()
    {
        Console.WriteLine("\n  -- Add Product --");
        Console.Write("  Name        : "); var name  = Console.ReadLine()?.Trim() ?? "";
        Console.Write("  Description : "); var desc  = Console.ReadLine()?.Trim() ?? "";
        Console.Write("  Price       : "); var pStr  = Console.ReadLine()?.Trim() ?? "";
        Console.Write("  Stock       : "); var sStr  = Console.ReadLine()?.Trim() ?? "";

        if (!decimal.TryParse(pStr, out var price) || !int.TryParse(sStr, out var stock))
        { Console.WriteLine("  [!] Invalid price or stock — price must be decimal, stock must be whole number."); return; }

        var product = Product.Create(ContextUserId!, name, desc, price, stock);
        if (product is not null)
            Console.WriteLine($"  [✓] Product created — ID: {product.Id}");
        else
            Console.WriteLine("  [!] Failed — ensure name is not empty and price/stock ≥ 0.");
    }

    private void DoUpdateProduct()
    {
        Console.WriteLine("\n  -- Update Product --");
        Console.Write("  Product ID  : "); var id   = Console.ReadLine()?.Trim() ?? "";
        Console.Write("  Name        : "); var name = Console.ReadLine()?.Trim() ?? "";
        Console.Write("  Description : "); var desc = Console.ReadLine()?.Trim() ?? "";
        Console.Write("  Price       : "); var pStr = Console.ReadLine()?.Trim() ?? "";
        Console.Write("  Stock       : "); var sStr = Console.ReadLine()?.Trim() ?? "";

        if (!decimal.TryParse(pStr, out var price) || !int.TryParse(sStr, out var stock))
        { Console.WriteLine("  [!] Invalid price or stock."); return; }

        if (Product.Update(ContextUserId!, id, name, desc, price, stock))
            Console.WriteLine("  [✓] Product updated.");
        else
            Console.WriteLine("  [!] Update failed — check product ID and input values.");
    }

    private void DoDeleteProduct()
    {
        Console.Write("\n  Product ID to delete: ");
        var id = Console.ReadLine()?.Trim() ?? "";

        if (Product.Delete(ContextUserId!, id))
            Console.WriteLine("  [✓] Product deleted.");
        else
            Console.WriteLine("  [!] Product not found.");
    }

    private void DoRestock()
    {
        Console.WriteLine("\n  -- Adjust Inventory --");
        Console.Write("  Product ID          : "); var id   = Console.ReadLine()?.Trim() ?? "";
        Console.Write("  Delta (+add/-remove): "); var dStr = Console.ReadLine()?.Trim() ?? "";

        if (!int.TryParse(dStr, out var delta))
        { Console.WriteLine("  [!] Delta must be a whole number (e.g. +10 or -3)."); return; }

        if (Administrator.AdjustInventory(ContextUserId!, id, delta))
            Console.WriteLine("  [✓] Inventory updated.");
        else
            Console.WriteLine("  [!] Failed — check product ID and ensure resulting stock ≥ 0.");
    }

    private void DoViewProducts()
    {
        var products = Product.SearchByName("");
        if (products.Count == 0) { Console.WriteLine("\n  No products in catalog."); return; }
        MainMenu.PrintProductTable(products);
    }

    private void DoViewOrders()
    {
        var orders = Administrator.ListAllOrders(ContextUserId!);
        if (orders.Count == 0) { Console.WriteLine("\n  No orders found."); return; }

        Console.WriteLine();
        Console.WriteLine($"  {"Date",-20} {"Customer ID",-36} {"Status",-12} {"Total",9}  {"Order ID"}");
        Console.WriteLine("  " + new string('-', 105));
        foreach (var o in orders)
            Console.WriteLine(
                $"  {o.CreatedAt:yyyy-MM-dd HH:mm,-20} {o.CustomerId,-36} {o.Status,-12} {o.Total,9:C}  {o.Id}");
    }

    private void DoUpdateStatus()
    {
        Console.WriteLine("\n  -- Update Order Status --");
        Console.Write("  Order ID                                              : "); var orderId   = Console.ReadLine()?.Trim() ?? "";
        Console.Write("  New status (Pending/Paid/Packed/Shipped/Delivered/Cancelled): ");
        var statusStr = Console.ReadLine()?.Trim() ?? "";

        if (!Enum.TryParse<OrderStatus>(statusStr, ignoreCase: true, out var status))
        { Console.WriteLine("  [!] Invalid status. Options: Pending, Paid, Packed, Shipped, Delivered, Cancelled."); return; }

        if (Order.UpdateStatus(ContextUserId!, orderId, status))
            Console.WriteLine($"  [✓] Order status updated to {status}.");
        else
            Console.WriteLine("  [!] Failed — invalid transition or order not found.");
    }

    private void DoLowStock()
    {
        var low = Product.SearchByName("").Where(p => p.Stock < 5).OrderBy(p => p.Stock).ToList();
        if (low.Count == 0) { Console.WriteLine("\n  All products have sufficient stock (≥ 5)."); return; }

        Console.WriteLine("\n  -- Low-Stock Products (stock < 5) --");
        MainMenu.PrintProductTable(low);
    }

    private void DoReport()
    {
        Console.WriteLine("\n  -- Generate Sales Report --");
        Console.Write("  From (yyyy-MM-dd): "); var fromStr = Console.ReadLine()?.Trim() ?? "";
        Console.Write("  To   (yyyy-MM-dd): "); var toStr   = Console.ReadLine()?.Trim() ?? "";

        if (!DateTime.TryParse(fromStr, out var from) || !DateTime.TryParse(toStr, out var to))
        { Console.WriteLine("  [!] Invalid date format — use yyyy-MM-dd."); return; }

        // Include the full final day
        var report = Administrator.GenerateReport(ContextUserId!, from, to.AddDays(1).AddSeconds(-1));
        Console.WriteLine();
        Console.WriteLine(report);
    }

    private void DoLogout()
    {
        User.Logout();
        Console.WriteLine("  [✓] Logged out successfully.");
        GlobalMenuHolder.SignOut();
    }
}
