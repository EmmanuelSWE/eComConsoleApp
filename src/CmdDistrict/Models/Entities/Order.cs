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

    // ── Static actions ────────────────────────────────────────────────────────

    /// <summary>Snapshots the cart into a new Order and saves it. Stock already decremented at cart add-time.</summary>
    public static Order? PlaceFromCart(string userId, Cart cart)
    {
        try
        {
            if (cart.CustomerId != userId) throw new InvalidOperationException("Cart does not belong to this user.");
            if (!cart.Items.Any()) return null;

            var order = new Order(userId);
            foreach (var item in cart.Items)
                order.Items.Add(new OrderItem(item.ProductId, item.ProductName, item.UnitPrice, item.Quantity));

            order.Total = order.Items.Sum(i => i.LineTotal);
            AppState.Orders.Add(order);
            return order;
        }
        catch (InvalidOperationException) { throw; }
        catch { return null; }
    }

    /// <summary>Cancels an order in Pending or Paid state, restocking products.</summary>
    public static bool Cancel(string userId, string orderId)
    {
        try
        {
            var order = AppState.Orders.SingleOrDefault(o => o.Id == orderId);
            if (order is null) return false;

            bool isAdmin = AppState.Users.Any(u => u.Id == userId && u.Role == "Administrator");
            if (order.CustomerId != userId && !isAdmin) return false;
            if (order.Status != OrderStatus.Pending && order.Status != OrderStatus.Paid) return false;

            // Restock
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

    /// <summary>Returns the current status of an order.</summary>
    public static OrderStatus TrackStatus(string userId, string orderId)
    {
        try
        {
            var order = AppState.Orders.SingleOrDefault(o => o.Id == orderId);
            if (order is null) throw new InvalidOperationException("Order not found.");
            bool isAdmin = AppState.Users.Any(u => u.Id == userId && u.Role == "Administrator");
            if (order.CustomerId != userId && !isAdmin) throw new InvalidOperationException("Access denied.");
            return order.Status;
        }
        catch (InvalidOperationException) { throw; }
        catch (Exception ex) { throw new InvalidOperationException(ex.Message); }
    }

    /// <summary>Advances an order's status using the allowed state machine (admin only).</summary>
    public static bool UpdateStatus(string userId, string orderId, OrderStatus newStatus)
    {
        try
        {
            if (!AppState.Users.Any(u => u.Id == userId && u.Role == "Administrator")) return false;
            var order = AppState.Orders.SingleOrDefault(o => o.Id == orderId);
            if (order is null) return false;

            bool valid = (order.Status, newStatus) switch
            {
                (OrderStatus.Pending,   OrderStatus.Paid)       => true,
                (OrderStatus.Pending,   OrderStatus.Cancelled)  => true,
                (OrderStatus.Paid,      OrderStatus.Packed)     => true,
                (OrderStatus.Paid,      OrderStatus.Cancelled)  => true,
                (OrderStatus.Packed,    OrderStatus.Shipped)    => true,
                (OrderStatus.Shipped,   OrderStatus.Delivered)  => true,
                _ => false
            };
            if (!valid) return false;
            order.Status = newStatus;
            return true;
        }
        catch { return false; }
    }
}
