using cmdDistrict.Common;
using cmdDistrict.DataAccess;
using cmdDistrict.Models.Entities;

namespace cmdDistrict.Infrastructure.Stores.Ef;

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
            Console.WriteLine($"function name is : {nameof(Login)}");
            Console.WriteLine($"Arguments are : email={email}, password=[hidden]");
            Console.WriteLine($"expected return : bool");
            using var ctx = new AppDbContext();
        
            var user = ctx.Users.SingleOrDefault(u =>
                u.Email.ToLower() == email.ToLower() &&
                u.Password == password);

            if (user is null)
            {
                Console.WriteLine($"actual return : false");
                Console.WriteLine($"Outcome : failed");
                return false;
            }

            Session.Set(user.Id, user.Role);
            Console.WriteLine($"actual return : true");
            Console.WriteLine($"Outcome : passed");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Outcome : encountered an error - {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Registers a new user (checks email uniqueness first).
    /// On success: persists to DB, sets Session(newId, role) and returns true.
    /// </summary>
    public static bool Sign(string name, string email, string password, string role)
    {
        try
        {
            Console.WriteLine($"function name is : {nameof(Sign)}");
            Console.WriteLine($"Arguments are : name={name}, email={email}, role={role}");
            Console.WriteLine($"expected return : bool");
            using var ctx = new AppDbContext();

            bool exists = ctx.Users.Any(u => u.Email.ToLower() == email.ToLower());
            if (exists)
            {
                Console.WriteLine($"actual return : false");
                Console.WriteLine($"Outcome : failed");
                return false;
            }

            User user = role.Trim().Equals("Administrator", StringComparison.OrdinalIgnoreCase)
                ? new Administrator(name, email, password)
                : new Customer(name, email, password);

            ctx.Users.Add(user);
           
try
{
    ctx.Users.Add(user);
    ctx.SaveChanges(); // <-- if this throws, you will see provider inner exception
}
catch (Exception ex)
{
    Console.WriteLine("EF FULL EXCEPTION:");
    Console.WriteLine(ex.ToString());
    throw; // optionally rethrow so you can see it in the console too
}


            Session.Set(user.Id, user.Role);
            Console.WriteLine($"actual return : true");
            Console.WriteLine($"Outcome : passed");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Outcome : encountered an error - {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Looks up an existing user by Id and refreshes Session(userId, role).
    /// Returns false when the user is not found.
    /// </summary>
    public static bool ResolveRoleFor(string userId)
    {
        try
        {
            Console.WriteLine($"function name is : {nameof(ResolveRoleFor)}");
            Console.WriteLine($"Arguments are : userId={userId}");
            Console.WriteLine($"expected return : bool");
            using var ctx = new AppDbContext();
            var user = ctx.Users.SingleOrDefault(u => u.Id == userId);

            if (user is null)
            {
                Console.WriteLine($"actual return : false");
                Console.WriteLine($"Outcome : failed");
                return false;
            }

            Session.Set(user.Id, user.Role);
            Console.WriteLine($"actual return : true");
            Console.WriteLine($"Outcome : passed");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Outcome : encountered an error - {ex.Message}");
            return false;
        }
    }
}
