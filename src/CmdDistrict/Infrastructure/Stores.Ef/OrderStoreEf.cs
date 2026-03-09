using cmdDistrict.DataAccess;
using cmdDistrict.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace cmdDistrict.Infrastructure.Stores.Ef;

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
            if (cart.CustomerId != userId)
            {
                Console.WriteLine($"Outcome : failed");
                return null;
            }
            if (!cart.Items.Any())
            {
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
            Console.WriteLine($"Outcome : passed");
            Console.WriteLine($"EntityMade : Order : {order.Id}");
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
            using var ctx = new AppDbContext();
            var order = ctx.Orders.Include(o => o.Items)
                                   .SingleOrDefault(o => o.Id == orderId);
            if (order is null || order.CustomerId != userId)
            {
                Console.WriteLine($"Outcome : failed");
                return false;
            }
            if (order.Status != OrderStatus.Pending && order.Status != OrderStatus.Paid)
            {
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
        using var ctx = new AppDbContext();

        var order = ctx.Orders.ToList().FirstOrDefault(o => o.Id == orderId);
        if (order is null)
        {
            Console.WriteLine("Outcome : failed");
            return false;
        }
        
        order.Status = newStatus;
        ctx.SaveChanges();

        Console.WriteLine("Outcome : passed");
        return true;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Outcome : encountered an error - {ex.Message}");
        return false;
    }
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
        Console.WriteLine($"Outcome : passed");
        return order.Status;
    }
}
