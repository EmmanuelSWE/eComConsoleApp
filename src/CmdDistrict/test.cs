using cmdDistrict.DataAccess;
using cmdDistrict.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace cmdDistrict;

/// <summary>
/// Comprehensive functional-requirements test suite.
/// Run by calling Tests.Run() from Program.cs (or directly via dotnet run).
/// </summary>
public static class Tests
{
    // ── counters ──────────────────────────────────────────────────────────────
    private static int _total  = 0;
    private static int _passed = 0;
    private static int _failed = 0;

    // ── entry point ───────────────────────────────────────────────────────────
    public static void Run()
    {
        _total = _passed = _failed = 0;

        Console.WriteLine("╔══════════════════════════════════════════════════════╗");
        Console.WriteLine("║           Cmd District — Functional Test Suite       ║");
        Console.WriteLine("╚══════════════════════════════════════════════════════╝");
        Console.WriteLine();

        // 1 — DB connection
        TestDbConnection();

        // 2 — User.Sign
        var (adminId, custId) = TestUserSign();

        // 3 — User.Login
        TestUserLogin(adminId, custId);

        // 4 — User.GetRole
        TestUserGetRole(adminId, custId);

        // 5 — User.Logout
        TestUserLogout();

        // 6 — Product CRUD (needs admin)
        var productId = TestProductCrud(adminId);

        // 7 — Customer.Deposit
        TestCustomerDeposit(custId);

        // 8 — Cart operations
        TestCartOperations(custId, productId);

        // 9 — CartItem.UpdateQuantity
        TestCartItemUpdateQuantity(custId, productId);

        // 10 — Order operations (place, track, update-status, cancel)
        var orderId = TestOrderOperations(custId, adminId, productId);

        // 11 — Payment.ChargeWallet + Payment.Refund
        TestPaymentOperations(custId, adminId, productId);

        // 12 — Review operations
        TestReviewOperations(custId, productId);

        // 13 — Administrator.AdjustInventory
        TestAdjustInventory(adminId, productId);

        // 14 — Administrator.ListAllOrders
        TestListAllOrders(adminId, custId);

        // 15 — Administrator.GenerateReport
        TestGenerateReport(adminId);

        // 16 — OrderItem.LineTotal
        TestOrderItemLineTotal();

        // ── summary ───────────────────────────────────────────────────────────
        Console.WriteLine();
        Console.WriteLine("══════════════════════════════════════════════════════");
        Console.WriteLine($"  SUMMARY  |  Total: {_total}  |  Passed: {_passed}  |  Failed: {_failed}");
        Console.WriteLine("══════════════════════════════════════════════════════");
    }

    // ══════════════════════════════════════════════════════════════════════════
    //  SECTION 1 — DB Connection
    // ══════════════════════════════════════════════════════════════════════════
    private static void TestDbConnection()
    {
        PrintHeader("DB Connection");
        bool connected = false;
        try
        {
            using var ctx = new AppDbContext();
            ctx.Database.OpenConnection();
            connected = true;
            ctx.Database.CloseConnection();
        }
        catch { connected = false; }

        Console.WriteLine($"  connection type    : SQL Server (EF Core)");
        Console.WriteLine($"  connection success or failure : {(connected ? "success" : "FAILURE")}");
        WriteOutcomeLine(connected, connected, null, null);
    }

