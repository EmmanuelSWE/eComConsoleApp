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

    public string        Id         => _id;
    public string        OrderId    => _orderId;
    public string        CustomerId => _customerId;
    public decimal       Amount     => _amount;
    public PaymentStatus Status     { get => _status; set => _status = value; }
}
