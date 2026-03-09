using cmdDistrict.Models.Entities; 

namespace cmdDistrict.Infrastructure.Stores.Ef;

public static class Helper
{
   public static string ConvStatus(OrderStatus status)
    {
        
        switch (status)
        {
            case OrderStatus.Pending:
                return "Pending";
            case OrderStatus.Paid:
                return "Paid";
            case OrderStatus.Packed:
                return "Packed";
            case OrderStatus.Shipped:
                return "Shipped";
            case OrderStatus.Delivered:
                return "Delivered";
            case OrderStatus.Cancelled:
                return "Cancelled";
            default:
                throw new ArgumentException("Invalid order status");
        }
    }

    public static OrderStatus ConvToOrderStatus(string status)
    {
        switch (status)
        {
            case "Pending":
                return OrderStatus.Pending;
            case "Paid":
                return OrderStatus.Paid;
            case "Packed":
                return OrderStatus.Packed;
            case "Shipped":
                return OrderStatus.Shipped;
            case "Delivered":
                return OrderStatus.Delivered;
            case "Cancelled":
                return OrderStatus.Cancelled;
            default:
                throw new ArgumentException("Invalid order status string");
        }
    }

    public static string ConvStatus(PaymentStatus status)
    {
        switch (status)
        {
            case PaymentStatus.Captured:
                return "Captured";
            case PaymentStatus.Failed:
                return "Failed";
            case PaymentStatus.Refunded:
                return "Refunded";
            default:
                throw new ArgumentException("Invalid payment status");
        }
    }

    public static PaymentStatus ConvToPaymentStatus(string status)
    {
        switch (status)
        {
            case "Captured":
                return PaymentStatus.Captured;
            case "Failed":
                return PaymentStatus.Failed;
            case "Refunded":
                return PaymentStatus.Refunded;
            default:
                throw new ArgumentException("Invalid payment status string");
        }
    }
}