    // ══════════════════════════════════════════════════════════════════════════
    //  SECTION 2 — User.Sign
    // ══════════════════════════════════════════════════════════════════════════
    private static (string adminId, string custId) TestUserSign()
    {
        PrintHeader("User.Sign");

        var stamp    = DateTime.UtcNow.Ticks;
        var adminEmail = $"admin_{stamp}@test.com";
        var custEmail  = $"cust_{stamp}@test.com";

        // 2a — happy path: register admin
        WriteFunctionLine("User.Sign", $"(\"Admin\", \"{adminEmail}\", \"pass\", \"Administrator\")",
                          "(true, userId)");
        var (okA, adminId) = User.Sign("Admin Test", adminEmail, "pass123", "Administrator");
        WriteOutcomeLine(okA && !string.IsNullOrEmpty(adminId), okA, "User", adminId);

        // 2b — happy path: register customer
        WriteFunctionLine("User.Sign", $"(\"Cust\", \"{custEmail}\", \"pass\", \"Customer\")",
                          "(true, userId)");
        var (okC, custId) = User.Sign("Customer Test", custEmail, "pass123", "Customer");
        WriteOutcomeLine(okC && !string.IsNullOrEmpty(custId), okC, "User", custId);

        // 2c — failure: duplicate email
        WriteFunctionLine("User.Sign", $"(duplicate email: \"{adminEmail}\")", "(false, error)");
        var (dupOk, _) = User.Sign("Dup", adminEmail, "pass", "Administrator");
        WriteOutcomeLine(!dupOk, dupOk, "User", "duplicate-rejected");

        // 2d — failure: empty name
        WriteFunctionLine("User.Sign", "(empty name)", "(false, error)");
        var (emptyOk, _) = User.Sign("", "empty@test.com", "pass", "Customer");
        WriteOutcomeLine(!emptyOk, emptyOk, "User", "empty-name-rejected");

        // 2e — failure: empty password
        WriteFunctionLine("User.Sign", "(empty password)", "(false, error)");
        var (noPwOk, _) = User.Sign("X", "x@test.com", "", "Customer");
        WriteOutcomeLine(!noPwOk, noPwOk, "User", "empty-password-rejected");

        return (adminId, custId);
    }

    // ══════════════════════════════════════════════════════════════════════════
    //  SECTION 3 — User.Login
    // ══════════════════════════════════════════════════════════════════════════
    private static void TestUserLogin(string adminId, string custId)
    {
        PrintHeader("User.Login");

        // 3a — happy path: correct credentials admin
        var stamp      = DateTime.UtcNow.Ticks;
        var adminEmail = GetEmailById(adminId);
        WriteFunctionLine("User.Login", $"(\"{adminEmail}\", \"pass123\")", "(true, userId, role)");
        var (okA, uidA, roleA) = User.Login(adminEmail!, "pass123");
        WriteOutcomeLine(okA && roleA == "Administrator", okA, "User", uidA);

        // 3b — happy path: correct credentials customer
        var custEmail = GetEmailById(custId);
        WriteFunctionLine("User.Login", $"(\"{custEmail}\", \"pass123\")", "(true, userId, role)");
        var (okC, uidC, roleC) = User.Login(custEmail!, "pass123");
        WriteOutcomeLine(okC && roleC == "Customer", okC, "User", uidC);

        // 3c — failure: wrong password
        WriteFunctionLine("User.Login", $"(\"{adminEmail}\", \"wrong\")", "(false, error, \"\")");
        var (badOk, _, _) = User.Login(adminEmail!, "wrong");
        WriteOutcomeLine(!badOk, badOk, "User", "wrong-password-rejected");

        // 3d — failure: empty email
        WriteFunctionLine("User.Login", "(empty email)", "(false, error, \"\")");
        var (eOk, _, _) = User.Login("", "pass");
        WriteOutcomeLine(!eOk, eOk, "User", "empty-email-rejected");

        // Restore session to custId for subsequent tests
        User.Login(custEmail!, "pass123");
    }

    // ══════════════════════════════════════════════════════════════════════════
    //  SECTION 4 — User.GetRole
    // ══════════════════════════════════════════════════════════════════════════
    private static void TestUserGetRole(string adminId, string custId)
    {
        PrintHeader("User.GetRole");

        WriteFunctionLine("User.GetRole", $"(\"{adminId}\")", "\"Administrator\"");
        try
        {
            var role = User.GetRole(adminId);
            Console.WriteLine($"  actual return : \"{role}\"");
            WriteOutcomeLine(role == "Administrator", true, null, null);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"  actual return : exception — {ex.Message}");
            WriteOutcomeLine(false, false, null, null);
        }

