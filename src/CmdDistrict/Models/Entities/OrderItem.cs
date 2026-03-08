namespace cmdDistrict.Models.Entities;

public class OrderItem
{
    private string  _id;
    private string  _productId;
    private string  _productName;
    private decimal _unitPrice;
    private int     _quantity;

    public OrderItem(string productId, string productName, decimal unitPrice, int quantity)
    {
        _id          = Guid.NewGuid().ToString();
        _productId   = productId;
        _productName = productName;
        _unitPrice   = unitPrice;
        _quantity    = quantity;
    }

    public string  Id          => _id;
    public string  ProductId   => _productId;
    public string  ProductName => _productName;
    public decimal UnitPrice   => _unitPrice;
    public int     Quantity    => _quantity;

    public decimal LineTotal => _unitPrice * _quantity;
}
