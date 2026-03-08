using cmdDistrict.Models.Entities;

namespace cmdDistrict.Common;

/// <summary>Single source of in-memory data for the process lifetime.</summary>
public static class AppState
{
    public static List<User>    Users    = new();
    public static List<Product> Products = new();
    public static List<Cart>    Carts    = new();
    public static List<Order>   Orders   = new();
    public static List<Review>  Reviews  = new();
    public static List<Payment> Payments = new();
}
