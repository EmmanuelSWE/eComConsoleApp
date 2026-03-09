using cmdDistrict.Common;
using cmdDistrict.Infrastructure.Stores.Ef;

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
            if (string.IsNullOrWhiteSpace(userId))
                throw new InvalidOperationException("userId must not be empty.");
            if (!UserStoreEf.ResolveRoleFor(userId))
                throw new InvalidOperationException($"User '{userId}' not found.");
            return Session.CurrentRole!;
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

            bool ok = UserStoreEf.Sign(name, email, password, role);
            if (!ok) return (false, "An account with that email already exists or registration failed.");

            return (true, Session.CurrentUserId!);
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

            bool ok = UserStoreEf.Login(email, password);
            if (!ok) return (false, "Email or password is incorrect.", "");

            return (true, Session.CurrentUserId!, Session.CurrentRole!);
        }
        catch (Exception ex) { return (false, ex.Message, ""); }
    }

    /// <summary>Clears the login session. GlobalMenuHolder also clears its own router state.</summary>
    public static bool Logout() { Session.Clear(); return true; }
}
