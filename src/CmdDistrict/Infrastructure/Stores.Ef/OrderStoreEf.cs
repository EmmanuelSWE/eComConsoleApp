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
            Console.WriteLine($"function name is : {nameof(PlaceFromCart)}");
            Console.WriteLine($"Arguments are : userId={userId}, cart.Id={cart.Id}, cart.ItemCount={cart.Items.Count}");
            Console.WriteLine($"expected return : Order?");
            if (cart.CustomerId != userId)
            {
                Console.WriteLine($"actual return : null");
                Console.WriteLine($"Outcome : failed");
                return null;
            }
            if (!cart.Items.Any())
            {
                Console.WriteLine($"actual return : null");
                Console.WriteLine($"Outcome : failed");
                return null;
            }

            using var ctx = new AppDbContext();
            var order = new Order(userId);
            foreach (var ci in cart.Items)
                order.Items.Add(new OrderItem(ci.ProductId, ci.ProductName, ci.UnitPrice, ci.Quantity));
            order.Total = order.Items.Sum(i => i.LineTotal);

            ctx.Orders.Add(order);
            ctx.SaveChanges();

            CartStoreEf.Clear(userId); // restores stock and removes cart items
            Console.WriteLine($"actual return : Order.Id={order.Id}");
            Console.WriteLine($"Outcome : passed");
            return order;
        }
        catch
        {
            Console.WriteLine($"Outcome : encountered an error");
            return null;
        }
    }

    /// <summary>Cancels a Pending or Paid order, restoring product stock.</summary>
    public static bool Cancel(string userId, string orderId)
    {
        try
        {
            Console.WriteLine($"function name is : {nameof(Cancel)}");
            Console.WriteLine($"Arguments are : userId={userId}, orderId={orderId}");
            Console.WriteLine($"expected return : bool");
            using var ctx = new AppDbContext();
            var order = ctx.Orders.Include(o => o.Items)
                                   .SingleOrDefault(o => o.Id == orderId);
            if (order is null || order.CustomerId != userId)
            {
                Console.WriteLine($"actual return : false");
                Console.WriteLine($"Outcome : failed");
                return false;
            }
            if (order.Status != OrderStatus.Pending && order.Status != OrderStatus.Paid)
            {
                Console.WriteLine($"actual return : false");
                Console.WriteLine($"Outcome : failed");
                return false;
            }

            foreach (var item in order.Items)
            {
                var product = ctx.Products.SingleOrDefault(p => p.Id == item.ProductId);
                if (product is not null) product.Stock += item.Quantity;
            }
            order.Status = OrderStatus.Cancelled;
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

    /// <summary>Advances or sets an order's status (admin or owner, following the allowed state machine).</summary>
    public static bool UpdateStatus(string userId, string orderId, OrderStatus newStatus)
    {
        try
        {
            Console.WriteLine($"function name is : {nameof(UpdateStatus)}");
            Console.WriteLine($"Arguments are : userId={userId}, orderId={orderId}, newStatus={newStatus}");
            Console.WriteLine($"expected return : bool");
            using var ctx = new AppDbContext();
            var order = ctx.Orders.SingleOrDefault(o => o.Id == orderId);
            if (order is null)
            {
                Console.WriteLine($"actual return : false");
                Console.WriteLine($"Outcome : failed");
                return false;
            }

            bool isAdmin = ctx.Users.Any(u => u.Id == userId && u.Role == "Administrator");
            bool isOwner = order.CustomerId == userId;
            if (!isAdmin && !isOwner)
            {
                Console.WriteLine($"actual return : false");
                Console.WriteLine($"Outcome : failed");
                return false;
            }

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
            if (!allowed)
            {
                Console.WriteLine($"actual return : false");
                Console.WriteLine($"Outcome : failed");
                return false;
            }

            order.Status = newStatus;
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

    // ── Read ──────────────────────────────────────────────────────────────────

    /// <summary>Returns the status of an order. Throws if not found or caller is not the owner.</summary>
    public static OrderStatus TrackStatus(string userId, string orderId)
    {
        Console.WriteLine($"function name is : {nameof(TrackStatus)}");
        Console.WriteLine($"Arguments are : userId={userId}, orderId={orderId}");
        Console.WriteLine($"expected return : OrderStatus");
        using var ctx = new AppDbContext();
        var order = ctx.Orders.SingleOrDefault(o => o.Id == orderId)
            ?? throw new InvalidOperationException($"Order '{orderId}' not found.");
        if (order.CustomerId != userId)
            throw new InvalidOperationException("Access denied: order belongs to a different customer.");
        Console.WriteLine($"actual return : {order.Status}");
        Console.WriteLine($"Outcome : passed");
        return order.Status;
    }
}
