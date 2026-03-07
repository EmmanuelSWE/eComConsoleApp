# Cmd District — Implementation (Local-Only, No Services)

> **Scope**  
> Single-file implementation guide for the **Cmd District** .NET console app with **no external DB and no Services layer**.  
> All logic stays **local/in-memory** inside Entities, Menus, and simple **InMemory Stores**.  
> Copy this file as-is into `docs/implementation.md`.

---

## 1) Tech Stack

- **.NET 8** Console Application
- **C# 12**
- **Console UI** (synchronous I/O)
- **Pure In-memory data** (static dictionaries/lists)
- **No Services layer**, **No database**

---

## 2) Folder Layout (Proper, Local-Only)

```
.
├─ docs/
│  ├─ spec.md
│  ├─ plan.md
│  └─ implementation.md                 # ← this file
│
└─ src/
   └─ CmdDistrict/
      ├─ CmdDistrict.csproj
      ├─ Program.cs                     # Entrypoint: bootstraps the menu system
      │
      ├─ Common/
      │  ├─ Guard.cs                    # Argument validation helpers
      │  ├─ Result.cs                   # Lightweight Result<T> pattern (optional use)
      │  └─ DateProvider.cs             # Centralized DateTime access
      │
      ├─ Models/
      │  ├─ Entities/
      │  │  ├─ Enums.cs                 # OrderStatus, PaymentStatus
      │  │  ├─ User.cs                  # abstract User (Id, Name, Email, Password, Role)
      │  │  ├─ Customer.cs              # Customer : User (Wallet, Defaults)
      │  │  ├─ Administrator.cs         # Administrator : User
      │  │  ├─ Product.cs
      │  │  ├─ CartItem.cs
      │  │  ├─ Cart.cs
      │  │  ├─ OrderItem.cs
      │  │  ├─ Order.cs
      │  │  ├─ Payment.cs
      │  │  └─ Review.cs
      │  │
      │  └─ Menu/
      │     ├─ Menu.cs                  # abstract base (Title/Key, Show/PrintOptions/Handle)
      │     ├─ MainMenu.cs              # Register/Login/Browse/Exit
      │     ├─ CustomerMenu.cs          # Customer actions (needs userId)
      │     ├─ AdminMenu.cs             # Admin actions (needs userId)
      │     └─ GlobalMenuHolder.cs      # Registry + Router (tracks CurrentUserId/Role)
      │
      └─ Infrastructure/
         └─ InMemory/                   # ← Data lives here for v1 (no DB)
            ├─ UserStore.cs
            ├─ ProductStore.cs
            ├─ CartStore.cs
            ├─ OrderStore.cs
            └─ ReviewStore.cs
```

> **Removed**: `Infrastructure/Services/*` entirely.

---

## 3) Key Decisions (Local-Only)

- **No Services layer**: Menus call **Entities** directly, and entities/read-write go through **InMemory Stores**.
- **Role-aware routing** remains: after login/register, keep `CurrentUserId` + role and route to Customer/Admin menu.
- **Validation** centralized via `Guard`. Menus handle user input errors (re-prompt); entities enforce business rules.
- **Result pattern** optional. For simplicity, entity methods can return `bool` or throw for programming errors.
- **Persistence**: in-memory collections live for the process lifetime. No files, no DB.

---

## 4) Build & Run

```bash
cd src/CmdDistrict
dotnet run
```

