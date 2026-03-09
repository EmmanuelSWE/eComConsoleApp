namespace cmdDistrict.DesignPattern.Command;

using cmdDistrict.Models;
using cmdDistrict.Models.Entities;

public sealed class BrowseGuestCommand : ICommand
{
    public string Description => "Browse Products (Guest) — Enter a search term to filter products by name, or press Enter to see the full catalog. No login required.";

    public void Execute()
    {
        Console.Write("\n  Search (leave blank for all): ");
        var query    = Console.ReadLine()?.Trim() ?? "";
        var products = Product.SearchByName(query);
        if (products.Count == 0) { Console.WriteLine("  No products found."); return; }
        MainMenu.PrintProductTable(products);
    }
}
