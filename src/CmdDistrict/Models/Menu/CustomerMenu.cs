using cmdDistrict.Models.Entities;
using cmdDistrict.Common;

namespace cmdDistrict.Models;

public class CustomerMenu : Menu
{
    public CustomerMenu() { Key = "customer"; Title = "Customer Menu"; }

    public override void PrintOptions()
    {
        var name = AppState.Users.FirstOrDefault(u => u.Id == ContextUserId)?.Name ?? "Customer";
        Console.WriteLine($"  Hello, {name}!\n");
        Console.WriteLine("  1)  Browse Products");
        Console.WriteLine("  2)  Add Product to Cart");
        Console.WriteLine("  3)  View Cart");
        Console.WriteLine("  4)  Update Cart Item Quantity");
        Console.WriteLine("  5)  Remove Cart Item");
        Console.WriteLine("  6)  Checkout");
        Console.WriteLine("  7)  View Order History");
        Console.WriteLine("  8)  Track Order Status");
        Console.WriteLine("  9)  Review a Product");
        Console.WriteLine("  10) View Wallet Balance");
        Console.WriteLine("  11) Add Wallet Funds");
        Console.WriteLine("  12) Logout");
    }

    public override bool HandleSelection(string input)
    {
        switch (input)
        {
            case "1":  DoBrowse();      return true;
            case "2":  DoAddToCart();   return true;
            case "3":  DoViewCart();    return true;
            case "4":  DoUpdateCart();  return true;
            case "5":  DoRemoveItem();  return true;
            case "6":  DoCheckout();    return true;
            case "7":  DoViewOrders();  return true;
            case "8":  DoTrackOrder();  return true;
            case "9":  DoReview();      return true;
            case "10": DoViewWallet();  return true;
            case "11": DoDeposit();     return true;
            case "12": DoLogout();      return true;
            default:   return false;
        }
    }

    // ── Helpers ──────────────────────────────────────────────────────────────

    private void DoBrowse()
    {
        Console.Write("\n  Search (leave blank for all): ");
        var q        = Console.ReadLine()?.Trim() ?? "";
        var products = Product.SearchByName(q);

        if (products.Count == 0) { Console.WriteLine("  No products found."); return; }
        MainMenu.PrintProductTable(products);
    }

    private void DoAddToCart()
    {
        Console.Write("\n  Product ID : "); var productId = Console.ReadLine()?.Trim() ?? "";
        Console.Write("  Quantity   : "); var qtyStr    = Console.ReadLine()?.Trim() ?? "";

        if (!int.TryParse(qtyStr, out var qty) || qty <= 0)
        { Console.WriteLine("  [!] Quantity must be a positive whole number."); return; }

        if (Cart.AddItem(ContextUserId!, productId, qty))
            Console.WriteLine("  [✓] Item added to cart.");
        else
            Console.WriteLine("  [!] Could not add item — check product ID, quantity, and available stock.");
    }

    private void DoViewCart()
    {
        var cart  = Customer.ViewCart(ContextUserId!);
        var total = Cart.GetTotal(ContextUserId!);

        if (cart.Items.Count == 0) { Console.WriteLine("\n  Your cart is empty."); return; }

        Console.WriteLine();
        Console.WriteLine($"  {"Product",-24} {"Qty",5}  {"Unit Price",10}  {"Subtotal",10}");
        Console.WriteLine("  " + new string('-', 58));
        foreach (var i in cart.Items)
            Console.WriteLine($"  {i.ProductName,-24} {i.Quantity,5}  {i.UnitPrice,10:C}  {i.UnitPrice * i.Quantity,10:C}");
        Console.WriteLine("  " + new string('-', 58));
        Console.WriteLine($"  {"Total",-24} {"",5}  {"",10}  {total,10:C}");
    }

    private void DoUpdateCart()
    {
        Console.Write("\n  Product ID   : "); var productId = Console.ReadLine()?.Trim() ?? "";
        Console.Write("  New Quantity : "); var qtyStr    = Console.ReadLine()?.Trim() ?? "";

        if (!int.TryParse(qtyStr, out var qty) || qty <= 0)
        { Console.WriteLine("  [!] Quantity must be a positive whole number."); return; }

        if (CartItem.UpdateQuantity(ContextUserId!, productId, qty))
            Console.WriteLine("  [✓] Cart updated.");
        else
            Console.WriteLine("  [!] Update failed — check product ID, quantity, and available stock.");
    }

    private void DoRemoveItem()
    {
        Console.Write("\n  Product ID to remove: ");
        var productId = Console.ReadLine()?.Trim() ?? "";

        if (Cart.RemoveItem(ContextUserId!, productId))
            Console.WriteLine("  [✓] Item removed from cart.");
        else
            Console.WriteLine("  [!] Item not found in cart.");
    }

