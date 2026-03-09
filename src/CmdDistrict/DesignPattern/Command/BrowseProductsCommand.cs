namespace cmdDistrict.DesignPattern.Command;

using cmdDistrict.Models;
using cmdDistrict.Models.Entities;

public sealed class BrowseProductsCommand : ICommand
{
    public string Description => "Browse and search products";

    public void Execute()
    {
        Console.Write("\n  Search (leave blank for all): ");
        var query    = Console.ReadLine()?.Trim() ?? "";
        var products = Product.SearchByName(query);
        if (products.Count == 0) { Console.WriteLine("  No products found."); return; }
        MainMenu.PrintProductTable(products);
    }
}
