using cmdDistrict.Common;

namespace cmdDistrict.Models.Entities;
{
    private string        _id;
    private string        _orderId;
    private string        _customerId;
    private decimal       _amount;
    private PaymentStatus _status;

    public Payment(string orderId, string customerId, decimal amount, PaymentStatus status)
    {
        _id         = Guid.NewGuid().ToString();
        _orderId    = orderId;
        _customerId = customerId;
        _amount     = amount;
        _status     = status;
    }

    public string        Id         { get => _id;         set => _id         = value; }
    public string        OrderId    { get => _orderId;    set => _orderId    = value; }
    public string        CustomerId { get => _customerId; set => _customerId = value; }
    public decimal       Amount     { get => _amount;     set => _amount     = value; }
    public PaymentStatus Status     { get => _status;     set => _status     = value; }

    // ── Static actions ────────────────────────────────────────────────────────

    /// <summary>Charges the customer's wallet. Returns a Captured or Failed payment.</summary>
    public static Payment ChargeWallet(string userId, string orderId, decimal amount)
    {
        try
        {
            var order = AppState.Orders.SingleOrDefault(o => o.Id == orderId);
            if (order is null || order.CustomerId != userId)
                return new Payment(orderId, userId, amount, PaymentStatus.Failed);
            if (amount <= 0)
                return new Payment(orderId, userId, amount, PaymentStatus.Failed);

            var customer = AppState.Users.OfType<Customer>().SingleOrDefault(u => u.Id == userId);
            if (customer is null)
                return new Payment(orderId, userId, amount, PaymentStatus.Failed);

            if (customer.WalletBalance < amount)
                return new Payment(orderId, userId, amount, PaymentStatus.Failed);

            customer.WalletBalance -= amount;
            order.Status = OrderStatus.Paid;
            var payment = new Payment(orderId, userId, amount, PaymentStatus.Captured);
            AppState.Payments.Add(payment);
            return payment;
        }
        catch { return new Payment(orderId, userId, amount, PaymentStatus.Failed); }
    }

    /// <summary>Refunds a captured payment and credits the customer's wallet (admin only).</summary>
    public static bool Refund(string userId, string paymentId)
    {
        try
        {
            if (!AppState.Users.Any(u => u.Id == userId && u.Role == "Administrator")) return false;
            var payment = AppState.Payments.SingleOrDefault(p => p.Id == paymentId);
            if (payment is null || payment.Status != PaymentStatus.Captured) return false;

            var customer = AppState.Users.OfType<Customer>().SingleOrDefault(u => u.Id == payment.CustomerId);
            if (customer is not null) customer.WalletBalance += payment.Amount;

            payment.Status = PaymentStatus.Refunded;

            var order = AppState.Orders.SingleOrDefault(o => o.Id == payment.OrderId);
            if (order is not null) order.Status = OrderStatus.Cancelled;

            return true;
        }
        catch { return false; }
    }
}
