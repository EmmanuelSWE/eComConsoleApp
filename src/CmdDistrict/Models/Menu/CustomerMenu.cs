using cmdDistrict.Common;
using cmdDistrict.Models.Entities;

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
            case "1":  DoBrowse();       return true;
            case "2":  DoAddToCart();    return true;
            case "3":  DoViewCart();     return true;
            case "4":  DoUpdateQty();    return true;
            case "5":  DoRemoveItem();   return true;
            case "6":  DoCheckout();     return true;
            case "7":  DoViewOrders();   return true;
            case "8":  DoTrackOrder();   return true;
            case "9":  DoAddReview();    return true;
            case "10": DoViewWallet();   return true;
            case "11": DoDeposit();      return true;
            case "12": DoLogout();       return true;
            default:   return false;
        }
    }

    // ── Actions ───────────────────────────────────────────────────────────────

    private void DoBrowse()
    {
        Console.Write("\n  Search (leave blank for all): ");
        var query    = Console.ReadLine()?.Trim() ?? "";
        var products = Product.SearchByName(query);
        if (products.Count == 0) { Console.WriteLine("  No products found."); return; }
        MainMenu.PrintProductTable(products);
    }

    private void DoAddToCart()
    {
        Console.WriteLine("\n  -- Add to Cart --");
        Console.Write("  Product ID : "); var productId = Console.ReadLine()?.Trim() ?? "";
        Console.Write("  Quantity   : "); var qStr      = Console.ReadLine()?.Trim() ?? "";

        if (!int.TryParse(qStr, out var qty) || qty <= 0)
        { Console.WriteLine("  [!] Quantity must be a positive whole number."); return; }

        if (Cart.AddItem(ContextUserId!, productId, qty))
            Console.WriteLine("  [✓] Item added to cart.");
        else
            Console.WriteLine("  [!] Could not add item — check product ID or available stock.");
    }

    private void DoViewCart()
    {
        var cart = Customer.ViewCart(ContextUserId!);
        if (cart is null || cart.Items.Count == 0)
        { Console.WriteLine("\n  Your cart is empty."); return; }

        Console.WriteLine();
        Console.WriteLine($"  {"Product",-26} {"Unit Price",10}  {"Qty",4}  {"Line Total",11}  {"Product ID"}");
        Console.WriteLine("  " + new string('-', 88));
        foreach (var item in cart.Items)
            Console.WriteLine(
                $"  {item.ProductName,-26} {item.UnitPrice,10:C}  {item.Quantity,4}  {item.UnitPrice * item.Quantity,11:C}  {item.ProductId}");
        Console.WriteLine("  " + new string('-', 88));
        Console.WriteLine($"  {"Total",-42} {Cart.GetTotal(ContextUserId!),11:C}");
    }

    private void DoUpdateQty()
    {
        Console.WriteLine("\n  -- Update Cart Item Quantity --");
        Console.Write("  Product ID   : "); var productId = Console.ReadLine()?.Trim() ?? "";
        Console.Write("  New Quantity : "); var qStr      = Console.ReadLine()?.Trim() ?? "";

        if (!int.TryParse(qStr, out var newQty) || newQty <= 0)
        { Console.WriteLine("  [!] Quantity must be a positive whole number."); return; }

        if (CartItem.UpdateQuantity(ContextUserId!, productId, newQty))
            Console.WriteLine("  [✓] Cart updated.");
        else
            Console.WriteLine("  [!] Update failed — check product ID or stock availability.");
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
        Console.WriteLine("\n  -- Checkout --");

        var cart = Customer.ViewCart(ContextUserId!);
        if (cart is null || cart.Items.Count == 0)
        { Console.WriteLine("  [!] Your cart is empty. Add items before checking out."); return; }

        var order = Order.PlaceFromCart(ContextUserId!, cart);
        if (order is null)
        { Console.WriteLine("  [!] Could not place order. Please try again."); return; }

        Console.WriteLine($"  Order placed (ID: {order.Id}). Total: {order.Total:C}");
        Console.WriteLine("  Processing payment...");

        var payment = Payment.ChargeWallet(ContextUserId!, order.Id, order.Total);

        if (payment.Status == PaymentStatus.Failed)
        {
            Console.WriteLine($"  [!] Insufficient funds. Your total is {order.Total:C}.");
            Console.Write("  Deposit now to complete payment? (y/n): ");
            var choice = Console.ReadLine()?.Trim().ToLower();

            if (choice == "y")
            {
                Console.Write("  Deposit amount: ");
                var aStr = Console.ReadLine()?.Trim() ?? "";
                if (decimal.TryParse(aStr, out var amount) && Customer.Deposit(ContextUserId!, amount))
                {
                    Console.WriteLine($"  [✓] Deposited {amount:C}. Retrying payment...");
                    payment = Payment.ChargeWallet(ContextUserId!, order.Id, order.Total);
                }
                else
                {
                    Console.WriteLine("  [!] Deposit failed. Order cancelled.");
                    Order.Cancel(ContextUserId!, order.Id);
                    return;
                }
            }
            else
            {
                Console.WriteLine("  [!] Payment skipped. Order cancelled.");
                Order.Cancel(ContextUserId!, order.Id);
                return;
            }
        }

        if (payment.Status == PaymentStatus.Captured)
        {
            Cart.Clear(ContextUserId!);
            Console.WriteLine($"  [✓] Payment captured. Order confirmed (ID: {order.Id}).");
        }
        else
        {
            Console.WriteLine("  [!] Payment failed after deposit. Order cancelled.");
            Order.Cancel(ContextUserId!, order.Id);
        }
    }

    private void DoViewOrders()
    {
        var orders = Customer.ViewOrders(ContextUserId!);
        if (orders.Count == 0) { Console.WriteLine("\n  No orders found."); return; }

        Console.WriteLine();
        Console.WriteLine($"  {"Date",-20} {"Status",-12} {"Total",9}  {"Order ID"}");
        Console.WriteLine("  " + new string('-', 80));
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
            Console.WriteLine($"  Order {orderId} is currently: {status}");
        }
        catch
        {
            Console.WriteLine("  [!] Order not found.");
        }
    }

    private void DoAddReview()
    {
        Console.WriteLine("\n  -- Add Review --");
        Console.Write("  Product ID : "); var productId = Console.ReadLine()?.Trim() ?? "";
        Console.Write("  Rating (1-5): "); var rStr     = Console.ReadLine()?.Trim() ?? "";
        Console.Write("  Comment     : "); var comment  = Console.ReadLine()?.Trim() ?? "";

        if (!int.TryParse(rStr, out var rating) || rating < 1 || rating > 5)
        { Console.WriteLine("  [!] Rating must be between 1 and 5."); return; }

        if (Review.Submit(ContextUserId!, productId, rating, comment))
            Console.WriteLine("  [✓] Review submitted. Thank you!");
        else
            Console.WriteLine("  [!] Review failed — check product ID or ensure you haven't reviewed this product already.");
    }

    private void DoViewWallet()
    {
        var customer = AppState.Users.OfType<Customer>().FirstOrDefault(u => u.Id == ContextUserId);
        if (customer is not null)
            Console.WriteLine($"\n  Wallet Balance: {customer.WalletBalance:C}");
        else
            Console.WriteLine("  [!] Could not retrieve wallet.");
    }

    private void DoDeposit()
    {
        Console.Write("\n  Deposit amount: ");
        var aStr = Console.ReadLine()?.Trim() ?? "";

        if (!decimal.TryParse(aStr, out var amount) || amount <= 0)
        { Console.WriteLine("  [!] Amount must be a positive number."); return; }

        if (Customer.Deposit(ContextUserId!, amount))
            Console.WriteLine($"  [✓] Deposited {amount:C} to your wallet.");
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
