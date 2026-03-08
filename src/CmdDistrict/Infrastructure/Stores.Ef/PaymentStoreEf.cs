using cmdDistrict.DataAccess;
using cmdDistrict.Models.Entities;

namespace cmdDistrict.Infrastructure.Stores.Ef;

/// <summary>
/// EF Core LINQ-backed payment store.
/// ChargeWallet returns a Payment whose Status indicates success/failure.
/// Refund is bool-only (admin-gated).
/// </summary>
public static class PaymentStoreEf
{
    // ── Write ─────────────────────────────────────────────────────────────────

    /// <summary>
    /// Deducts <paramref name="amount"/> from the customer's wallet and marks the order Paid.
    /// Returns a Captured payment on success; a Failed payment on any guard failure.
    /// </summary>
    public static Payment ChargeWallet(string userId, string orderId, decimal amount)
    {
        try
        {
            Console.WriteLine($"function name is : {nameof(ChargeWallet)}");
            Console.WriteLine($"Arguments are : userId={userId}, orderId={orderId}, amount={amount}");
            Console.WriteLine($"expected return : Payment");
            using var ctx = new AppDbContext();
            var order = ctx.Orders.SingleOrDefault(o => o.Id == orderId);
            if (order is null || order.CustomerId != userId)
            {
                Console.WriteLine($"actual return : Payment(Status=Failed)");
                Console.WriteLine($"Outcome : failed");
                return new Payment(orderId, userId, amount, PaymentStatus.Failed);
            }
            if (amount <= 0)
            {
                Console.WriteLine($"actual return : Payment(Status=Failed)");
                Console.WriteLine($"Outcome : failed");
                return new Payment(orderId, userId, amount, PaymentStatus.Failed);
            }

            var customer = ctx.Users.OfType<Customer>().SingleOrDefault(u => u.Id == userId);
            if (customer is null)
            {
                Console.WriteLine($"actual return : Payment(Status=Failed)");
                Console.WriteLine($"Outcome : failed");
                return new Payment(orderId, userId, amount, PaymentStatus.Failed);
            }
            if (customer.WalletBalance < amount)
            {
                Console.WriteLine($"actual return : Payment(Status=Failed)");
                Console.WriteLine($"Outcome : failed");
                return new Payment(orderId, userId, amount, PaymentStatus.Failed);
            }

            customer.WalletBalance -= amount;
            order.Status = OrderStatus.Paid;

            var payment = new Payment(orderId, userId, amount, PaymentStatus.Captured);
            ctx.Payments.Add(payment);
            ctx.SaveChanges();
            Console.WriteLine($"actual return : Payment(Status=Captured, Id={payment.Id})");
            Console.WriteLine($"Outcome : passed");
            return payment;
        }
        catch
        {
            Console.WriteLine($"Outcome : encountered an error");
            return new Payment(orderId, userId, amount, PaymentStatus.Failed);
        }
    }

    /// <summary>Refunds a captured payment and credits the customer's wallet (admin only).</summary>
    public static bool Refund(string userId, string paymentId)
    {
        try
        {
            Console.WriteLine($"function name is : {nameof(Refund)}");
            Console.WriteLine($"Arguments are : userId={userId}, paymentId={paymentId}");
            Console.WriteLine($"expected return : bool");
            using var ctx = new AppDbContext();
            if (!ctx.Users.Any(u => u.Id == userId && u.Role == "Administrator"))
            {
                Console.WriteLine($"actual return : false");
                Console.WriteLine($"Outcome : failed");
                return false;
            }

            var payment = ctx.Payments.SingleOrDefault(p => p.Id == paymentId);
            if (payment is null || payment.Status != PaymentStatus.Captured)
            {
                Console.WriteLine($"actual return : false");
                Console.WriteLine($"Outcome : failed");
                return false;
            }

            var customer = ctx.Users.OfType<Customer>().SingleOrDefault(u => u.Id == payment.CustomerId);
            if (customer is not null) customer.WalletBalance += payment.Amount;

            payment.Status = PaymentStatus.Refunded;

            var order = ctx.Orders.SingleOrDefault(o => o.Id == payment.OrderId);
            if (order is not null) order.Status = OrderStatus.Cancelled;

            ctx.SaveChanges();
            Console.WriteLine($"actual return : true");
            Console.WriteLine($"Outcome : passed");
            return true;
        }
        catch
        {
            Console.WriteLine($"Outcome : encountered an error");
            return false;
        }
    }
}
