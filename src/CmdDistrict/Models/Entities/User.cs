using cmdDistrict.Common;

namespace cmdDistrict.Models.Entities;

public abstract class User
{
    private string _id;
    private string _name;
    private string _email;
    private string _password;
    private string _role;

    protected User(string name, string email, string password, string role)
    {
        _id       = $"{DateProvider.UtcNow:yyyyMMddHHmmss}-{new Random().Next(100000, 999999)}";
        _name     = name;
        _email    = email;
        _password = password;
        _role     = role;
    }

    public string Id       => _id;
    public string Name     => _name;
    public string Email    => _email;
    public string Password => _password;
    public string Role     => _role;
}
