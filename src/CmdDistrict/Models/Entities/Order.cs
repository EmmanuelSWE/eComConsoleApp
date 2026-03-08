using cmdDistrict.Common;

namespace cmdDistrict.Models.Entities;

public class Order
{
    private string          _id;
    private string          _customerId;
    private List<OrderItem> _items;
    private decimal         _total;
    private OrderStatus     _status;
    private DateTime        _createdAt;

    public Order(string customerId)
    {
        _id         = Guid.NewGuid().ToString();
        _customerId = customerId;
        _items      = new List<OrderItem>();
        _total      = 0m;
        _status     = OrderStatus.Pending;
        _createdAt  = DateProvider.UtcNow;
    }

    public string          Id         => _id;
    public string          CustomerId => _customerId;
    public List<OrderItem> Items      => _items;
    public decimal         Total      { get => _total;  set => _total  = value; }
    public OrderStatus     Status     { get => _status; set => _status = value; }
    public DateTime        CreatedAt  => _createdAt;
}
