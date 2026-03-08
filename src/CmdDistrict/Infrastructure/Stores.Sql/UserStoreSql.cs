using cmdDistrict.Common;
using cmdDistrict.DataAccess;
using Microsoft.Data.SqlClient;

namespace cmdDistrict.Infrastructure.StoresSql;

/// <summary>
/// SQL Server-backed user store (ADO.NET, parameterized queries only).
/// All methods return bool; on success they set Session internally.
/// </summary>
public static class UserStoreSql
{
    /// <summary>
    /// Authenticates by email + password.
    /// On success: sets Session(userId, role) and returns true.
    /// </summary>
    public static bool Login(string email, string password)
    {
        try
        {
            using var conn = Db.Open();
            using var cmd  = new SqlCommand(
                "SELECT Id, Role FROM dbo.Users WHERE Email = @Email AND Password = @Password",
                conn);
            cmd.Parameters.AddWithValue("@Email",    email);
            cmd.Parameters.AddWithValue("@Password", password);

            using var reader = cmd.ExecuteReader();
            if (!reader.Read()) return false;

            Session.Set(reader.GetString(0), reader.GetString(1));
            return true;
        }
        catch { return false; }
    }

    /// <summary>
    /// Registers a new user (email uniqueness checked first).
    /// On success: inserts row, sets Session(newId, role) and returns true.
    /// </summary>
    public static bool Sign(string name, string email, string password, string role)
    {
        try
        {
            using var conn = Db.Open();

            // Uniqueness check
            using (var checkCmd = new SqlCommand(
                "SELECT COUNT(1) FROM dbo.Users WHERE Email = @Email", conn))
            {
                checkCmd.Parameters.AddWithValue("@Email", email);
                if ((int)checkCmd.ExecuteScalar()! > 0) return false;
            }

            var newId          = Guid.NewGuid().ToString();
            var normalizedRole = role.Trim().Equals("Administrator", StringComparison.OrdinalIgnoreCase)
                                 ? "Administrator" : "Customer";

            using var insertCmd = new SqlCommand(
                "INSERT INTO dbo.Users (Id, Name, Email, Password, Role) " +
                "VALUES (@Id, @Name, @Email, @Password, @Role)",
                conn);
            insertCmd.Parameters.AddWithValue("@Id",       newId);
            insertCmd.Parameters.AddWithValue("@Name",     name);
            insertCmd.Parameters.AddWithValue("@Email",    email);
            insertCmd.Parameters.AddWithValue("@Password", password);
            insertCmd.Parameters.AddWithValue("@Role",     normalizedRole);
            insertCmd.ExecuteNonQuery();

            Session.Set(newId, normalizedRole);
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
            using var conn = Db.Open();
            using var cmd  = new SqlCommand(
                "SELECT Role FROM dbo.Users WHERE Id = @Id", conn);
            cmd.Parameters.AddWithValue("@Id", userId);

            var result = cmd.ExecuteScalar();
            if (result is null or DBNull) return false;

            Session.Set(userId, (string)result);
            return true;
        }
        catch { return false; }
    }
}
