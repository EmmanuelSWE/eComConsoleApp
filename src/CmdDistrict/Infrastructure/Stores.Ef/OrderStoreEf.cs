using cmdDistrict.DataAccess;
using cmdDistrict.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace cmdDistrict.Infrastructure.StoresEf;

/// <summary>
/// EF Core LINQ-backed order store.
/// All public write methods return bool or Order?; throws only where the spec mandates.
/// </summary>
public static class OrderStoreEf
{
    // ── Write ─────────────────────────────────────────────────────────────────

    /// <summary>Snapshots the customer's cart into a new Order, persists it, then clears the cart.</summary>
    public static Order? PlaceFromCart(string userId, Cart cart)
    {
        try
        {
            if (cart.CustomerId != userId) return null;
            if (!cart.Items.Any()) return null;

            using var ctx = new AppDbContext();
            var order = new Order(userId);
            foreach (var ci in cart.Items)
                order.Items.Add(new OrderItem(ci.ProductId, ci.ProductName, ci.UnitPrice, ci.Quantity));
            order.Total = order.Items.Sum(i => i.LineTotal);

            ctx.Orders.Add(order);
            ctx.SaveChanges();

            CartStoreEf.Clear(userId); // restores stock and removes cart items
            return order;
        }
        catch { return null; }
    }

    /// <summary>Cancels a Pending or Paid order, restoring product stock.</summary>
    public static bool Cancel(string userId, string orderId)
    {
        try
        {
            using var ctx = new AppDbContext();
            var order = ctx.Orders.Include(o => o.Items)
                                   .SingleOrDefault(o => o.Id == orderId);
            if (order is null || order.CustomerId != userId) return false;
            if (order.Status != OrderStatus.Pending && order.Status != OrderStatus.Paid) return false;

            foreach (var item in order.Items)
            {
                var product = ctx.Products.SingleOrDefault(p => p.Id == item.ProductId);
                if (product is not null) product.Stock += item.Quantity;
            }
            order.Status = OrderStatus.Cancelled;
            ctx.SaveChanges();
            return true;
        }
        catch { return false; }
    }

    /// <summary>Advances or sets an order's status (admin or owner, following the allowed state machine).</summary>
    public static bool UpdateStatus(string userId, string orderId, OrderStatus newStatus)
    {
        try
        {
            using var ctx = new AppDbContext();
            var order = ctx.Orders.SingleOrDefault(o => o.Id == orderId);
            if (order is null) return false;

            bool isAdmin = ctx.Users.Any(u => u.Id == userId && u.Role == "Administrator");
            bool isOwner = order.CustomerId == userId;
            if (!isAdmin && !isOwner) return false;

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
            ctx.SaveChanges();
            return true;
        }
        catch { return false; }
    }

    // ── Read ──────────────────────────────────────────────────────────────────

    /// <summary>Returns the status of an order. Throws if not found or caller is not the owner.</summary>
    public static OrderStatus TrackStatus(string userId, string orderId)
    {
        using var ctx = new AppDbContext();
        var order = ctx.Orders.SingleOrDefault(o => o.Id == orderId)
            ?? throw new InvalidOperationException($"Order '{orderId}' not found.");
        if (order.CustomerId != userId)
            throw new InvalidOperationException("Access denied: order belongs to a different customer.");
        return order.Status;
    }
}