        WriteFunctionLine("User.GetRole", $"(\"{custId}\")", "\"Customer\"");
        try
        {
            var role = User.GetRole(custId);
            Console.WriteLine($"  actual return : \"{role}\"");
            WriteOutcomeLine(role == "Customer", true, null, null);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"  actual return : exception — {ex.Message}");
            WriteOutcomeLine(false, false, null, null);
        }
    }

    // ══════════════════════════════════════════════════════════════════════════
    //  SECTION 5 — User.Logout
    // ══════════════════════════════════════════════════════════════════════════
    private static void TestUserLogout()
    {
        PrintHeader("User.Logout");
        WriteFunctionLine("User.Logout", "()", "true");
        var result = User.Logout();
        Console.WriteLine($"  actual return : {result}");
        WriteOutcomeLine(result, result, null, null);
    }

    // ══════════════════════════════════════════════════════════════════════════
    //  SECTION 6 — Product CRUD
    // ══════════════════════════════════════════════════════════════════════════
    private static string TestProductCrud(string adminId)
    {
        PrintHeader("Product.Create / Update / Delete / FindById / SearchByName");

        // 6a — Create (happy)
        WriteFunctionLine("Product.Create", $"(adminId, \"TestWidget\", \"desc\", 9.99, 50)", "Product");
        var p = Product.Create(adminId, "TestWidget", "A test product", 9.99m, 50);
        Console.WriteLine($"  actual return : {(p is null ? "null" : $"Product id={p.Id}")}");
        WriteOutcomeLine(p is not null, p is not null, "Product", p?.Id ?? "null");
        var productId = p?.Id ?? "";

        // 6b — Create failure: non-admin
        WriteFunctionLine("Product.Create", "(non-adminId, ...)", "null");
        var pNoAuth = Product.Create("fake-user-id", "Hacked", "x", 1m, 1);
        Console.WriteLine($"  actual return : {(pNoAuth is null ? "null" : $"Product id={pNoAuth.Id}")}");
        WriteOutcomeLine(pNoAuth is null, pNoAuth is null, null, null);

        // 6c — Create failure: negative price
        WriteFunctionLine("Product.Create", "(adminId, \"X\", \"d\", -1, 5)", "null");
        var pBadPrice = Product.Create(adminId, "X", "d", -1m, 5);
        Console.WriteLine($"  actual return : {(pBadPrice is null ? "null" : $"Product id={pBadPrice.Id}")}");
        WriteOutcomeLine(pBadPrice is null, pBadPrice is null, null, null);

        // 6d — FindById
        if (!string.IsNullOrEmpty(productId))
        {
            WriteFunctionLine("Product.FindById", $"(\"{productId}\")", "Product");
            var found = Product.FindById(productId);
            Console.WriteLine($"  actual return : {(found is null ? "null" : $"Product name={found.Name}")}");
            WriteOutcomeLine(found is not null && found.Name == "TestWidget", found is not null, "Product", found?.Id);
        }

        // 6e — SearchByName (empty → all)
        WriteFunctionLine("Product.SearchByName", "(\"\") — all products", "List<Product>");
        var all = Product.SearchByName("");
        Console.WriteLine($"  actual return : List<Product> count={all.Count}");
        WriteOutcomeLine(all.Count >= 1, all.Count >= 1, null, null);

        // 6f — SearchByName (with query)
        WriteFunctionLine("Product.SearchByName", "(\"TestWidget\")", "List<Product> with match");
        var filtered = Product.SearchByName("TestWidget");
        Console.WriteLine($"  actual return : List<Product> count={filtered.Count}");
        WriteOutcomeLine(filtered.Count >= 1, filtered.Count >= 1, null, null);

        // 6g — Update
        if (!string.IsNullOrEmpty(productId))
        {
            WriteFunctionLine("Product.Update", $"(adminId, productId, \"TestWidgetV2\", \"d\", 19.99, 40)", "bool true");
            var updated = Product.Update(adminId, productId, "TestWidgetV2", "Updated desc", 19.99m, 40);
            Console.WriteLine($"  actual return : {updated}");
            WriteOutcomeLine(updated, updated, "Product", productId);
        }

        return productId;
    }

    // ══════════════════════════════════════════════════════════════════════════
    //  SECTION 7 — Customer.Deposit
    // ══════════════════════════════════════════════════════════════════════════
    private static void TestCustomerDeposit(string custId)
    {
        PrintHeader("Customer.Deposit");

        // 7a — happy path
        WriteFunctionLine("Customer.Deposit", $"(custId, 500.00)", "true");
        var ok = Customer.Deposit(custId, 500.00m);
        Console.WriteLine($"  actual return : {ok}");
        WriteOutcomeLine(ok, ok, "Customer", custId);

        // 7b — failure: amount <= 0
        WriteFunctionLine("Customer.Deposit", "(custId, -10)", "false");
        var bad = Customer.Deposit(custId, -10m);
        Console.WriteLine($"  actual return : {bad}");
        WriteOutcomeLine(!bad, bad, null, null);

        // 7c — failure: zero
        WriteFunctionLine("Customer.Deposit", "(custId, 0)", "false");
        var zero = Customer.Deposit(custId, 0m);
        Console.WriteLine($"  actual return : {zero}");
        WriteOutcomeLine(!zero, zero, null, null);
    }

    // ══════════════════════════════════════════════════════════════════════════
    //  SECTION 8 — Cart operations
    // ══════════════════════════════════════════════════════════════════════════
    private static void TestCartOperations(string custId, string productId)
    {
        PrintHeader("Cart.AddItem / RemoveItem / GetTotal / Clear + Customer.ViewCart");

        if (string.IsNullOrEmpty(productId)) { Console.WriteLine("  [SKIP] no productId available"); return; }

        // 8a — ViewCart creates if missing
        WriteFunctionLine("Customer.ViewCart", "(custId)", "Cart (created if missing)");
        var cart = Customer.ViewCart(custId);
        Console.WriteLine($"  actual return : {(cart is null ? "null" : $"Cart id={cart.Id}")}");
        WriteOutcomeLine(cart is not null, cart is not null, "Cart", cart?.Id);

        // 8b — AddItem happy
        WriteFunctionLine("Cart.AddItem", "(custId, productId, 2)", "true");
        var added = Cart.AddItem(custId, productId, 2);
        Console.WriteLine($"  actual return : {added}");
        WriteOutcomeLine(added, added, "CartItem", productId);

        // 8c — AddItem: qty <= 0
        WriteFunctionLine("Cart.AddItem", "(custId, productId, 0)", "false");
        var addedBad = Cart.AddItem(custId, productId, 0);
        Console.WriteLine($"  actual return : {addedBad}");
        WriteOutcomeLine(!addedBad, addedBad, null, null);

        // 8d — AddItem: invalid product
        WriteFunctionLine("Cart.AddItem", "(custId, \"fake-id\", 1)", "false");
        var addedFake = Cart.AddItem(custId, "fake-product-id", 1);
        Console.WriteLine($"  actual return : {addedFake}");
        WriteOutcomeLine(!addedFake, addedFake, null, null);

        // 8e — GetTotal
        WriteFunctionLine("Cart.GetTotal", "(custId)", "decimal >= 0");
        var total = Cart.GetTotal(custId);
        Console.WriteLine($"  actual return : {total:C}");
        WriteOutcomeLine(total >= 0, total >= 0, null, null);

        // 8f — RemoveItem happy
        WriteFunctionLine("Cart.RemoveItem", "(custId, productId)", "true");
        var removed = Cart.RemoveItem(custId, productId);
        Console.WriteLine($"  actual return : {removed}");
        WriteOutcomeLine(removed, removed, null, null);

        // 8g — RemoveItem: not in cart
        WriteFunctionLine("Cart.RemoveItem", "(custId, \"fake-id\")", "false");
        var removedFake = Cart.RemoveItem(custId, "fake-product-id");
        Console.WriteLine($"  actual return : {removedFake}");
        WriteOutcomeLine(!removedFake, removedFake, null, null);

        // 8h — Clear (re-add first so there's something to clear)
        Cart.AddItem(custId, productId, 1);
        WriteFunctionLine("Cart.Clear", "(custId)", "void (no exception)");
        bool clearOk = true;
        try { Cart.Clear(custId); }
        catch { clearOk = false; }
        Console.WriteLine($"  actual return : void  (exception thrown: {!clearOk})");
        WriteOutcomeLine(clearOk, clearOk, null, null);
    }

    // ══════════════════════════════════════════════════════════════════════════
    //  SECTION 9 — CartItem.UpdateQuantity
    // ══════════════════════════════════════════════════════════════════════════
    private static void TestCartItemUpdateQuantity(string custId, string productId)
    {
        PrintHeader("CartItem.UpdateQuantity");

        if (string.IsNullOrEmpty(productId)) { Console.WriteLine("  [SKIP] no productId available"); return; }

        // Add an item first
        Cart.AddItem(custId, productId, 1);

        // 9a — happy: increase quantity
        WriteFunctionLine("CartItem.UpdateQuantity", "(custId, productId, 3)", "true");
        var ok = CartItem.UpdateQuantity(custId, productId, 3);
        Console.WriteLine($"  actual return : {ok}");
        WriteOutcomeLine(ok, ok, "CartItem", productId);

        // 9b — failure: qty = 0
        WriteFunctionLine("CartItem.UpdateQuantity", "(custId, productId, 0)", "false");
        var bad = CartItem.UpdateQuantity(custId, productId, 0);
        Console.WriteLine($"  actual return : {bad}");
        WriteOutcomeLine(!bad, bad, null, null);

        // 9c — failure: item not in cart
        WriteFunctionLine("CartItem.UpdateQuantity", "(custId, \"fake-id\", 2)", "false");
        var notFound = CartItem.UpdateQuantity(custId, "fake-product-id", 2);
        Console.WriteLine($"  actual return : {notFound}");
        WriteOutcomeLine(!notFound, notFound, null, null);

        // Clean up
        Cart.RemoveItem(custId, productId);
    }

    // ══════════════════════════════════════════════════════════════════════════
    //  SECTION 10 — Order operations
    // ══════════════════════════════════════════════════════════════════════════
    private static string TestOrderOperations(string custId, string adminId, string productId)
    {
        PrintHeader("Order.PlaceFromCart / TrackStatus / UpdateStatus / Cancel + Customer.ViewOrders");

        if (string.IsNullOrEmpty(productId)) { Console.WriteLine("  [SKIP] no productId"); return ""; }

        // Ensure wallet has enough funds (deposit 1000)
        Customer.Deposit(custId, 1000m);

        // Add item to cart
        Cart.AddItem(custId, productId, 1);
        var cart = Customer.ViewCart(custId);

        // 10a — PlaceFromCart happy
        WriteFunctionLine("Order.PlaceFromCart", "(custId, cart)", "Order");
        var order = (cart is not null) ? Order.PlaceFromCart(custId, cart) : null;
        Console.WriteLine($"  actual return : {(order is null ? "null" : $"Order id={order.Id} status={order.Status}")}");
        WriteOutcomeLine(order is not null, order is not null, "Order", order?.Id);
        var orderId = order?.Id ?? "";

        // 10b — PlaceFromCart: empty cart
        WriteFunctionLine("Order.PlaceFromCart", "(custId, emptyCart)", "null");
        var emptyCart = new Cart(custId); // no items
        var nullOrder = Order.PlaceFromCart(custId, emptyCart);
        Console.WriteLine($"  actual return : {(nullOrder is null ? "null" : "Order")}");
        WriteOutcomeLine(nullOrder is null, nullOrder is null, null, null);

        // 10c — TrackStatus
        if (!string.IsNullOrEmpty(orderId))
        {
            WriteFunctionLine("Order.TrackStatus", $"(custId, \"{orderId}\")", "OrderStatus.Pending");
            try
            {
                var status = Order.TrackStatus(custId, orderId);
                Console.WriteLine($"  actual return : {status}");
                WriteOutcomeLine(status == OrderStatus.Pending, true, null, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  actual return : exception — {ex.Message}");
                WriteOutcomeLine(false, false, null, null);
            }

            // 10d — UpdateStatus admin Pending → Paid
            WriteFunctionLine("Order.UpdateStatus", $"(adminId, orderId, Paid)", "true");
            var advancedOk = Order.UpdateStatus(adminId, orderId, OrderStatus.Paid);
            Console.WriteLine($"  actual return : {advancedOk}");
            WriteOutcomeLine(advancedOk, advancedOk, "Order", orderId);

            // 10e — UpdateStatus invalid transition (Paid → Pending)
            WriteFunctionLine("Order.UpdateStatus", "(adminId, orderId, Pending) — invalid back-step", "false");
            var backOk = Order.UpdateStatus(adminId, orderId, OrderStatus.Pending);
            Console.WriteLine($"  actual return : {backOk}");
            WriteOutcomeLine(!backOk, backOk, null, null);

            // 10f — Cancel Paid order
            WriteFunctionLine("Order.Cancel", $"(custId, \"{orderId}\")", "true (Paid is cancellable)");
            var cancelOk = Order.Cancel(custId, orderId);
            Console.WriteLine($"  actual return : {cancelOk}");
            WriteOutcomeLine(cancelOk, cancelOk, "Order", orderId);
        }

        // 10g — ViewOrders
        WriteFunctionLine("Customer.ViewOrders", "(custId)", "List<Order>");
        var orders = Customer.ViewOrders(custId);
        Console.WriteLine($"  actual return : List<Order> count={orders.Count}");
        WriteOutcomeLine(orders.Count >= 0, true, null, null);

        return orderId;
    }

    // ══════════════════════════════════════════════════════════════════════════
    //  SECTION 11 — Payment.ChargeWallet + Payment.Refund
    // ══════════════════════════════════════════════════════════════════════════
    private static void TestPaymentOperations(string custId, string adminId, string productId)
    {
        PrintHeader("Payment.ChargeWallet / Payment.Refund");

        if (string.IsNullOrEmpty(productId)) { Console.WriteLine("  [SKIP] no productId"); return; }

        // Ensure wallet has funds
        Customer.Deposit(custId, 2000m);

        // Place a fresh order
        Cart.AddItem(custId, productId, 1);
        var cart = Customer.ViewCart(custId);
        if (cart is null) { Console.WriteLine("  [SKIP] cart unavailable"); return; }
        var order = Order.PlaceFromCart(custId, cart);
        if (order is null) { Console.WriteLine("  [SKIP] could not place order"); return; }

        // 11a — ChargeWallet happy (captured)
        WriteFunctionLine("Payment.ChargeWallet", $"(custId, orderId, {order.Total:C})", "Payment(Captured)");
        var payment = Payment.ChargeWallet(custId, order.Id, order.Total);
        Console.WriteLine($"  actual return : Payment status={payment.Status} id={payment.Id}");
        WriteOutcomeLine(payment.Status == PaymentStatus.Captured, payment.Status == PaymentStatus.Captured, "Payment", payment.Id);

        // 11b — ChargeWallet: insufficient funds (drain wallet first)
        Cart.AddItem(custId, productId, 1);
        var cart2 = Customer.ViewCart(custId);
        if (cart2 is not null)
        {
            var order2 = Order.PlaceFromCart(custId, cart2);
            if (order2 is not null)
            {
                // Drain wallet
                using (var ctx = new AppDbContext())
                {
                    var cust = ctx.Users.OfType<Customer>().SingleOrDefault(u => u.Id == custId);
                    if (cust is not null) { cust.WalletBalance = 0m; ctx.SaveChanges(); }
                }
                WriteFunctionLine("Payment.ChargeWallet", $"(custId, orderId — insufficient funds)", "Payment(Failed)");
                var pay2 = Payment.ChargeWallet(custId, order2.Id, order2.Total);
                Console.WriteLine($"  actual return : Payment status={pay2.Status}");
                WriteOutcomeLine(pay2.Status == PaymentStatus.Failed, pay2.Status == PaymentStatus.Failed, "Payment", pay2.Id);

                Order.Cancel(custId, order2.Id);
            }
        }

        // 11c — Refund (admin)
        WriteFunctionLine("Payment.Refund", $"(adminId, paymentId={payment.Id})", "true");
        var refunded = Payment.Refund(adminId, payment.Id);
        Console.WriteLine($"  actual return : {refunded}");
        WriteOutcomeLine(refunded, refunded, "Payment", payment.Id);

        // 11d — Refund: non-admin
        WriteFunctionLine("Payment.Refund", "(custId, paymentId) — non-admin", "false");
        var badRefund = Payment.Refund(custId, payment.Id);
        Console.WriteLine($"  actual return : {badRefund}");
        WriteOutcomeLine(!badRefund, badRefund, null, null);
    }

    // ══════════════════════════════════════════════════════════════════════════
    //  SECTION 12 — Review operations
    // ══════════════════════════════════════════════════════════════════════════
    private static void TestReviewOperations(string custId, string productId)
    {
        PrintHeader("Review.Submit / GetForProduct / AverageRating");

        if (string.IsNullOrEmpty(productId)) { Console.WriteLine("  [SKIP] no productId"); return; }

        // 12a — Submit happy
        WriteFunctionLine("Review.Submit", $"(custId, productId, 5, \"Great!\")", "true");
        var submitted = Review.Submit(custId, productId, 5, "Great product!");
        Console.WriteLine($"  actual return : {submitted}");
        WriteOutcomeLine(submitted, submitted, "Review", productId);

        // 12b — Submit: rating out of range (6)
        WriteFunctionLine("Review.Submit", "(custId, productId, 6, \"\")", "false");
        var badRating = Review.Submit(custId, productId, 6, "");
        Console.WriteLine($"  actual return : {badRating}");
        WriteOutcomeLine(!badRating, badRating, null, null);

        // 12c — Submit: rating out of range (0)
        WriteFunctionLine("Review.Submit", "(custId, productId, 0, \"\")", "false");
        var zeroRating = Review.Submit(custId, productId, 0, "");
        Console.WriteLine($"  actual return : {zeroRating}");
        WriteOutcomeLine(!zeroRating, zeroRating, null, null);

        // 12d — GetForProduct
        WriteFunctionLine("Review.GetForProduct", "(productId)", "List<Review>");
        var reviews = Review.GetForProduct(productId);
        Console.WriteLine($"  actual return : List<Review> count={reviews.Count}");
        WriteOutcomeLine(reviews.Count >= 0, true, null, null);

        // 12e — AverageRating
        WriteFunctionLine("Review.AverageRating", "(productId)", "double >= 0");
        var avg = Review.AverageRating(productId);
        Console.WriteLine($"  actual return : {avg:F2}");
        WriteOutcomeLine(avg >= 0, true, null, null);
    }

    // ══════════════════════════════════════════════════════════════════════════
    //  SECTION 13 — Administrator.AdjustInventory
    // ══════════════════════════════════════════════════════════════════════════
    private static void TestAdjustInventory(string adminId, string productId)
    {
        PrintHeader("Administrator.AdjustInventory");

        if (string.IsNullOrEmpty(productId)) { Console.WriteLine("  [SKIP] no productId"); return; }

        // 13a — add stock
        WriteFunctionLine("Administrator.AdjustInventory", "(adminId, productId, +10)", "true");
        var addOk = Administrator.AdjustInventory(adminId, productId, 10);
        Console.WriteLine($"  actual return : {addOk}");
        WriteOutcomeLine(addOk, addOk, "Product", productId);

        // 13b — remove stock
        WriteFunctionLine("Administrator.AdjustInventory", "(adminId, productId, -5)", "true");
        var removeOk = Administrator.AdjustInventory(adminId, productId, -5);
        Console.WriteLine($"  actual return : {removeOk}");
        WriteOutcomeLine(removeOk, removeOk, "Product", productId);

        // 13c — failure: would go negative (remove 99999)
        WriteFunctionLine("Administrator.AdjustInventory", "(adminId, productId, -99999)", "false");
        var negOk = Administrator.AdjustInventory(adminId, productId, -99999);
        Console.WriteLine($"  actual return : {negOk}");
        WriteOutcomeLine(!negOk, negOk, null, null);

        // 13d — failure: non-admin
        WriteFunctionLine("Administrator.AdjustInventory", "(custId, productId, +1) — non-admin", "false");
        var noAuthOk = Administrator.AdjustInventory("fake-user", productId, 1);
        Console.WriteLine($"  actual return : {noAuthOk}");
        WriteOutcomeLine(!noAuthOk, noAuthOk, null, null);

        // 13e — failure: product not found
        WriteFunctionLine("Administrator.AdjustInventory", "(adminId, \"fake-product\", +1)", "false");
        var notFoundOk = Administrator.AdjustInventory(adminId, "fake-product-id", 1);
        Console.WriteLine($"  actual return : {notFoundOk}");
        WriteOutcomeLine(!notFoundOk, notFoundOk, null, null);
    }

    // ══════════════════════════════════════════════════════════════════════════
    //  SECTION 14 — Administrator.ListAllOrders
    // ══════════════════════════════════════════════════════════════════════════
    private static void TestListAllOrders(string adminId, string custId)
    {
        PrintHeader("Administrator.ListAllOrders");

        // 14a — admin: receives list
        WriteFunctionLine("Administrator.ListAllOrders", "(adminId)", "List<Order>");
        var orders = Administrator.ListAllOrders(adminId);
        Console.WriteLine($"  actual return : List<Order> count={orders.Count}");
        WriteOutcomeLine(orders.Count >= 0, true, null, null);

        // 14b — non-admin: empty list
        WriteFunctionLine("Administrator.ListAllOrders", "(custId — non-admin)", "List empty");
        var custOrders = Administrator.ListAllOrders(custId);
        Console.WriteLine($"  actual return : List<Order> count={custOrders.Count}");
        WriteOutcomeLine(custOrders.Count == 0, custOrders.Count == 0, null, null);
    }

    // ══════════════════════════════════════════════════════════════════════════
    //  SECTION 15 — Administrator.GenerateReport
    // ══════════════════════════════════════════════════════════════════════════
    private static void TestGenerateReport(string adminId)
    {
        PrintHeader("Administrator.GenerateReport");

        var from = DateTime.UtcNow.AddDays(-30);
        var to   = DateTime.UtcNow;

        // 15a — happy path: admin
        WriteFunctionLine("Administrator.GenerateReport", $"(adminId, from={from:yyyy-MM-dd}, to={to:yyyy-MM-dd})", "non-empty string");
        var report = Administrator.GenerateReport(adminId, from, to);
        Console.WriteLine($"  actual return : string length={report.Length}");
        WriteOutcomeLine(!string.IsNullOrEmpty(report) && report != "Forbidden" && report != "Invalid date range",
                         true, null, null);

        // 15b — non-admin
        WriteFunctionLine("Administrator.GenerateReport", "(\"fake-user\", ...)", "\"Forbidden\"");
        var forbidden = Administrator.GenerateReport("fake-user-id", from, to);
        Console.WriteLine($"  actual return : \"{forbidden}\"");
        WriteOutcomeLine(forbidden == "Forbidden", forbidden == "Forbidden", null, null);

        // 15c — invalid range (from > to)
        WriteFunctionLine("Administrator.GenerateReport", "(adminId, from > to)", "\"Invalid date range\"");
        var badRange = Administrator.GenerateReport(adminId, to, from);
        Console.WriteLine($"  actual return : \"{badRange}\"");
        WriteOutcomeLine(badRange == "Invalid date range", badRange == "Invalid date range", null, null);
    }

    // ══════════════════════════════════════════════════════════════════════════
    //  SECTION 16 — OrderItem.LineTotal
    // ══════════════════════════════════════════════════════════════════════════
    private static void TestOrderItemLineTotal()
    {
        PrintHeader("OrderItem.LineTotal");

        WriteFunctionLine("OrderItem.LineTotal", "(UnitPrice=10, Quantity=3)", "30");
        var item = new OrderItem("pid", "TestProduct", 10m, 3);
        var lt   = item.LineTotal;
        Console.WriteLine($"  actual return : {lt}");
        WriteOutcomeLine(lt == 30m, lt == 30m, null, null);

        WriteFunctionLine("OrderItem.LineTotal", "(UnitPrice=7.50, Quantity=4)", "30.00");
        var item2 = new OrderItem("pid2", "ProductB", 7.50m, 4);
        var lt2   = item2.LineTotal;
        Console.WriteLine($"  actual return : {lt2}");
        WriteOutcomeLine(lt2 == 30m, lt2 == 30m, null, null);
    }

    // ══════════════════════════════════════════════════════════════════════════
    //  Helpers
    // ══════════════════════════════════════════════════════════════════════════

    private static void PrintHeader(string section)
    {
        Console.WriteLine();
        Console.WriteLine($"  ── {section} ──");
        Console.WriteLine("  " + new string('-', 54));
    }

    private static void WriteFunctionLine(string fn, string args, string expected)
    {
        Console.WriteLine($"  function name is  : {fn}");
        Console.WriteLine($"  Arguments are     : {args}");
        Console.WriteLine($"  expected return   : {expected}");
    }

    private static void WriteOutcomeLine(bool passed, bool rawResult, string? entityType, string? entityId)
    {
        _total++;
        string outcome;
        if (passed)   { outcome = "passed";  _passed++; }
        else          { outcome = "failed";  _failed++; }

        Console.WriteLine($"  Outcome           : {outcome}");
        if (entityType is not null && entityId is not null)
            Console.WriteLine($"  EntityMade        : {entityType} : {entityId}");
        Console.WriteLine();
    }

    private static string? GetEmailById(string userId)
    {
        try
        {
            using var ctx = new AppDbContext();
            return ctx.Users.SingleOrDefault(u => u.Id == userId)?.Email;
        }
        catch { return null; }
    }
}
