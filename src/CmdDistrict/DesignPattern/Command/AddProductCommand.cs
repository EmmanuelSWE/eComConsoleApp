namespace cmdDistrict.DesignPattern.Command;

using cmdDistrict.Models;
using cmdDistrict.Models.Entities;

public sealed class AddProductCommand : ICommand
{
    public string Description => "Add a new product to the catalog";

    public void Execute()
    {
        Console.WriteLine("\n  -- Add Product --");
        Console.Write("  Name        : "); var name = Console.ReadLine()?.Trim() ?? "";
        Console.Write("  Description : "); var desc = Console.ReadLine()?.Trim() ?? "";
        Console.Write("  Price       : "); var pStr = Console.ReadLine()?.Trim() ?? "";
        Console.Write("  Stock       : "); var sStr = Console.ReadLine()?.Trim() ?? "";

        if (!decimal.TryParse(pStr, out var price) || !int.TryParse(sStr, out var stock))
        { Console.WriteLine("  [!] Invalid price or stock — price must be decimal, stock must be whole number."); return; }

        var product = Product.Create(GlobalMenuHolder.CurrentUserId!, name, desc, price, stock);
        if (product is not null)
            Console.WriteLine($"  [✓] Product created — ID: {product.Id}");
        else
            Console.WriteLine("  [!] Failed — ensure name is not empty and price/stock ≥ 0.");
    }
}
