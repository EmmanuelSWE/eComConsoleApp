namespace cmdDistrict.DesignPattern.Command;

using cmdDistrict.Models;
using cmdDistrict.Models.Entities;

public sealed class CheckoutCommand : ICommand
{
    public string Description => "Checkout — Converts your cart into an order and charges your wallet. Ensure your wallet balance covers the cart total before checking out.";

    public void Execute()
    {
        Console.WriteLine("\n  -- Checkout --");
        var userId = GlobalMenuHolder.CurrentUserId!;

        var cart = Customer.ViewCart(userId);
        if (cart is null || cart.Items.Count == 0)
        { Console.WriteLine("  [!] Your cart is empty. Add items before checking out."); return; }

        var order = Order.PlaceFromCart(userId, cart);
        if (order is null)
        { Console.WriteLine("  [!] Could not place order. Please try again."); return; }

        Console.WriteLine($"  Order placed (ID: {order.Id}). Total: {order.Total:C}");
        Console.WriteLine("  Processing payment...");

        var payment = Payment.ChargeWallet(userId, order.Id, order.Total);

        if (payment.Status == PaymentStatus.Failed)
        {
            Console.WriteLine($"  [!] Insufficient funds. Your total is {order.Total:C}.");
            Console.Write("  Deposit now to complete payment? (y/n): ");
            var choice = Console.ReadLine()?.Trim().ToLower();

            if (choice == "y")
            {
                Console.Write("  Deposit amount: ");
                var aStr = Console.ReadLine()?.Trim() ?? "";
                if (decimal.TryParse(aStr, out var amount) && Customer.Deposit(userId, amount))
                {
                    Console.WriteLine($"  [✓] Deposited {amount:C}. Retrying payment...");
                    payment = Payment.ChargeWallet(userId, order.Id, order.Total);
                }
                else
                {
                    Console.WriteLine("  [!] Deposit failed. Order cancelled.");
                    Order.Cancel(userId, order.Id);
                    return;
                }
            }
            else
            {
                Console.WriteLine("  [!] Payment skipped. Order cancelled.");
                Order.Cancel(userId, order.Id);
                return;
            }
        }

        if (payment.Status == PaymentStatus.Captured)
        {
            Cart.Clear(userId);
            Console.WriteLine($"  [✓] Payment captured. Order confirmed (ID: {order.Id}).");
        }
        else
        {
            Console.WriteLine("  [!] Payment failed after deposit. Order cancelled.");
            Order.Cancel(userId, order.Id);
        }
    }
}
