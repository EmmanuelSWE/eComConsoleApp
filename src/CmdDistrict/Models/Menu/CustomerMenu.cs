using cmdDistrict.DesignPattern.Command;

namespace cmdDistrict.Models;

public class CustomerMenu : Menu
{
    public CustomerMenu()
    {
        Key   = "customer";
        Title = "Customer Menu";
        Commands = new List<ICommand>
        {
            new BrowseProductsCommand(),
            new AddToCartCommand(),
            new ViewCartCommand(),
            new UpdateCartQtyCommand(),
            new RemoveCartItemCommand(),
            new CheckoutCommand(),
            new ViewOrdersCommand(),
            new TrackOrderCommand(),
            new AddReviewCommand(),
            new ViewWalletCommand(),
            new DepositCommand(),
            new LogoutCommand(),
        };
    }

    public override void PrintOptions()
    {
        Console.WriteLine("  1)  Browse Products");
        Console.WriteLine("  2)  Add to Cart");
        Console.WriteLine("  3)  View Cart");
        Console.WriteLine("  4)  Update Cart Item Quantity");
        Console.WriteLine("  5)  Remove Item from Cart");
        Console.WriteLine("  6)  Checkout");
        Console.WriteLine("  7)  View My Orders");
        Console.WriteLine("  8)  Track Order");
        Console.WriteLine("  9)  Add Product Review");
        Console.WriteLine("  10) View Wallet Balance");
        Console.WriteLine("  11) Deposit Funds");
        Console.WriteLine("  12) Logout");
        Console.WriteLine("  13) Describe Menu");
    }

    public override bool HandleSelection(string input)
    {
        switch (input)
        {
            case "1":  Commands[0].Execute();  return true;
            case "2":  Commands[1].Execute();  return true;
            case "3":  Commands[2].Execute();  return true;
            case "4":  Commands[3].Execute();  return true;
            case "5":  Commands[4].Execute();  return true;
            case "6":  Commands[5].Execute();  return true;
            case "7":  Commands[6].Execute();  return true;
            case "8":  Commands[7].Execute();  return true;
            case "9":  Commands[8].Execute();  return true;
            case "10": Commands[9].Execute();  return true;
            case "11": Commands[10].Execute(); return true;
            case "12": Commands[11].Execute(); return true;
            case "13": DescribeMenu();         return true;
            default:   return false;
        }
    }


}
