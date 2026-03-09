using cmdDistrict.DesignPattern.Command;

namespace cmdDistrict.Models;

public class CustomerMenu : Menu
{
    public CustomerMenu() { Key = "customer"; Title = "Customer Menu"; }

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
    }

    public override bool HandleSelection(string input)
    {
        switch (input)
        {
            case "1":  new BrowseProductsCommand().Execute();  return true;
            case "2":  new AddToCartCommand().Execute();       return true;
            case "3":  new ViewCartCommand().Execute();        return true;
            case "4":  new UpdateCartQtyCommand().Execute();   return true;
            case "5":  new RemoveCartItemCommand().Execute();  return true;
            case "6":  new CheckoutCommand().Execute();        return true;
            case "7":  new ViewOrdersCommand().Execute();      return true;
            case "8":  new TrackOrderCommand().Execute();      return true;
            case "9":  new AddReviewCommand().Execute();       return true;
            case "10": new ViewWalletCommand().Execute();      return true;
            case "11": new DepositCommand().Execute();         return true;
            case "12": new LogoutCommand().Execute();          return true;
            default:   return false;
        }
    }


}
