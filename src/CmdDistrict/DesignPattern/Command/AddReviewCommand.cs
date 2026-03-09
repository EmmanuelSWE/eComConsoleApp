namespace cmdDistrict.DesignPattern.Command;

using cmdDistrict.Models;
using cmdDistrict.Models.Entities;

public sealed class AddReviewCommand : ICommand
{
    public string Description => "Submit a product review";

    public void Execute()
    {
        Console.WriteLine("\n  -- Add Review --");
        Console.Write("  Product ID  : "); var productId = Console.ReadLine()?.Trim() ?? "";
        Console.Write("  Rating (1-5): "); var rStr      = Console.ReadLine()?.Trim() ?? "";
        Console.Write("  Comment     : "); var comment   = Console.ReadLine()?.Trim() ?? "";

        if (!int.TryParse(rStr, out var rating) || rating < 1 || rating > 5)
        { Console.WriteLine("  [!] Rating must be between 1 and 5."); return; }

        if (Review.Submit(GlobalMenuHolder.CurrentUserId!, productId, rating, comment))
            Console.WriteLine("  [✓] Review submitted. Thank you!");
        else
            Console.WriteLine("  [!] Review failed — check product ID or ensure you haven't reviewed this product already.");
    }
}
