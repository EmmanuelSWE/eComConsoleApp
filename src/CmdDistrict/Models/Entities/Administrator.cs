namespace cmdDistrict.Models.Entities;

public class Administrator : User
{
    private string _permissionLevel;

    public Administrator(string name, string email, string password,
                         string permissionLevel = "standard")
        : base(name, email, password, "Administrator")
    {
        _permissionLevel = permissionLevel;
    }

    public string PermissionLevel
    {
        get => _permissionLevel;
        set => _permissionLevel = value;
    }
}
