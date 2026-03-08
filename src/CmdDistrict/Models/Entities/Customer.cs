namespace cmdDistrict.Models.Entities;

public class Customer : User
{
    private decimal _walletBalance;
    private string? _defaultShippingAddress;

    public Customer(string name, string email, string password)
        : base(name, email, password, "Customer")
    {
        _walletBalance          = 0m;
        _defaultShippingAddress = null;
    }

    public decimal WalletBalance
    {
        get => _walletBalance;
        set => _walletBalance = value;
    }

    public string? DefaultShippingAddress
    {
        get => _defaultShippingAddress;
        set => _defaultShippingAddress = value;
    }
}