    private void DoCheckout()
    {
        var cart = Customer.ViewCart(ContextUserId!);

        if (cart.Items.Count == 0)
        { Console.WriteLine("\n  [!] Your cart is empty."); return; }

        var order = Order.PlaceFromCart(ContextUserId!, cart);
        if (order is null)
        { Console.WriteLine("  [!] Checkout failed — some items may be out of stock."); return; }

        Console.WriteLine($"  [✓] Order created  ID: {order.Id}");
        Console.WriteLine($"      Total: {order.Total:C}");
        Console.WriteLine("  Processing payment...");

        var payment = Payment.ChargeWallet(ContextUserId!, order.Id, order.Total);

        if (payment.Status == PaymentStatus.Captured)
        {
            Cart.Clear(ContextUserId!);
            Console.WriteLine("  [✓] Payment successful — order status: Paid.");
        }
        else
        {
            var balance = (AppState.Users.FirstOrDefault(u => u.Id == ContextUserId) as Customer)
                             ?.WalletBalance ?? 0m;
            Console.WriteLine($"  [!] Insufficient funds — wallet: {balance:C}, required: {order.Total:C}.");
            Console.Write("  Add funds now? (y/n): ");
            var answer = Console.ReadLine()?.Trim().ToLower();

            if (answer == "y")
            {
                DoDeposit();
                Console.WriteLine("  Retrying payment...");
                var retry = Payment.ChargeWallet(ContextUserId!, order.Id, order.Total);
                if (retry.Status == PaymentStatus.Captured)
                {
                    Cart.Clear(ContextUserId!);
                    Console.WriteLine("  [✓] Payment successful — order status: Paid.");
                }
                else
                {
                    Console.WriteLine("  [!] Still insufficient funds — order saved as Pending.");
                }
            }
            else
            {
                Console.WriteLine("  Order saved as Pending. You can retry payment later.");
            }
        }
    }

    private void DoViewOrders()
    {
        var orders = Customer.ViewOrders(ContextUserId!);

        if (orders.Count == 0) { Console.WriteLine("\n  You have no orders yet."); return; }

        Console.WriteLine();
        Console.WriteLine($"  {"Date",-20} {"Status",-12} {"Total",9}  {"Order ID"}");
        Console.WriteLine("  " + new string('-', 75));
        foreach (var o in orders)
            Console.WriteLine(
                $"  {o.CreatedAt:yyyy-MM-dd HH:mm,-20} {o.Status,-12} {o.Total,9:C}  {o.Id}");
    }

    private void DoTrackOrder()
    {
        Console.Write("\n  Order ID: ");
        var orderId = Console.ReadLine()?.Trim() ?? "";
        try
        {
            var status = Order.TrackStatus(ContextUserId!, orderId);
            Console.WriteLine($"  Order status: {status}");
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"  [!] {ex.Message}");
        }
    }

    private void DoReview()
    {
        Console.Write("\n  Product ID    : "); var productId = Console.ReadLine()?.Trim() ?? "";
        Console.Write("  Rating (1–5)  : "); var ratingStr = Console.ReadLine()?.Trim() ?? "";
        Console.Write("  Comment       : "); var comment   = Console.ReadLine()?.Trim() ?? "";

        if (!int.TryParse(ratingStr, out var rating))
        { Console.WriteLine("  [!] Rating must be a number between 1 and 5."); return; }

        if (Review.Submit(ContextUserId!, productId, rating, comment))
            Console.WriteLine("  [✓] Review submitted.");
        else
            Console.WriteLine("  [!] Failed — rating must be between 1 and 5.");
    }

    private void DoViewWallet()
    {
        var balance = (AppState.Users.FirstOrDefault(u => u.Id == ContextUserId) as Customer)
                         ?.WalletBalance ?? 0m;
        Console.WriteLine($"\n  Wallet Balance: {balance:C}");
    }

    private void DoDeposit()
    {
        Console.Write("\n  Amount to deposit: $");
        var amtStr = Console.ReadLine()?.Trim() ?? "";

        if (!decimal.TryParse(amtStr, out var amount) || amount <= 0)
        { Console.WriteLine("  [!] Amount must be greater than zero."); return; }

        if (Customer.Deposit(ContextUserId!, amount))
            Console.WriteLine($"  [✓] {amount:C} added to your wallet.");
        else
            Console.WriteLine("  [!] Deposit failed.");
    }

    private void DoLogout()
    {
        User.Logout();
        Console.WriteLine("  [✓] Logged out successfully.");
        GlobalMenuHolder.SignOut();
    }
}
