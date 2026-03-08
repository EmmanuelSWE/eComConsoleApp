using cmdDistrict.Common;

namespace cmdDistrict.Models.Entities;

/// <summary>Abstract base for all user roles.</summary>
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

    public string Id       { get => _id;       set => _id       = value; }
    public string Name     { get => _name;     set => _name     = value; }
    public string Email    { get => _email;    set => _email    = value; }
    public string Password { get => _password; set => _password = value; }
    public string Role     { get => _role;     set => _role     = value; }

    // ── Static actions ────────────────────────────────────────────────────────

    /// <summary>Returns the role string for a given userId.</summary>
    public static string GetRole(string userId)
    {
        try
        {
            var user = AppState.Users.SingleOrDefault(u => u.Id == userId);
            if (user is null) throw new InvalidOperationException($"User '{userId}' not found.");
            return user.Role;
        }
        catch (InvalidOperationException) { throw; }
        catch (Exception ex) { throw new InvalidOperationException(ex.Message); }
    }

    /// <summary>Registers a new user. Returns (true, userId) or (false, errorMessage).</summary>
    public static (bool ok, string userId) Sign(string name, string email, string password, string role)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                return (false, "Name, email, and password must not be empty.");

            if (AppState.Users.Any(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase)))
                return (false, "An account with that email already exists.");

            User user = role.Trim().Equals("Administrator", StringComparison.OrdinalIgnoreCase)
                ? new Administrator(name, email, password)
                : new Customer(name, email, password);

            AppState.Users.Add(user);

            if (user is Customer)
                AppState.Carts.Add(new Cart(user.Id));

            return (true, user.Id);
        }
        catch (Exception ex) { return (false, ex.Message); }
    }

    /// <summary>Authenticates a user. Returns (true, userId, role) or (false, errorMessage, "").</summary>
    public static (bool ok, string userId, string role) Login(string email, string password)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                return (false, "Email and password must not be empty.", "");

            var user = AppState.Users.SingleOrDefault(u =>
                u.Email.Equals(email, StringComparison.OrdinalIgnoreCase) &&
                u.Password == password);

            if (user is null)
                return (false, "Email or password is incorrect.", "");

            return (true, user.Id, user.Role);
        }
        catch (Exception ex) { return (false, ex.Message, ""); }
    }

    /// <summary>No-op in entity layer — GlobalMenuHolder.SignOut() handles clearing context.</summary>
    public static bool Logout() => true;
}
