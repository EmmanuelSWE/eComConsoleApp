using cmdDistrict.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace cmdDistrict.DataAccess;

/// <summary>
/// EF Core DbContext for Cmd District.
/// Connection string is read from the CMD_SQLSERVER_CS environment variable;
/// falls back to a local Docker dev string when the variable is absent.
/// </summary>
public class AppDbContext : DbContext
{
    private const string _devFallback =
        "Server=localhost,1433;Database=CmdDistrict;User Id=sa;" +
        "Password=CookAzureDBAshimwe@B0x;Encrypt=True;TrustServerCertificate=True;";

    // ── DbSets ────────────────────────────────────────────────────────────────

    public DbSet<User>      Users     { get; set; } = null!;
    public DbSet<Product>   Products  { get; set; } = null!;
    public DbSet<Cart>      Carts     { get; set; } = null!;
    public DbSet<CartItem>  CartItems { get; set; } = null!;
    public DbSet<Order>     Orders    { get; set; } = null!;
    public DbSet<OrderItem> OrderItems{ get; set; } = null!;
    public DbSet<Payment>   Payments  { get; set; } = null!;
    public DbSet<Review>    Reviews   { get; set; } = null!;

    // ── Configuration ─────────────────────────────────────────────────────────

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        string cs = Environment.GetEnvironmentVariable("CMD_SQLSERVER_CS")
                    ?? _devFallback;

        options.UseSqlServer(cs)
        .EnableDetailedErrors()          // richer stack traces & provider messages
        .EnableSensitiveDataLogging();

    }

    /// <summary>Prints the EF Core connection type and whether SQL Server is reachable.</summary>
    public static void CheckConnection()
    {
        Console.WriteLine($"connection type : EF Core → Microsoft.EntityFrameworkCore.SqlServer");
        try
        {
            using var ctx = new AppDbContext();
            bool ok = ctx.Database.CanConnect();
            Console.WriteLine($"connection success or failure : {(ok ? "success" : "failure")}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"connection success or failure : failure — {ex.Message.Substring(0,20)}... ");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User: Table-Per-Hierarchy (single dbo.Users table, Role as discriminator)
        modelBuilder.Entity<User>()
            .HasDiscriminator<string>(u => u.Role)
            .HasValue<Customer>("Customer")
            .HasValue<Administrator>("Administrator");

        
   modelBuilder.Entity<User>()
        .Property(u => u.Role)
        .IsRequired();


        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<User>().Property(u => u.Id).ValueGeneratedNever();
        modelBuilder.Entity<Product>().Property(p => p.Id).ValueGeneratedNever();
        modelBuilder.Entity<Cart>().Property(c => c.Id).ValueGeneratedNever();
        modelBuilder.Entity<CartItem>().Property(ci => ci.Id).ValueGeneratedNever();
        modelBuilder.Entity<Order>().Property(o => o.Id).ValueGeneratedNever();
        modelBuilder.Entity<OrderItem>().Property(oi => oi.Id).ValueGeneratedNever();
        modelBuilder.Entity<Payment>().Property(p => p.Id).ValueGeneratedNever();
        modelBuilder.Entity<Review>().Property(r => r.Id).ValueGeneratedNever();

        
 modelBuilder.Entity<Customer>();
    modelBuilder.Entity<Administrator>();

    }
}
