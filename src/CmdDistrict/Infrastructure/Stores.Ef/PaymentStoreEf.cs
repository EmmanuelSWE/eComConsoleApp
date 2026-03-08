using cmdDistrict.DataAccess;
using cmdDistrict.Models.Entities;

namespace cmdDistrict.Infrastructure.StoresEf;

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
            using var ctx = new AppDbContext();
            var order = ctx.Orders.SingleOrDefault(o => o.Id == orderId);
            if (order is null || order.CustomerId != userId)
                return new Payment(orderId, userId, amount, PaymentStatus.Failed);
            if (amount <= 0)
                return new Payment(orderId, userId, amount, PaymentStatus.Failed);

            var customer = ctx.Users.OfType<Customer>().SingleOrDefault(u => u.Id == userId);
            if (customer is null)
                return new Payment(orderId, userId, amount, PaymentStatus.Failed);
            if (customer.WalletBalance < amount)
                return new Payment(orderId, userId, amount, PaymentStatus.Failed);

            customer.WalletBalance -= amount;
            order.Status = OrderStatus.Paid;

            var payment = new Payment(orderId, userId, amount, PaymentStatus.Captured);
            ctx.Payments.Add(payment);
            ctx.SaveChanges();
            return payment;
        }
        catch { return new Payment(orderId, userId, amount, PaymentStatus.Failed); }
    }

    /// <summary>Refunds a captured payment and credits the customer's wallet (admin only).</summary>
    public static bool Refund(string userId, string paymentId)
    {
        try
        {
            using var ctx = new AppDbContext();
            if (!ctx.Users.Any(u => u.Id == userId && u.Role == "Administrator")) return false;

            var payment = ctx.Payments.SingleOrDefault(p => p.Id == paymentId);
            if (payment is null || payment.Status != PaymentStatus.Captured) return false;

            var customer = ctx.Users.OfType<Customer>().SingleOrDefault(u => u.Id == payment.CustomerId);
            if (customer is not null) customer.WalletBalance += payment.Amount;

            payment.Status = PaymentStatus.Refunded;

            var order = ctx.Orders.SingleOrDefault(o => o.Id == payment.OrderId);
            if (order is not null) order.Status = OrderStatus.Cancelled;

            ctx.SaveChanges();
            return true;
        }
        catch { return false; }
    }
}
