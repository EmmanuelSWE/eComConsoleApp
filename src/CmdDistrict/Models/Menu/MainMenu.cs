using cmdDistrict.DesignPattern.Command;
using cmdDistrict.Models.Entities;
using cmdDistrict;

namespace cmdDistrict.Models;

public class MainMenu : Menu
{
    public MainMenu()
    {
        Key   = "main";
        Title = "Cmd District — Main Menu";
        Commands = new List<ICommand>
        {
            new RegisterCommand(),
            new LoginCommand(),
            new BrowseGuestCommand(),
            new ExitCommand(),
        };
    }

    public override void PrintOptions()
    {
        Console.WriteLine("  1) Register");
        Console.WriteLine("  2) Login");
        Console.WriteLine("  3) Browse Products  (guest)");
        Console.WriteLine("  4) Exit");
        Console.WriteLine("  5) Test functions (not a real menu option)");
        Console.WriteLine("  6) Describe Menu");
    }

    public override bool HandleSelection(string input)
    {
        switch (input)
        {
            case "1": Commands[0].Execute(); return true;
            case "2": Commands[1].Execute(); return true;
            case "3": Commands[2].Execute(); return true;
            case "4": Commands[3].Execute(); return true;
            case "5": Tests.Run();           return true;
            case "6": DescribeMenu();        return true;
            default:  return false;
        }
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
