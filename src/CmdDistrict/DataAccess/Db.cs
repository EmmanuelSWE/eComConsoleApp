using Microsoft.Data.SqlClient;

namespace cmdDistrict.DataAccess;

/// <summary>
/// ADO.NET connection factory.
/// Reads the connection string from the CMD_SQLSERVER_CS environment variable.
/// Falls back to a local Docker dev string when the variable is absent.
/// </summary>
public static class Db
{
    private const string _devFallback =
        "Server=localhost,1433;Database=CmdDistrict;User Id=sa;" +
        "Password=CookAzureDBAshimwe@B0x;Encrypt=True;TrustServerCertificate=True;";

    /// <summary>Returns an open <see cref="SqlConnection"/>.</summary>
    public static SqlConnection Open()
    {
        Console.WriteLine($"connection type : ADO.NET → Microsoft.Data.SqlClient");
        string cs = Environment.GetEnvironmentVariable("CMD_SQLSERVER_CS")
                    ?? _devFallback;
        var conn = new SqlConnection(cs);
        try
        {
            conn.Open();
            Console.WriteLine($"connection success or failure : success");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"connection success or failure : failure — {ex.Message}");
            throw;
        }
        return conn;
    }
}
