using cmdDistrict.Common;
using cmdDistrict.DataAccess;
using cmdDistrict.Models.Entities;

namespace cmdDistrict.Infrastructure.StoresEf;

/// <summary>
/// EF Core LINQ-backed user store.
/// All public methods return bool; on success they set Session internally.
/// </summary>
public static class UserStoreEf
{
    /// <summary>
    /// Authenticates by email + password (case-insensitive email).
    /// On success: sets Session(userId, role) and returns true.
    /// </summary>
    public static bool Login(string email, string password)
    {
        try
        {
            using var ctx = new AppDbContext();
            var user = ctx.Users.SingleOrDefault(u =>
                u.Email.ToLower() == email.ToLower() &&
                u.Password == password);

            if (user is null) return false;

            Session.Set(user.Id, user.Role);
            return true;
        }
        catch { return false; }
    }

    /// <summary>
    /// Registers a new user (checks email uniqueness first).
    /// On success: persists to DB, sets Session(newId, role) and returns true.
    /// </summary>
    public static bool Sign(string name, string email, string password, string role)
    {
        try
        {
            using var ctx = new AppDbContext();

            bool exists = ctx.Users.Any(u => u.Email.ToLower() == email.ToLower());
            if (exists) return false;

            User user = role.Trim().Equals("Administrator", StringComparison.OrdinalIgnoreCase)
                ? new Administrator(name, email, password)
                : new Customer(name, email, password);

            ctx.Users.Add(user);
            ctx.SaveChanges();

            Session.Set(user.Id, user.Role);
            return true;
        }
        catch { return false; }
    }

    /// <summary>
    /// Looks up an existing user by Id and refreshes Session(userId, role).
    /// Returns false when the user is not found.
    /// </summary>
    public static bool ResolveRoleFor(string userId)
    {
        try
        {
            using var ctx = new AppDbContext();
            var user = ctx.Users.SingleOrDefault(u => u.Id == userId);

            if (user is null) return false;

            Session.Set(user.Id, user.Role);
            return true;
        }
        catch { return false; }
    }
}
