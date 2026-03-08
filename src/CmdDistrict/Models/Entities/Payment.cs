using cmdDistrict.Infrastructure.StoresEf;

namespace cmdDistrict.Models.Entities;

public class Payment
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

    // ── Static actions ────────────────────────────────────────────────────

    /// <summary>Charges the customer's wallet. Returns Captured or Failed payment.</summary>
    public static Payment ChargeWallet(string userId, string orderId, decimal amount)
        => PaymentStoreEf.ChargeWallet(userId, orderId, amount);

    /// <summary>Refunds a captured payment, crediting the customer's wallet (admin only).</summary>
    public static bool Refund(string userId, string paymentId)
        => PaymentStoreEf.Refund(userId, paymentId);
}
