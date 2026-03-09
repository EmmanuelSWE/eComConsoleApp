namespace cmdDistrict.DesignPattern.Command;

using cmdDistrict.Models;
using cmdDistrict.Models.Entities;

public sealed class BrowseGuestCommand : ICommand
{
    public string Description => "Browse products as a guest";

    public void Execute()
    {
        Console.Write("\n  Search (leave blank for all): ");
        var query    = Console.ReadLine()?.Trim() ?? "";
        var products = Product.SearchByName(query);
        if (products.Count == 0) { Console.WriteLine("  No products found."); return; }
        MainMenu.PrintProductTable(products);
    }
}
