namespace cmdDistrict.DesignPattern.Command;

using cmdDistrict.Models;
using cmdDistrict.Models.Entities;

public sealed class ViewAllProductsCommand : ICommand
{
    public string Description => "View all products in the catalog";

    public void Execute()
    {
        var products = Product.SearchByName("");
        if (products.Count == 0) { Console.WriteLine("\n  No products in catalog."); return; }
        MainMenu.PrintProductTable(products);
    }
}
