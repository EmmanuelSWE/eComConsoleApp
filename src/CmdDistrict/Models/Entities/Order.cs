using cmdDistrict.Common;
using cmdDistrict.Infrastructure.StoresEf;

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

    public string          Id         { get => _id;         set => _id         = value; }
    public string          CustomerId { get => _customerId; set => _customerId = value; }
    public List<OrderItem> Items      { get => _items;      set => _items      = value; }
    public decimal         Total      { get => _total;      set => _total      = value; }
    public OrderStatus     Status     { get => _status;     set => _status     = value; }
    public DateTime        CreatedAt  { get => _createdAt;  set => _createdAt  = value; }

    // ── Static actions ────────────────────────────────────────────────────

    /// <summary>Snapshots the customer's cart into a new Order and clears the cart.</summary>
    public static Order? PlaceFromCart(string userId, Cart cart)
        => OrderStoreEf.PlaceFromCart(userId, cart);

    /// <summary>Cancels a Pending or Paid order, restoring product stock.</summary>
    public static bool Cancel(string userId, string orderId)
        => OrderStoreEf.Cancel(userId, orderId);

    /// <summary>Returns the status of an order. Throws if not found or user is not the owner.</summary>
    public static OrderStatus TrackStatus(string userId, string orderId)
        => OrderStoreEf.TrackStatus(userId, orderId);

    /// <summary>Advances or sets an order's status following the allowed state machine (admin or owner).</summary>
    public static bool UpdateStatus(string userId, string orderId, OrderStatus newStatus)
        => OrderStoreEf.UpdateStatus(userId, orderId, newStatus);
}
