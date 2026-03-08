using cmdDistrict.Common;

namespace cmdDistrict.Models.Entities;

public class CartItem
{
    private string  _id;
    private string  _productId;
    private string  _productName;
    private decimal _unitPrice;
    private int     _quantity;

    public CartItem(string productId, string productName, decimal unitPrice, int quantity)
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

    // ── Static actions ────────────────────────────────────────────────────

    /// <summary>Changes a cart line quantity, adjusting stock for the delta.</summary>
    public static bool UpdateQuantity(string userId, string productId, int newQuantity)
    {
        try
        {
            if (newQuantity <= 0) return false;
            var cart = AppState.Carts.SingleOrDefault(c => c.CustomerId == userId);
            if (cart is null) return false;
            var item = cart.Items.SingleOrDefault(i => i.ProductId == productId);
            if (item is null) return false;
            var product = AppState.Products.SingleOrDefault(p => p.Id == productId);
            if (product is null) return false;

            var delta = newQuantity - item.Quantity; // +ve means we need more stock
            if (delta > 0 && product.Stock < delta) return false;
            product.Stock -= delta;
            item.Quantity  = newQuantity;
            return true;
        }
        catch { return false; }
    }
}
