namespace cmdDistrict.DesignPattern.Command;

using cmdDistrict.Models;
using cmdDistrict.Models.Entities;

public sealed class LowStockCommand : ICommand
{
    public string Description => "View products with low stock (< 5)";

    public void Execute()
    {
        var low = Product.SearchByName("").Where(p => p.Stock < 5).OrderBy(p => p.Stock).ToList();
        if (low.Count == 0) { Console.WriteLine("\n  All products have sufficient stock (≥ 5)."); return; }

        Console.WriteLine("\n  -- Low-Stock Products (stock < 5) --");
        MainMenu.PrintProductTable(low);
    }
}
