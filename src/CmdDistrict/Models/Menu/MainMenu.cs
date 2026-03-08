using cmdDistrict.Common;
using cmdDistrict.Models.Entities;

namespace cmdDistrict.Models;

public class MainMenu : Menu
{
    public MainMenu() { Key = "main"; Title = "Cmd District — Main Menu"; }

    public override void PrintOptions()
    {
        Console.WriteLine("  1) Register");
        Console.WriteLine("  2) Login");
        Console.WriteLine("  3) Browse Products  (guest)");
        Console.WriteLine("  4) Exit");
    }

    public override bool HandleSelection(string input)
    {
        switch (input)
        {
            case "1": DoRegister(); return true;
            case "2": DoLogin();    return true;
            case "3": DoBrowse();   return true;
            case "4": DoExit();     return true;
            default:  return false;
        }
    }

    // ── Actions ───────────────────────────────────────────────────────────────

    private void DoRegister()
    {
        Console.WriteLine("\n  -- Register --");
        Console.Write("  Name                             : "); var name  = Console.ReadLine()?.Trim() ?? "";
        Console.Write("  Email                            : "); var email = Console.ReadLine()?.Trim() ?? "";
        Console.Write("  Password                         : "); var pass  = Console.ReadLine()?.Trim() ?? "";
        Console.Write("  Role (Customer / Administrator)  : "); var role  = Console.ReadLine()?.Trim() ?? "";

        var (ok, userId) = User.Sign(name, email, pass, role);
        if (ok)
        {
            Console.WriteLine($"  [✓] Registered successfully. Welcome, {name}!");
            GlobalMenuHolder.SwitchToRole(userId);
        }
        else
        {
            Console.WriteLine($"  [!] Registration failed: {userId}");
        }
    }

    private void DoLogin()
    {
        Console.WriteLine("\n  -- Login --");
        Console.Write("  Email    : "); var email = Console.ReadLine()?.Trim() ?? "";
        Console.Write("  Password : "); var pass  = Console.ReadLine()?.Trim() ?? "";

        var (ok, userId, _) = User.Login(email, pass);
        if (ok)
        {
            Console.WriteLine("  [✓] Login successful. Welcome back!");
            GlobalMenuHolder.SwitchToRole(userId);
        }
        else
        {
            Console.WriteLine($"  [!] Login failed: {userId}");
        }
    }

    private void DoBrowse()
    {
        Console.Write("\n  Search (leave blank for all): ");
        var query = Console.ReadLine()?.Trim() ?? "";
        var products = Product.SearchByName(query);
        if (products.Count == 0) { Console.WriteLine("  No products found."); return; }
        PrintProductTable(products);
    }

    private static void DoExit()
    {
        Console.WriteLine("\n  Goodbye!\n");
        Environment.Exit(0);
    }

    // ── Shared table printer (reused by AdminMenu / CustomerMenu) ─────────────

    public static void PrintProductTable(List<Product> products)
    {
        Console.WriteLine();
        Console.WriteLine($"  {"Name",-26} {"Price",9}  {"Stock",6}  {"ID"}");
        Console.WriteLine("  " + new string('-', 82));
        foreach (var p in products)
            Console.WriteLine($"  {p.Name,-26} {p.Price,9:C}  {p.Stock,6}  {p.Id}");
    }
}