Runtime order:
1. `Program.cs` → `GlobalMenuHolder.Start()`
2. `GlobalMenuHolder.Run()` loops
3. `MainMenu.Show()` for guest actions
4. On success login/sign: `GlobalMenuHolder.SwitchToRole(userId)` → **Customer**/**Admin** menus.

---

## 5) Coding Standards (Unchanged)

### 5.1 Naming & Structure
- Namespaces per layout above.
- `PascalCase` for types/members; `camelCase` for locals/params.
- One public type per file; file path mirrors namespace.

### 5.2 Error Handling & Validation
- `Guard` for preconditions (null/empty/range).
- Menus show friendly messages for invalid user input; entities enforce rules and may return `false` or throw `InvalidOperationException` for illegal transitions.

### 5.3 State & Immutability
- Use `init` for `Id`. Prefer controlled mutation through entity methods.

### 5.4 Security (v1 local)
- Passwords stored in plain text (for demo only). No tokens/sessions.


### classes and functions 
- private fields with prefix of _ 
---

## 6) InMemory Stores (No DB)

> Each store exposes minimal CRUD helpers. Use **static** methods.

- **UserStore**: users by `Email` + by `Id`.
- **ProductStore**: list of products; find by name.
- **CartStore**: carts by `CustomerId`.
- **OrderStore**: orders list; by customer.
- **ReviewStore**: reviews list; by product id.

**Rules:**
- Only entity methods touch stores (Menus call entities, not stores directly) to keep flow clean.
- all private fields of a class must have getters and setters.

---

## 7) Model Actions — Implementation Descriptions (Local, LINQ-Only, Numbered)

> All actions operate on **in‑memory lists** in `AppState` using **LINQ** (no services, no stores).  
> Use `DateProvider.UtcNow` for timestamps where needed.

### 7.1 `User` (abstract, `cmdDistrict.Models.Entities`)

**Attributes**: `id : string`, `Name`, `Email`, `Password`, `Role`
**Construcctor**: takes `Name`, `Email`, `Password`, `Role` all strings

#### 7.1.1 Login(email, password) ⇒ (bool success, string userId, string role)
1. user inputs login
2. Prompted for email : inputs
3. Prompted for password : inputs 

4. validation check if email  and password input ( must not be empty)
5. Linq query : use Helper find UserBy Email using email. 
6. return find UserBy.


**Failure cases**
5a. inputs failed check :
    1. send message ask user to reprompt. 
6a. Helper returns false : 
    1. notify user of email or password may be invalid.
    2. reprompt.
---

#### 7.1.2 Signin(name, email, password, role) ⇒ (bool success, string userId)
1. user inputs signin
2. Prompted for name : inputs
3. Prompted for email : inputs
4. Prompted for password : inputs 

5. validation check if email name and password input (add must not be empty)
6. Linq query : use Helper find UserBy Email using email. 
7. create user instance (email, name, password (hashed)).
8. save user instance to table.
9. return true.


**Failure cases**
5a. inputs failed check :
    1. send message ask user to reprompt. 
6a. Helper returns true : 
    1. notify user of email being part of system already.
    2. reprompt.


---

#### 7.1.3 Logout() ⇒ bool
1. set CurrentUser id in GlobalMenu instance to null
2. return true.

**Failure cases**
1) None (local no‑op).

---

#### 7.1.4 GetRole(userId) ⇒ string
1. return role use getters and setters

**Failure cases**
2. none.

---

### 7.2 `Customer : User`

**Attributes**: `WalletBalance`, `DefaultShippingAddress`

#### 7.2.1 Deposit(userId, amount) ⇒ bool
1) **Validate**: `amount > 0`.  
2) **Find (LINQ)**:  
   `var c = AppState.Users.SingleOrDefault(u => u.Id == userId && u.Role == "Customer");`  
3) **Update**: `c.WalletBalance += amount`.  
4) **Return** `true`.

**Failure cases**
1) `amount <= 0` → `false`.  
2) Not found / wrong role → `false`.

---

#### 7.2.2 ViewOrders(userId) ⇒ List<Order>
1) **Query (LINQ)**:  
   `AppState.Orders.Where(o => o.CustomerId == userId).OrderByDescending(o => o.CreatedAt).ToList();`  
2) **Return** list.

**Failure cases**
1) None (empty list ok).

---

#### 7.2.3 ViewCart(userId) ⇒ Cart
1) **Find (LINQ)**:  
   `var cart = AppState.Carts.SingleOrDefault(c => c.CustomerId == userId);`  
2) **Create if missing**: new `Cart { CustomerId = userId }` and `AppState.Carts.Add(cart)`.  
3) **Return** cart.

**Failure cases**
1) None.

---

### 7.3 `Administrator : User`

#### 7.3.1 AdjustInventory(userId, productId, delta) ⇒ bool
1) **Auth**:  
   `AppState.Users.Any(u => u.Id == userId && u.Role == "Administrator")`.  
2) **Find product (LINQ)**:  
   `var p = AppState.Products.SingleOrDefault(x => x.Id == productId);`  
3) **Compute**: `var newStock = p.Stock + delta;`  
4) **Validate**: `newStock >= 0`.  
5) **Apply**: `p.Stock = newStock;`  
6) **Return** `true`.

**Failure cases**
1) Non‑admin → `false`.  
2) Product not found → `false`.  
3) Stock would be negative → `false`.

---

#### 7.3.2 ListAllOrders(userId) ⇒ List<Order>
1) **Auth admin** via LINQ (as above).  
2) **Return** `AppState.Orders.OrderByDescending(o => o.CreatedAt).ToList()`.

**Failure cases**
1) Non‑admin → return `new List<Order>()` (or handle at menu level).

---

#### 7.3.3 GenerateReport(userId, from, to) ⇒ string
1) **Auth admin**.  
2) **Filter (LINQ)**: `var range = AppState.Orders.Where(o => o.CreatedAt >= from && o.CreatedAt <= to);`  
3) **By status**: `var byStatus = range.GroupBy(o => o.Status).Select(g => (g.Key, Count: g.Count(), Total: g.Sum(o => o.Total)));`  
4) **Revenue**: `var revenue = range.Sum(o => o.Total);`  
5) **Top products**:  
   `var top = range.SelectMany(o => o.Items).GroupBy(i => i.ProductId).Select(g => (ProductId: g.Key, Qty: g.Sum(i => i.Quantity))).OrderByDescending(x => x.Qty).Take(5);`  
6) **Format** as lines; **return** string.

**Failure cases**
1) Non‑admin → `"Forbidden"`.  
2) `from > to` → `"Invalid date range"`.

---

### 7.4 `Product`

#### 7.4.1 Create(userId, name, description, price, stock) ⇒ Product
1) **Auth admin**.  
2) **Validate**: `!string.IsNullOrWhiteSpace(name)`, `price >= 0`, `stock >= 0`.  
3) **Create**: new `Product { Id = Guid.NewGuid().ToString(), ... }`.  
4) **Add (LINQ list op)**: `AppState.Products.Add(product)`.  
5) **Return** product.

**Failure cases**
1) Non‑admin → `null`.  
2) Invalid fields → `null`.

---

#### 7.4.2 Update(userId, id, name, description, price, stock) ⇒ bool
1) **Auth admin**.  
2) **Find (LINQ)**: `var p = AppState.Products.SingleOrDefault(x => x.Id == id);`  
3) **Validate** fields.  
4) **Apply** changes.  
5) **Return** `true`.

**Failure cases**
1) Non‑admin → `false`.  
2) Product not found → `false`.  
3) Invalid fields → `false`.

---

#### 7.4.3 Delete(userId, id) ⇒ bool
1) **Auth admin**.  
2) **Find** product.  
3) **Remove**: `AppState.Products.Remove(p)`.  
4) **Return** `true`.

**Failure cases**
1) Non‑admin → `false`.  
2) Not found → `false`.

---

#### 7.4.4 FindById(id) ⇒ Product?
1) **LINQ**: `AppState.Products.SingleOrDefault(p => p.Id == id)`.  
2) **Return** product or `null`.

**Failure cases**
1) None.

---

#### 7.4.5 SearchByName(query) ⇒ List<Product>
1) **Normalize**: `q = query?.Trim();`  
2) **If empty**: return **all** products ordered by name.  
3) **Else**:  
   `AppState.Products.Where(p => p.Name.Contains(q, StringComparison.OrdinalIgnoreCase)).OrderBy(p => p.Name).ToList();`

**Failure cases**
1) None.

---

### 7.5 `Cart`

#### 7.5.1 AddItem(userId, productId, quantity) ⇒ bool
1) **Validate**: `quantity > 0`.  
2) **Cart (LINQ)**: `var cart = AppState.Carts.SingleOrDefault(c => c.CustomerId == userId) ?? create+add`.  
3) **Product (LINQ)**: `var p = AppState.Products.SingleOrDefault(x => x.Id == productId)`.  
4) **Existing line**: `var line = cart.Items.SingleOrDefault(i => i.ProductId == productId);`  
5) **Desired qty**: `var newQty = (line?.Quantity ?? 0) + quantity;`  
6) **Stock check**: `p.Stock >= newQty` (stock decremented on **order**, not here).  
7) **Upsert**: create/update `CartItem` with `UnitPrice = p.Price`.  
8) **Return** `true`.

**Failure cases**
1) `quantity <= 0` → `false`.  
2) Product not found → `false`.  
3) `newQty` exceeds current stock → `false`.

---

#### 7.5.2 RemoveItem(userId, productId) ⇒ bool
1) **Cart**: find by `userId`.  
2) **Line**: `SingleOrDefault` by `productId`.  
3) **If found**: `cart.Items.Remove(line)`; **return** `true`.  
4) **Else**: `false`.

**Failure cases**
1) Missing cart/line → `false`.

---

#### 7.5.3 Clear(userId) ⇒ void
1) **Cart**: get by `userId`; if missing create then clear.  
2) **Clear**: `cart.Items.Clear()`.

**Failure cases**
1) None.

---

#### 7.5.4 GetTotal(userId) ⇒ decimal
1) **Cart**: get by `userId`.  
2) **Sum (LINQ)**: `cart.Items.Sum(i => i.UnitPrice * i.Quantity)`.  
3) **Return** total (0 if null/empty).

**Failure cases**
1) None.

---

### 7.6 `CartItem`

#### 7.6.1 UpdateQuantity(userId, newQuantity) ⇒ bool
1) **Validate**: `newQuantity > 0`.  
2) **Cart**: get by `userId`.  
3) **Line**: locate item.  
4) **Product**: from `AppState.Products` by `line.ProductId`.  
5) **Stock check**: `product.Stock >= newQuantity`.  
6) **Apply**: `line.Quantity = newQuantity`.  
7) **Return** `true`.

**Failure cases**
1) Line not found → `false`.  
2) `newQuantity <= 0` → `false`.  
3) Stock insufficient → `false`.

---

### 7.7 `Order`

#### 7.7.1 PlaceFromCart(userId, cart) ⇒ Order
1) **Ownership**: `cart.CustomerId == userId`.  
2) **Non‑empty**: `cart.Items.Any()`.  
3) **Re‑validate stock (LINQ)** for each line against `AppState.Products`.  
4) **Snapshot** `OrderItem`s (copy `ProductName`, `UnitPrice`, `Quantity`).  
5) **Decrement stock** on each corresponding product.  
6) **Compute total**: `items.Sum(i => i.UnitPrice * i.Quantity)`.  
7) **Create** order `{ Status = Pending, CreatedAt = DateProvider.UtcNow }`.  
8) **Add** to `AppState.Orders`.  
9) **Return** order.

**Failure cases**
1) Cart does not belong to user → throw `InvalidOperationException`.  
2) Cart empty → return `null`.  
3) Stock now insufficient → **fail** and do not create order.

---

#### 7.7.2 Cancel(userId, orderId) ⇒ bool
1) **Find order**: `var o = AppState.Orders.SingleOrDefault(x => x.Id == orderId);`  
2) **Ownership**: user must be owner (or admin via role check).  
3) **Allowed**: `o.Status ∈ { Pending, Paid }` and **not Shipped**.  
4) **Set** `o.Status = Cancelled`.  
5) **Restock** products if not shipped.  
6) **Return** `true`.

**Failure cases**
1) Not owner (and not admin) → `false`.  
2) Status not cancellable → `false`.  
3) Order not found → `false`.

---

#### 7.7.3 TrackStatus(userId, orderId) ⇒ OrderStatus
1) **Find** order via LINQ.  
2) **Ownership** check for customers.  
3) **Return** `o.Status`.

**Failure cases**
1) Not owner (customer) → throw `InvalidOperationException`.  
2) Order not found → throw `InvalidOperationException`.

---

#### 7.7.4 UpdateStatus(userId, orderId, status) ⇒ bool
1) **Auth admin**.  
2) **Find** order.  
3) **Validate transition** against graph:  
   `Pending → Paid → Packed → Shipped → Delivered` and `Pending/Paid → Cancelled`.  
4) **Apply**; **return** `true`.

**Failure cases**
1) Not admin → `false`.  
2) Invalid transition → `false`.  
3) Order not found → `false`.

---

### 7.8 `OrderItem`

#### 7.8.1 LineTotal() ⇒ decimal
1) **Compute** `UnitPrice * Quantity`.  
2) **Return** value.

**Failure cases**
1) None.

---

### 7.9 `Payment`

#### 7.9.1 ChargeWallet(userId, orderId, amount) ⇒ Payment
1) **Find order** by id; ensure `order.CustomerId == userId`.  
2) **Find customer** by id.  
3) **Validate amount** `> 0`.  
4) **Funds check**: if `customer.WalletBalance >= amount` →  
   4.1) Deduct wallet (`customer.WalletBalance -= amount`).  
   4.2) Create `Payment { Status = Captured }`; add to `order.Payments` (or global list if you keep one).  
   4.3) Set `order.Status = Paid`.  
   4.4) **Return** payment.  
5) **Insufficient funds** →  
   5.1) Create `Payment { Status = Failed }`;  
   5.2) **Return** payment and **surface message** “Insufficient funds — add funds now?”.

**Failure cases**
1) Order not owned by user → `Payment(Status = Failed)` with error.  
2) Amount ≤ 0 → `Payment(Status = Failed)`.  
3) Missing order/user → `Payment(Status = Failed)`.

> **Menu rule**: If `Failed` due to funds → prompt **Yes** (go to Wallet Deposit then retry) or **No** (back to Customer Menu).

---

#### 7.9.2 Refund(userId, paymentId) ⇒ bool
1) **Auth admin**.  
2) **Find payment** (LINQ) from orders’ payments.  
3) **Must be Captured**.  
4) **Credit wallet** by `payment.Amount`.  
5) **Mark** `payment.Status = Refunded`.  
6) **Optionally** mark order `Refunded`.  
7) **Return** `true`.

**Failure cases**
1) Not admin → `false`.  
2) Payment not found or not captured → `false`.

---

### 7.10 `Review`

#### 7.10.1 Submit(userId, productId, rating, comment) ⇒ bool
1) **Validate** `rating ∈ [1..5]`.  
2) **(v2)** Ensure user purchased product (LINQ over orders).  
3) **Create** review with timestamps.  
4) **Add** to `AppState.Reviews`.  
5) **Return** `true`.

**Failure cases**
1) Rating out of range → `false`.  
2) (v2) Not purchased → `false`.

---

#### 7.10.2 GetForProduct(productId) ⇒ List<Review>
1) **LINQ**:  
   `AppState.Reviews.Where(r => r.ProductId == productId).OrderByDescending(r => r.CreatedAt).ToList();`  
2) **Return** list.

**Failure cases**
1) None.

---

#### 7.10.3 AverageRating(productId) ⇒ double
1) **Project (LINQ)**: `var ratings = AppState.Reviews.Where(r => r.ProductId == productId).Select(r => r.Rating);`  
2) **If none**: return `0`.  
3) **Else**: `ratings.Average()`.

**Failure cases**
1) None.

---

## 8) Menus — Behavior & Routing (Local, LINQ-Only, Numbered)

### 8.1 `Menu` (abstract, `cmdDistrict.Models`)
1) **Set context**: `ContextUserId = userId`.  
2) **Render** title.  
3) **PrintOptions()**.  
4) **Read** input.  
5) **Dispatch**: `HandleSelection(input)`.  
6) **If invalid**: show “Invalid selection” and remain.

**Failure cases**
1) Invalid input → re‑prompt message, stay in same menu.

---

### 8.2 `MainMenu` (guest)
1) **Register**  
   1.1) Prompt name/email/password/role.  
   1.2) `User.Sign` (LINQ uniqueness check on `AppState.Users`).  
   1.3) **Success** → `GlobalMenuHolder.SwitchToRole(userId)`.  
   1.4) **Fail** → show reason; remain in Main.
2) **Login**  
   2.1) Prompt email/password.  
   2.2) `User.Login` (LINQ lookup).  
   2.3) **Success** → `SwitchToRole(userId)`.  
   2.4) **Fail** → “Invalid credentials”; remain in Main.
3) **Browse (Guest)**  
   3.1) Optional search query.  
   3.2) `Product.SearchByName` (LINQ contains).  
   3.3) If none → “No products available”.
4) **Exit** → terminate process.

**Failure cases**
1) Bad inputs → stay in Main.  
2) Browse no results → friendly notice.

---

### 8.3 `CustomerMenu` (requires `userId`)
1) **Browse Products**  
   1.1) Search/list with LINQ.  
   1.2) Show name, price, stock.
2) **Add to Cart**  
   2.1) Prompt product id & qty.  
   2.2) `Cart.AddItem` using LINQ checks.  
   2.3) On failure (not found/stock/qty) → show error.
3) **View Cart**  
   3.1) `Cart.GetTotal(userId)` (LINQ sum).  
   3.2) Render lines & totals.
4) **Checkout**  
   4.1) `Order.PlaceFromCart` (re‑validates stock via LINQ).  
   4.2) `Payment.ChargeWallet`.  
   4.3) If **Captured** → clear cart; success message.  
   4.4) If **Failed due to funds** → prompt:
        - **Yes** → go **Deposit Wallet**, then retry.  
        - **No** → return to menu with message.
5) **View Orders**  
   5.1) LINQ filter by user; show latest first.  
6) **Add Review**  
   6.1) Prompt productId, rating, comment.  
   6.2) `Review.Submit` (LINQ validation).  
7) **Deposit Wallet**  
   7.1) Prompt amount.  
   7.2) `Customer.Deposit`; validate > 0.  
8) **Logout**  
   8.1) `User.Logout()`; clear `CurrentUserId`; `Switch("main")`.

**Failure cases**
1) Invalid product/qty → add-to-cart fails.  
2) Checkout insufficient funds → offer deposit flow.  
3) Invalid rating → error.  
4) Deposit amount <= 0 → error.

---

### 8.4 `AdminMenu` (requires `userId`)
1) **Add Product**  
   1.1) Prompt name/desc/price/stock.  
   1.2) `Product.Create` (LINQ add).  
2) **Update Product**  
   2.1) Prompt id & new fields.  
   2.2) `Product.Update` (LINQ find+apply).  
3) **Delete Product**  
   3.1) Prompt id.  
   3.2) `Product.Delete` (LINQ remove).  
4) **Adjust Inventory**  
   4.1) Prompt id & delta.  
   4.2) `Administrator.AdjustInventory` (LINQ validate & apply).  
5) **View All Orders**  
   5.1) `Administrator.ListAllOrders` (LINQ order desc).  
6) **Update Order Status**  
   6.1) Prompt order id & target status.  
   6.2) `Order.UpdateStatus` (LINQ find + transition rules).  
7) **Generate Report**  
   7.1) Prompt date range.  
   7.2) `Administrator.GenerateReport` (LINQ group/sum).  
8) **Logout**  
   8.1) `User.Logout()`; clear `CurrentUserId`; `Switch("main")`.

**Failure cases**
1) Non‑admin access (should be blocked by router).  
2) Invalid fields / negative stock / invalid transition → show error.

---

### 8.5 `GlobalMenuHolder` (router)
1) **Bootstrap**: register `MainMenu`, `CustomerMenu`, `AdminMenu`; set current = Main.  
2) **SwitchToRole(userId)**:  
   2.1) `CurrentUserId = userId`.  
   2.2) `CurrentRole = User.GetRole(userId)` (LINQ lookup).  
   2.3) If `"Administrator"` → `Switch("admin")`, else `Switch("customer")`.  
3) **Run**: loop `current.Show(CurrentUserId)`.

**Failure cases**
1) Role lookup fails → show error; switch to Main.  
2) Missing menu key (dev mistake) → diagnostic; switch to Main.
## 9) Build & Project Files (Reference)

**`root/Program.cs`**
```csharp
using cmdDistrict.Models;

GlobalMenuHolder.Bootstrap();
GlobalMenuHolder.Run();
```

---

## 10) Manual Test Cases (Local)

**Navigation**
- Invalid input → re-prompt; main exit ends process.

**Auth**
- Register Customer/Admin and auto-route.
- Login wrong password → stays on main.

**Customer**
- Empty product list → message.
- Valid add to cart → visible in cart; total correct.
- Checkout sufficient/insufficient wallet paths.
- Review rating bounds enforced.

**Admin**
- Add/Update/Delete product.
- Prevent negative stock on adjust.
- Enforce order status transitions.
- Generate report over date range.

---

## 11) Future Work

- Swap InMemory with file-backed JSON for persistence (still local).
- Later (v2): introduce Services + Repositories when moving to DB.
- Add unit tests for entities and stores.

---

## 12) Links

- `../spec.md`
- `../plan.md`
- `./implementation.md` (this file)

---
