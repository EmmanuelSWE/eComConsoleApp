namespace cmdDistrict.Models.Entities;

public enum OrderStatus
{
    Pending,
    Paid,
    Packed,
    Shipped,
    Delivered,
    Cancelled
}

public enum PaymentStatus
{
    Captured,
    Failed,
    Refunded
}
