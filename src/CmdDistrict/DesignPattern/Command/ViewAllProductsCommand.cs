namespace cmdDistrict.DesignPattern.Command;

using cmdDistrict.Models;
using cmdDistrict.Models.Entities;

public sealed class ViewAllProductsCommand : ICommand
{
    public string Description => "View All Products — Displays every product in the catalog with name, price, stock level, and ID regardless of availability.";

    public void Execute()
    {
        var products = Product.SearchByName("");
        if (products.Count == 0) { Console.WriteLine("\n  No products in catalog."); return; }
        MainMenu.PrintProductTable(products);
    }
}
