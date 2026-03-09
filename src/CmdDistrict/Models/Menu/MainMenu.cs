using cmdDistrict.Common;
using cmdDistrict.DataAccess;
using cmdDistrict.DesignPattern.Command;
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
            case "1": new RegisterCommand(DoRegister).Execute();    return true;
            case "2": new LoginCommand(DoLogin).Execute();          return true;
            case "3": new BrowseGuestCommand(DoBrowse).Execute();   return true;
            case "4": new ExitCommand(DoExit).Execute();            return true;
            default:  return false;
        }
    }

    // ── Actions ───────────────────────────────────────────────────────────────

    private void DoRegister()
    {
        Console.WriteLine("\n  -- Register --");
        Console.Write("  Name                                    : "); var name      = Console.ReadLine()?.Trim() ?? "";
        Console.Write("  Email                                   : "); var email     = Console.ReadLine()?.Trim() ?? "";
        Console.Write("  Password                                : "); var pass      = Console.ReadLine()?.Trim() ?? "";
        Console.Write("  Role (1 = Customer, 2 = Administrator)  : "); var roleInput = Console.ReadLine()?.Trim() ?? "";

        var role    = roleInput == "2" ? "Administrator" : "Customer";
        var address = "";

        if (role == "Customer")
        {
            Console.Write("  Default Shipping Address                : "); address = Console.ReadLine()?.Trim() ?? "";
        }

        if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(pass))
        {
            Console.WriteLine("  [!] Name, email, and password must not be empty.");
            return;
        }
        if (role == "Customer" && string.IsNullOrEmpty(address))
        {
            Console.WriteLine("  [!] Shipping address must not be empty for Customer accounts.");
            return;
        }

        var (ok, userId) = User.Sign(name, email, pass, role);
        if (ok)
        {
            if (role == "Customer" && !string.IsNullOrEmpty(address))
            {
                using var ctx = new AppDbContext();
                var customer = ctx.Users.OfType<Customer>().SingleOrDefault(u => u.Id == Session.CurrentUserId);
                if (customer is not null)
                {
                    customer.DefaultShippingAddress = address;
                    ctx.SaveChanges();
                }
            }
            Console.WriteLine($"  [✓] Registered successfully. Welcome, {name}!");
            GlobalMenuHolder.SwitchToRole(userId);
        }
        else
        {
            Console.WriteLine("  [!] Registration failed — email may already be in use.");
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
