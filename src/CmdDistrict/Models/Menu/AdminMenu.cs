using cmdDistrict.DesignPattern.Command;

namespace cmdDistrict.Models;

public class AdminMenu : Menu
{
    public AdminMenu()
    {
        Key   = "admin";
        Title = "Administrator Menu";
        Commands = new List<ICommand>
        {
            new AddProductCommand(),
            new UpdateProductCommand(),
            new DeleteProductCommand(),
            new RestockCommand(),
            new ViewAllProductsCommand(),
            new ViewAllOrdersCommand(),
            new UpdateOrderStatusCommand(),
            new LowStockCommand(),
            new GenerateReportCommand(),
            new LogoutCommand(),
        };
    }

    public override void PrintOptions()
    {
        Console.WriteLine("  1)  Add Product");
        Console.WriteLine("  2)  Update Product");
        Console.WriteLine("  3)  Delete Product");
        Console.WriteLine("  4)  Restock Product  (Adjust Inventory)");
        Console.WriteLine("  5)  View All Products");
        Console.WriteLine("  6)  View All Orders");
        Console.WriteLine("  7)  Update Order Status");
        Console.WriteLine("  8)  View Low-Stock Products  (stock < 5)");
        Console.WriteLine("  9)  Generate Sales Report");
        Console.WriteLine("  10) Logout");
        Console.WriteLine("  11) Describe Menu");
    }

    public override bool HandleSelection(string input)
    {
        switch (input)
        {
            case "1":  Commands[0].Execute(); return true;
            case "2":  Commands[1].Execute(); return true;
            case "3":  Commands[2].Execute(); return true;
            case "4":  Commands[3].Execute(); return true;
            case "5":  Commands[4].Execute(); return true;
            case "6":  Commands[5].Execute(); return true;
            case "7":  Commands[6].Execute(); return true;
            case "8":  Commands[7].Execute(); return true;
            case "9":  Commands[8].Execute(); return true;
            case "10": Commands[9].Execute(); return true;
            case "11": DescribeMenu();        return true;
            default:   return false;
        }
    }

  
}
