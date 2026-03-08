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
        string cs = Environment.GetEnvironmentVariable("CMD_SQLSERVER_CS")
                    ?? _devFallback;
        var conn = new SqlConnection(cs);
        conn.Open();
        return conn;
    }
}
