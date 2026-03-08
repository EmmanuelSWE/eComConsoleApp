# Cmd District — Implementation (SQL Server via ADO.NET, No Services)

> **Scope**  
> Single-file implementation guide for the **Cmd District** .NET console app with **SQL Server (Docker) via ADO.NET, no Services/ORMs**.  
> **Entities call Stores.Sql; Menus call Entities; no Services/ORMs.**  
> Copy this file as-is into `docs/implementation.md`.

---

## 1) Tech Stack

- **.NET 8** Console Application
- **C# 12**
- **Console UI** (synchronous I/O)
- **SQL Server (Docker)**
- **ADO.NET** (`Microsoft.Data.SqlClient`)
- **No Services/ORM layer**

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
      │  ├─ DateProvider.cs             # Centralized DateTime access
      │  └─ Session.cs                  # Login session state (CurrentUserId, CurrentRole)
      │
      ├─ DataAccess/
      │  └─ Db.cs                       # SqlConnection factory & helpers
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
         └─ Stores.Sql/                 # ← SQL Server stores (ADO.NET)
            ├─ UserStore.cs
            ├─ ProductStore.cs
            ├─ CartStore.cs
            ├─ OrderStore.cs
            └─ ReviewStore.cs
```

> **Removed**: `Infrastructure/Services/*` entirely. `Infrastructure/InMemory/` replaced by `Infrastructure/Stores.Sql/`.

---

## 3) Key Decisions (Local-Only)

- **No Services/ORM layer**: Menus call **Entities** directly, and entities/read-write go through **Stores.Sql**.
- **Role-aware routing** remains: after login/register, `Session.CurrentUserId` + `Session.CurrentRole` are set internally by the Store and route to Customer/Admin menu.
- **Validation** centralized via `Guard`. Menus handle user input errors (re-prompt); entities enforce business rules.
- **Result pattern** optional. For simplicity, entity methods can return `bool` or throw for programming errors.
- **Persistence**: data persists in SQL Server (Docker). Connection via `Db.cs` (ADO.NET, `Microsoft.Data.SqlClient`).

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

## 6) SQL Stores via ADO.NET

> Each store exposes minimal CRUD helpers via **ADO.NET** (`Microsoft.Data.SqlClient`). Use **static** methods backed by `Db.cs`.

- **UserStore**: users by `Email` + by `Id`; `Login` and `Sign` set `Session.CurrentUserId` + `Session.CurrentRole` internally and return `bool`.
- **ProductStore**: products table; find by name.
- **CartStore**: carts by `CustomerId`.
- **OrderStore**: orders table; by customer.
- **ReviewStore**: reviews table; by product id.

**Rules:**
- Only entity methods touch stores (Menus call entities, not stores directly) to keep flow clean.
- all private fields of a class must have getters and setters.
- all SQL queries are surrounded by a try catch block.
- when mapping entity items in a list rather have it display the index + 1 of it first then product content . when searching first filter by looking for userInput -1 as index in list : 
**validate** -> `if searchIndex < list.length`
**found** -> use entity Id for linq query
**error** ->  handle, dont let crash app. 

---

## 7) Model Actions — Implementation Descriptions (Local, LINQ-Only, Numbered)

> All actions operate on **SQL Server tables** via **Stores.Sql** (no services, no ORMs).  
> Use `DateProvider.UtcNow` for timestamps where needed.

### 7.1 `User` (abstract, `cmdDistrict.Models.Entities`)

**Attributes**: `id : string`, `Name`, `Email`, `Password`, `Role`
**Construcctor**: takes `Name`, `Email`, `Password`, `Role` all strings

#### 7.1.1 Login(email, password) ⇒ bool
1. user inputs login
2. Prompted for email : inputs
3. Prompted for password : inputs 

4. validation check if email  and password input ( must not be empty)
5. Store query : use UserStore to find user by email. 
6. Store returns `bool`; on success, sets `Session.CurrentUserId` and `Session.CurrentRole` internally.


**Failure cases**
5a. inputs failed check :
    1. send message ask user to reprompt. 
6a. Helper returns false : 
    1. notify user of email or password may be invalid.
    2. reprompt.
---

#### 7.1.2 Signin(name, email, password, role) ⇒ bool
1. user inputs signin
2. Prompted for name : inputs
3. Prompted for email : inputs
4. Prompted for password : inputs 

5. validation check if email name and password input (add must not be empty)
6. Store query : use UserStore to check email uniqueness. 
7. create user instance (email, name, password (hashed)) and wallet instance for user.
8. save user instance and wallet via Store (SQL INSERT).
9. Store returns `bool`; on success, sets `Session.CurrentUserId` and `Session.CurrentRole` internally.


**Failure cases**
5a. inputs failed check :
    1. send message ask user to reprompt. 
6a. Helper returns true : 
    1. notify user of email being part of system already.
    2. reprompt.


---

#### 7.1.3 Logout() ⇒ bool
1. Store clears `Session.CurrentUserId` and `Session.CurrentRole` (set both to `null`).
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
1. validate amount > 0 :
2. get users wallet using userID via Store query.
3. Increment Wallet balance.
4. 

**Failure cases**
1) `amount <= 0` → `false`.  
2) Not found / wrong role → `false`.
3. Wallet not found -> throw exception.

---

#### 7.2.2 ViewOrders(userId) ⇒ List<Order>
1. Store query find orders of user : parameter userId used in search.

**Failure cases**
1) None (empty list ok).

---

#### 7.2.3 ViewCart(userId) ⇒ Cart
1) **Find (Store)**:  
   `var cart = CartStore.GetByCustomerId(userId);`  
2) **Create if missing**: new `Cart { CustomerId = userId }` 
3) **Return** cart.

**Failure cases**
1) None.

---

### 7.3 `Administrator : User`

#### 7.3.1 AdjustInventory(userId, productId) ⇒ bool
1) **Auth**:  
   `UserStore.IsAdmin(userId)`.  
2) **Find product (Store)**:  
   get product using search by name or product id via ProductStore.
3) **Compute**: `var newStock = p.Stock ;`  
4) **Validate**: `newStock >= 0`.  
5) **Apply**: `p.Stock = newStock;`  
6) **Return** `true`.

**Failure cases**
1) Non‑admin → `false`.  
2) Product not found → `false`.  
3) Stock would be negative → `false`.

---

#### 7.3.2 ListAllOrders(userId) ⇒ List<Order>
1) **Auth admin** via Store (as above).  
2) **Return** `List of orders

**Failure cases**
1) Non‑admin → return `new List<Order>()` (or handle at menu level).

---

#### 7.3.3 GenerateReport(userId, from) ⇒ string
1) **Auth admin**.  
2. Fetch all usrs, products and orders put them in serparate lists. 
3. store all have variables to store avg and toatl for each list.
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
4) **Add (Store INSERT)**: save product to products table via ProductStore.
5) **Return** product.

**Failure cases**
1) Non‑admin → `null`.  
2) Invalid fields → `null`.

---

#### 7.4.2 Update(userId, id, name, description, price, stock) ⇒ bool
1) **Auth admin**.  
2) **Find (Store)**: find from products table via ProductStore. 
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
3) **Remove (Store DELETE)**: remove using id as WHERE parameter from products table. 
4) **Return** `true`.

**Failure cases**
1) Non‑admin → `false`.  
2) Not found → `false`.

---

#### 7.4.4 FindById(id) ⇒ Product?
1) **Store query**: find by id as param via ProductStore.
2) **Return** product or `null`.

**Failure cases**
1) None.

---

#### 7.4.5 SearchByName(query) ⇒ List<Product>
1) **Normalize**: `q = query?.Trim();`  
2) **If empty**: return **all** products ordered by name via Store.  
3) **Else**:  
    return products where name like `'%{phrase}%'` via Store SQL query.

**Failure cases**
1) None.

---

### 7.5 `Cart`

#### 7.5.1 AddItem(userId, productId, quantity) ⇒ bool
1) **Validate**: `quantity > 0`.  
2) **Cart (Store)**: get cart of user from carts table via CartStore, param: userId.
3) **Product (Store)**: get product from products table via ProductStore.
5) **Desired qty**: if p.stock >= quantity 
6) **Stock check**: decrement stock of product record. 
7. add stock item to users cart.  
8. save all reconrds to theire tables
9) **Return** `true`.

**Failure cases**
1) `quantity <= 0` → `false`.  
2) Product not found → `false`.  
3) `newQty` exceeds current stock → `false`.

---

#### 7.5.2 RemoveItem(userId, productId) ⇒ bool
1) **Cart** Store: find by `userId` via CartStore.  
2) **Line**: find by `productId` and cartId in cartItems table via Store.  
3) **If found**: remove from cartItems table via Store DELETE; **return** `true`.  
4) **Else**: `false`.

**Failure cases**
1) Missing cart/line → `false`.

---

#### 7.5.3 Clear(userId) ⇒ void
1) **Cart**: get by `userId`; if missing create then clear.
2. Store query : find all cartItems by cartId.  
3. foreach cartItem found: delete via Store DELETE.
4. commit
5. remove from cart via Store DELETE.
5. print dynamic message of cartItems being deleted


**Failure cases**
1) None.

---

#### 7.5.4 GetTotal(userId) ⇒ decimal
1) **Cart**: get by `userId`.  
2) **Sum (Store)**: `CartStore.GetTotal(userId)` (SQL `SUM(UnitPrice * Quantity)`).  
3) **Return** total (0 if null/empty).

**Failure cases**
1) None.

---

### 7.6 `CartItem`

#### 7.6.1 UpdateQuantity(userId, newQuantity) ⇒ bool
1) **Validate**: `newQuantity > 0`.  
2) **Cart**: get by `userId`.  
3) **Line**: locate item.  
4) **Product**: from `ProductStore` by `line.ProductId`.  
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
3) **Re‑validate stock (Store)** for each line against `ProductStore`.  
4) **Snapshot** `OrderItem`s (copy `ProductName`, `UnitPrice`, `Quantity`).  
5) **Decrement stock** on each corresponding product.  
6) **Compute total**: `items.Sum(i => i.UnitPrice * i.Quantity)`.  
7) **Create** order `{ Status = Pending, CreatedAt = DateProvider.UtcNow }`.  
8) **Add** to orders table via Store INSERT.  
9) **Return** order.

**Failure cases**
1) Cart does not belong to user → throw `InvalidOperationException`.  
2) Cart empty → return `null`.  
3) Stock now insufficient → **fail** and do not create order.

---

#### 7.7.2 Cancel(userId, orderId) ⇒ bool
1) **Find order**: `var o = OrderStore.GetById(orderId);`  
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
1) **Find** order via Store.  
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
2) **Find payment** (Store query) from payments table.  
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
4) **Add** to reviews table via Store INSERT.  
5) **Return** `true`.

**Failure cases**
1) Rating out of range → `false`.  
2) (v2) Not purchased → `false`.

---

#### 7.10.2 GetForProduct(productId) ⇒ List<Review>
1) **Store query**:  
   `ReviewStore.GetByProductId(productId)` ordered by `CreatedAt` desc.  
2) **Return** list.

**Failure cases**
1) None.

---

#### 7.10.3 AverageRating(productId) ⇒ double
1) **Store query**: `ReviewStore.GetAverageRating(productId)` (SQL `AVG(Rating)`).  
2) **If none**: return `0`.  
3) **Else**: return computed average.

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
   1.2) `User.Sign` (uniqueness check via UserStore).  
   1.3) **Success** → `GlobalMenuHolder.SwitchToRole(userId)`.  
   1.4) **Fail** → show reason; remain in Main.
2) **Login**  
   2.1) Prompt email/password.  
   2.2) `User.Login` (lookup via UserStore; sets Session on success).  
   2.3) **Success** → `SwitchToRole(userId)`.  
   2.4) **Fail** → “Invalid credentials”; remain in Main.
3) **Browse (Guest)**  
   3.1) Optional search query.  
   3.2) `Product.SearchByName` (SQL LIKE contains via ProductStore).  
   3.3) If none → “No products available”.
4) **Exit** → terminate process.

**Failure cases**
1) Bad inputs → stay in Main.  
2) Browse no results → friendly notice.

---

### 8.3 `CustomerMenu` (requires `userId`)
1) **Browse Products**  
   1.1) Search/list via entity method.  
   1.2) Show name, price, stock.
2) **Add to Cart**  
   2.1) Prompt product id & qty.  
   2.2) `Cart.AddItem` using Store checks.  
   2.3) On failure (not found/stock/qty) → show error.
3) **View Cart**  
   3.1) `Cart.GetTotal(userId)` (SQL sum via Store).  
   3.2) Render lines & totals.
4) **Checkout**  
   4.1) `Order.PlaceFromCart` (re‑validates stock via Store).  
   4.2) `Payment.ChargeWallet`.  
   4.3) If **Captured** → clear cart; success message.  
   4.4) If **Failed due to funds** → prompt:
        - **Yes** → go **Deposit Wallet**, then retry.  
        - **No** → return to menu with message.
5) **View Orders**  
   5.1) SQL filter by user via entity method; show latest first.  
6) **Add Review**  
   6.1) Prompt productId, rating, comment.  
   6.2) `Review.Submit` (Store validation).  
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
   1.2) `Product.Create` (Store INSERT).  
2) **Update Product**  
   2.1) Prompt id & new fields.  
   2.2) `Product.Update` (Store find+apply).  
3) **Delete Product**  
   3.1) Prompt id.  
   3.2) `Product.Delete` (Store DELETE).  
4) **Adjust Inventory**  
   4.1) Prompt id & delta.  
   4.2) `Administrator.AdjustInventory` (Store validate & apply).  
5) **View All Orders**  
   5.1) `Administrator.ListAllOrders` (SQL ORDER BY desc via Store).  
6) **Update Order Status**  
   6.1) Prompt order id & target status.  
   6.2) `Order.UpdateStatus` (Store find + transition rules).  
7) **Generate Report**  
   7.1) Prompt date range.  
   7.2) `Administrator.GenerateReport` (SQL GROUP BY / SUM via Store).  
8) **Logout**  
   8.1) `User.Logout()`; clear `CurrentUserId`; `Switch("main")`.

**Failure cases**
1) Non‑admin access (should be blocked by router).  
2) Invalid fields / negative stock / invalid transition → show error.

---

### 8.5 `GlobalMenuHolder` (router)
1) **Start**: register `MainMenu`, `CustomerMenu`, `AdminMenu`; set current = Main.  
2) **SwitchToRole(userId)**:  
   2.1) `CurrentUserId = Session.CurrentUserId` (set by Store on login/register).  
   2.2) `CurrentRole = Session.CurrentRole` (set by Store on login/register).  
   2.3) If `"Administrator"` → `Switch("admin")`, else `Switch("customer")`.  
3) **Run**: loop `current.Show(CurrentUserId)`.

**Failure cases**
1) Role lookup fails → show error; switch to Main.  
2) Missing menu key (dev mistake) → diagnostic; switch to Main.
## 9) Build & Project Files (Reference)

**`root/Program.cs`**
```csharp
using cmdDistrict.Models;

GlobalMenuHolder.Start();
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

- Later (v2): introduce a Services layer or swap ADO.NET for Dapper / EF Core.
- Add unit tests for entities and stores.

---

## 12) Links

- `../spec.md`
- `../plan.md`
- `./implementation.md` (this file)

---
