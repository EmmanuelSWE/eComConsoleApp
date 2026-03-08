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

    public string  Id          { get => _id;          set => _id          = value; }
    public string  ProductId   { get => _productId;   set => _productId   = value; }
    public string  ProductName { get => _productName; set => _productName = value; }
    public decimal UnitPrice   { get => _unitPrice;   set => _unitPrice   = value; }
    public int     Quantity    { get => _quantity;    set => _quantity    = value; }

    /// <summary>Computed line total — UnitPrice × Quantity.</summary>
    public decimal LineTotal => _unitPrice * _quantity;
}
