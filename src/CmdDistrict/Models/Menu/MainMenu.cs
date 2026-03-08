using cmdDistrict.Models.Entities;

namespace cmdDistrict.Models;

public class MainMenu : Menu
{
    public MainMenu() { Key = "main"; Title = "Main Menu"; }

    public override void PrintOptions()
    {
        Console.WriteLine("  1) Register");
        Console.WriteLine("  2) Login");
        Console.WriteLine("  3) Browse Products (Guest)");
        Console.WriteLine("  4) Exit");
    }

    public override bool HandleSelection(string input)
    {
        switch (input)
        {
            case "1": DoRegister(); return true;
            case "2": DoLogin();    return true;
            case "3": DoBrowse();   return true;
            case "4":
                Console.WriteLine("  Goodbye!");
                Environment.Exit(0);
                return true;
            default:
                return false;
        }
    }

    // ── Helpers ──────────────────────────────────────────────────────────────

    private void DoRegister()
    {
        Console.WriteLine("\n  -- Register --");
        Console.Write("  Name                          : "); var name     = Console.ReadLine()?.Trim() ?? "";
        Console.Write("  Email                         : "); var email    = Console.ReadLine()?.Trim() ?? "";
        Console.Write("  Password (min 6 chars)        : "); var password = Console.ReadLine()?.Trim() ?? "";
        Console.Write("  Role (Customer/Administrator) : "); var role     = Console.ReadLine()?.Trim() ?? "";

        var (ok, userId) = User.Sign(name, email, password, role);
        if (ok)
        {
            Console.WriteLine("  [✓] Registration successful!");
            GlobalMenuHolder.SwitchToRole(userId);
        }
        else
        {
            Console.WriteLine("  [!] Registration failed.");
            Console.WriteLine("      • Email already in use, OR");
            Console.WriteLine("      • Password must be at least 6 characters, OR");
            Console.WriteLine("      • Role must be exactly 'Customer' or 'Administrator'.");
        }
    }

    private void DoLogin()
    {
        Console.WriteLine("\n  -- Login --");
        Console.Write("  Email    : "); var email    = Console.ReadLine()?.Trim() ?? "";
        Console.Write("  Password : "); var password = Console.ReadLine()?.Trim() ?? "";

        var (ok, userId, _) = User.Login(email, password);
        if (ok)
        {
            Console.WriteLine("  [✓] Login successful!");
            GlobalMenuHolder.SwitchToRole(userId);
        }
        else
        {
            Console.WriteLine("  [!] Invalid email or password.");
        }
    }

    private void DoBrowse()
    {
        Console.WriteLine("\n  -- Browse Products --");
        Console.Write("  Search (leave blank for all): ");
        var q        = Console.ReadLine()?.Trim() ?? "";
        var products = Product.SearchByName(q);

        if (products.Count == 0)
        {
            Console.WriteLine("  No products available.");
            return;
        }
        PrintProductTable(products);
    }

    // Shared by AdminMenu ────────────────────────────────────────────────────
    internal static void PrintProductTable(List<Product> products)
    {
        Console.WriteLine();
        Console.WriteLine($"  {"Name",-24} {"Price",9}  {"Stock",5}  {"ID"}");
        Console.WriteLine("  " + new string('-', 75));
        foreach (var p in products)
            Console.WriteLine($"  {p.Name,-24} {p.Price,9:C}  {p.Stock,5}  {p.Id}");
    }
}
