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
1. `Program.cs` → `GlobalMenuHolder.Bootstrap()`
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

---

## 6) InMemory Stores (No DB)

> Each store exposes minimal CRUD helpers. Use **static** collections for simplicity.

- **UserStore**: users by `Email` + by `Id`.
- **ProductStore**: list of products; find by id/name.
- **CartStore**: carts by `CustomerId`.
- **OrderStore**: orders list; by customer; by id.
- **ReviewStore**: reviews list; by product.

**Rules:**
- Only entity methods touch stores (Menus call entities, not stores directly) to keep flow clean.

---

## 7) Model Actions — Implementation Descriptions (Local)

> Each action touches **InMemory Stores** only. Use `DateProvider.UtcNow` for timestamps when needed.

### 7.1 `User` (abstract, `cmdDistrict.Models.Entities`)
- **Attributes**: `id : string`, `Name`, `Email`, `Password`, `Role`
- **Login(email, password) ⇒ (bool success, string userId, string role)**  
  `UserStore.GetByEmail(email)`; compare password; return tuple.
- **Sign(name, email, password, role) ⇒ (bool success, string userId)**  
  Fail if email exists; create user with new Guid; `UserStore.Add(user)`.
- **Logout() ⇒ bool**  
  No-op in local mode; return `true`.
- **GetRole(userId) ⇒ string**  
  Returns user's role from store.

### 7.2 `Customer : User`
- **Attributes**: `WalletBalance`, `DefaultShippingAddress`
- **Deposit(userId, amount) ⇒ bool**  
  Validate `amount > 0`; increment and persist on `UserStore`.
- **ViewOrders(userId) ⇒ List<Order>**  
  `OrderStore.GetByCustomer(userId)`.
- **ViewCart(userId) ⇒ Cart**  
  `CartStore.GetOrCreate(userId)`.

### 7.3 `Administrator : User`
- **AdjustInventory(userId, productId, delta) ⇒ bool**  
  Require role `Administrator`; `ProductStore.Get(productId).Stock += delta` (disallow < 0).
- **ListAllOrders(userId) ⇒ List<Order>**  
  Require role `Administrator`; return all from `OrderStore`.
- **GenerateReport(userId, from, to) ⇒ string**  
  Require role `Administrator`; aggregate orders in range and return text.

### 7.4 `Product`
- **Create(userId, name, description, price, stock) ⇒ Product**  
  Admin-only; validate; `ProductStore.Add(product)`.
- **Update(userId, id, name, description, price, stock) ⇒ bool**  
  Admin-only; validate; `ProductStore.Update(...)`.
- **Delete(userId, id) ⇒ bool**  
  Admin-only; `ProductStore.Remove(id)` (v1 allow even if referenced).
- **FindById(id) ⇒ Product?**  
  `ProductStore.Get(id)`.
- **SearchByName(query) ⇒ List<Product>**  
  Case-insensitive contains on `Name`.

### 7.5 `Cart`
- **AddItem(userId, productId, quantity) ⇒ bool**  
  Validate quantity; ensure product exists and stock ≥ quantity; merge/add item; `CartStore.Save(cart)`.
- **RemoveItem(userId, productId) ⇒ bool**  
  Remove line by product id; save.
- **Clear(userId) ⇒ void**  
  `cart.Items.Clear()`; save.
- **GetTotal(userId) ⇒ decimal**  
  Sum of `UnitPrice * Quantity`.

### 7.6 `CartItem`
- **UpdateQuantity(userId, newQuantity) ⇒ bool**  
  Validate `newQuantity > 0`; ensure product stock supports qty; update; save cart.

### 7.7 `Order`
- **PlaceFromCart(userId, cart) ⇒ Order**  
  Ensure cart belongs to user; snapshot items with current prices; decrement product stock; `OrderStore.Add(order)`; status `Pending`.
- **Cancel(userId, orderId) ⇒ bool**  
  If order belongs to user and not shipped: set `Cancelled`; restock if needed; save.
- **TrackStatus(userId, orderId) ⇒ OrderStatus**  
  Return status if owner; else throw `InvalidOperationException` (or return a sentinel in v1).
- **UpdateStatus(userId, orderId, status) ⇒ bool**  
  Admin-only; enforce transitions; save.

### 7.8 `OrderItem`
- **LineTotal() ⇒ decimal**  
  `UnitPrice * Quantity`.

### 7.9 `Payment`
- **ChargeWallet(userId, orderId, amount) ⇒ Payment**  
  If customer's wallet ≥ amount: deduct; payment `Captured`; order → `Paid`; save.
  If not inform of lacking funds, ask if 'would you like to add funds' if yes send to wallet if no send to Customer menu with message.
- **Refund(userId, paymentId) ⇒ bool**  
  Admin-only; credit wallet; mark refunded.

### 7.10 `Review`
- **Submit(userId, productId, rating, comment) ⇒ bool**  
  Validate rating 1..5; `ReviewStore.Add(review)`.
- **GetForProduct(productId) ⇒ List<Review>**  
  `ReviewStore.GetByProduct(productId)`.
- **AverageRating(productId) ⇒ double**  
  Compute average; `0` if none.

---

## 8) Menus — Behavior & Routing (Local)

### 8.1 `Menu` (abstract, `cmdDistrict.Models`)
- Members: `Key`, `Title`, `ContextUserId?`
- `Show(userId?)` sets context; prints title; `PrintOptions()`; read; `HandleSelection()`.

### 8.2 `MainMenu` (guest)
- **Register** → `User.Sign` (role chosen: Customer or Admin) → `GlobalMenuHolder.SwitchToRole(userId)`.
- **Login** → `User.Login` → on success route by role.
- **Browse (Guest)** → read-only `Product.SearchByName` or list all.
- **Exit** → terminate.

### 8.3 `CustomerMenu` (needs `userId`)
- Browse → list/search products.
- Add to Cart → `Cart.AddItem(userId, productId, qty)`.
- View Cart → list lines & total.
- Checkout → `Order.PlaceFromCart(userId, cart)` then `Payment.ChargeWallet(userId, order.Id, order.Total)` → on success clear cart.
- View Orders → `Customer.ViewOrders(userId)`.
- Add Review → `Review.Submit(userId, ...)`.
- Deposit Wallet → `Customer.Deposit(userId, amount)`.
- Logout → `GlobalMenuHolder.Switch("main")`.

### 8.4 `AdminMenu` (needs `userId`)
- Manage Products → `Product.Create/Update/Delete`.
- Adjust Inventory → `Administrator.AdjustInventory(userId, productId, delta)`.
- Orders → `Administrator.ListAllOrders(userId)`.
- Update Status → `Order.UpdateStatus(userId, orderId, status)`.
- Report → `Administrator.GenerateReport(userId, from, to)`.
- Logout → main.

### 8.5 `GlobalMenuHolder`
- Registers `MainMenu`, `CustomerMenu`, `AdminMenu`.
- Holds `CurrentUserId`, `CurrentRole`.
- `SwitchToRole(userId)` → resolves role via `User.GetRole(userId)` → switch to `admin` or `customer`.
- `Run()` loops calling current menu `Show(CurrentUserId)`.

---

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
