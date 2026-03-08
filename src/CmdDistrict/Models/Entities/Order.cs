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

    public string          Id         { get => _id;         set => _id         = value; }
    public string          CustomerId { get => _customerId; set => _customerId = value; }
    public List<OrderItem> Items      { get => _items;      set => _items      = value; }
    public decimal         Total      { get => _total;      set => _total      = value; }
    public OrderStatus     Status     { get => _status;     set => _status     = value; }
    public DateTime        CreatedAt  { get => _createdAt;  set => _createdAt  = value; }

    // ── Static actions ────────────────────────────────────────────────────

    /// <summary>Snapshots the customer's cart into a new Order and clears the cart.</summary>
    public static Order? PlaceFromCart(string userId, Cart cart)
    {
        try
        {
            if (cart.CustomerId != userId) return null;
            if (!cart.Items.Any()) return null;

            var order = new Order(userId);
            foreach (var ci in cart.Items)
                order.Items.Add(new OrderItem(ci.ProductId, ci.ProductName, ci.UnitPrice, ci.Quantity));
            order.Total = order.Items.Sum(i => i.LineTotal);

            AppState.Orders.Add(order);
            Cart.Clear(userId);   // restores stock already done — Cart.Clear handles it
            return order;
        }
        catch { return null; }
    }

    /// <summary>Cancels a Pending or Paid order, restoring product stock.</summary>
    public static bool Cancel(string userId, string orderId)
    {
        try
        {
            var order = AppState.Orders.SingleOrDefault(o => o.Id == orderId);
            if (order is null || order.CustomerId != userId) return false;
            if (order.Status != OrderStatus.Pending && order.Status != OrderStatus.Paid) return false;

            foreach (var item in order.Items)
            {
                var product = AppState.Products.SingleOrDefault(p => p.Id == item.ProductId);
                if (product is not null) product.Stock += item.Quantity;
            }
            order.Status = OrderStatus.Cancelled;
            return true;
        }
        catch { return false; }
    }

    /// <summary>Returns the status of an order. Throws if not found or user is not the owner.</summary>
    public static OrderStatus TrackStatus(string userId, string orderId)
    {
        var order = AppState.Orders.SingleOrDefault(o => o.Id == orderId)
            ?? throw new InvalidOperationException($"Order '{orderId}' not found.");
        if (order.CustomerId != userId)
            throw new InvalidOperationException("Access denied: order belongs to a different customer.");
        return order.Status;
    }

    /// <summary>Advances or sets an order's status following the allowed state machine (admin or owner).</summary>
    public static bool UpdateStatus(string userId, string orderId, OrderStatus newStatus)
    {
        try
        {
            var order = AppState.Orders.SingleOrDefault(o => o.Id == orderId);
            if (order is null) return false;

            bool isAdmin = AppState.Users.Any(u => u.Id == userId && u.Role == "Administrator");
            bool isOwner = order.CustomerId == userId;
            if (!isAdmin && !isOwner) return false;

            // Allowed forward transitions
            bool allowed = (order.Status, newStatus) switch
            {
                (OrderStatus.Pending,   OrderStatus.Paid)      => true,
                (OrderStatus.Paid,      OrderStatus.Packed)    => isAdmin,
                (OrderStatus.Packed,    OrderStatus.Shipped)   => isAdmin,
                (OrderStatus.Shipped,   OrderStatus.Delivered) => isAdmin,
                (OrderStatus.Pending,   OrderStatus.Cancelled) => true,
                (OrderStatus.Paid,      OrderStatus.Cancelled) => isAdmin,
                _ => false
            };
            if (!allowed) return false;

            order.Status = newStatus;
            return true;
        }
        catch { return false; }
    }
}
