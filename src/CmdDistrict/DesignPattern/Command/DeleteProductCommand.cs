namespace cmdDistrict.DesignPattern.Command;

using cmdDistrict.Models;
using cmdDistrict.Models.Entities;

public sealed class DeleteProductCommand : ICommand
{
    public string Description => "Delete a product from the catalog";

    public void Execute()
    {
        Console.Write("\n  Product ID to delete: ");
        var id = Console.ReadLine()?.Trim() ?? "";

        if (Product.Delete(GlobalMenuHolder.CurrentUserId!, id))
            Console.WriteLine("  [✓] Product deleted.");
        else
            Console.WriteLine("  [!] Product not found.");
    }
}
