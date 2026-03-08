namespace cmdDistrict.Models.Entities;

public class Product
{
    private string  _id;
    private string  _name;
    private string  _description;
    private decimal _price;
    private int     _stock;

    public Product(string name, string description, decimal price, int stock)
    {
        _id          = Guid.NewGuid().ToString();
        _name        = name;
        _description = description;
        _price       = price;
        _stock       = stock;
    }

    public string  Id          => _id;
    public string  Name        { get => _name;        set => _name        = value; }
    public string  Description { get => _description; set => _description = value; }
    public decimal Price       { get => _price;       set => _price       = value; }
    public int     Stock       { get => _stock;       set => _stock       = value; }
}
