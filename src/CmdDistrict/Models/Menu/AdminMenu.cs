using cmdDistrict.DesignPattern.Command;

namespace cmdDistrict.Models;

public class AdminMenu : Menu
{
    public AdminMenu() { Key = "admin"; Title = "Administrator Menu"; }

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
    }

    public override bool HandleSelection(string input)
    {
        switch (input)
        {
            case "1":  new AddProductCommand().Execute();          return true;
            case "2":  new UpdateProductCommand().Execute();       return true;
            case "3":  new DeleteProductCommand().Execute();       return true;
            case "4":  new RestockCommand().Execute();             return true;
            case "5":  new ViewAllProductsCommand().Execute();     return true;
            case "6":  new ViewAllOrdersCommand().Execute();       return true;
            case "7":  new UpdateOrderStatusCommand().Execute();   return true;
            case "8":  new LowStockCommand().Execute();            return true;
            case "9":  new GenerateReportCommand().Execute();      return true;
            case "10": new LogoutCommand().Execute();              return true;
            default:   return false;
        }
    }

  
}
