# SDS.md — System Domain Specification

## Classes :

---

### src/Models/Entities/User
**Namespace:** cmdDistrict.Models.Entities

#### User Entity  
**abstract class User :**

**Attributes**
- id : string
- Name : string
- Email : string
- Password : string
- Role : string  _(values: "Customer" | "Administrator")_

**Actions**
- public static Login : string email, string password ⇒ (bool success, string userId, string role)
- public static Sign : string name, string email, string password, string role ⇒ (bool success, string userId)
- public Logout : User ⇒ bool
- public static GetRole : string userId ⇒ string

---

### src/Models/Entities/Customer
**Namespace:** cmdDistrict.Models.Entities

#### Customer : User

**Attributes**
- WalletBalance : decimal
- DefaultShippingAddress : string?

**Actions**
- Deposit : string userId, decimal amount ⇒ bool
- ViewOrders : string userId ⇒ List<Order>
- ViewCart : string userId ⇒ Cart

---

### src/Models/Entities/Administrator
**Namespace:** cmdDistrict.Models.Entities

#### Administrator : User

**Attributes**
- PermissionLevel : string

**Actions**
- AdjustInventory : string userId, string productId, int delta ⇒ bool
- ListAllOrders : string userId ⇒ List<Order>
- GenerateReport : string userId, DateTime from, DateTime to ⇒ string

---

### src/Models/Entities/Product
**Namespace:** cmdDistrict.Models.Entities

#### Product

**Attributes**
- id : string
- Name : string
- Description : string
- Price : decimal
- Stock : int

**Actions**
- Create : string userId, string name, string description, decimal price, int stock ⇒ Product
- Update : string userId, string id, string name, string description, decimal price, int stock ⇒ bool
- Delete : string userId, string id ⇒ bool
- FindById : string id ⇒ Product?
- SearchByName : string query ⇒ List<Product>

---

### src/Models/Entities/Cart
**Namespace:** cmdDistrict.Models.Entities

#### Cart

**Attributes**
- id : string
- CustomerId : string
- Items : List<CartItem>

**Actions**
- AddItem : string userId, string productId, int quantity ⇒ bool
- RemoveItem : string userId, string productId ⇒ bool
- Clear : string userId ⇒ void
- GetTotal : string userId ⇒ decimal

---

### src/Models/Entities/CartItem
**Namespace:** cmdDistrict.Models.Entities

#### CartItem

**Attributes**
- id : string
- ProductId : string
- ProductName : string
- UnitPrice : decimal
- Quantity : int

**Actions**
- UpdateQuantity : string userId, int newQuantity ⇒ bool

---

### src/Models/Entities/Order
**Namespace:** cmdDistrict.Models.Entities

#### Order

**Attributes**
- id : string
- CustomerId : string
- Items : List<OrderItem>
- Total : decimal
- Status : OrderStatus

**Actions**
- PlaceFromCart : string userId, Cart ⇒ Order
- Cancel : string userId, string orderId ⇒ bool
- TrackStatus : string userId, string orderId ⇒ OrderStatus
- UpdateStatus : string userId, string orderId, OrderStatus status ⇒ bool

---

### src/Models/Entities/OrderItem
**Namespace:** cmdDistrict.Models.Entities

#### OrderItem

**Attributes**
- id : string
- ProductId : string
- ProductName : string
- UnitPrice : decimal
- Quantity : int

**Actions**
- LineTotal : void ⇒ decimal

---

### src/Models/Entities/Payment
**Namespace:** cmdDistrict.Models.Entities

#### Payment

**Attributes**
- id : string
- OrderId : string
- CustomerId : string
- Amount : decimal
- Status : PaymentStatus

**Actions**
- ChargeWallet : string userId, string orderId, decimal amount ⇒ Payment
- Refund : string userId, string paymentId ⇒ bool

---

### src/Models/Entities/Review
**Namespace:** cmdDistrict.Models.Entities

#### Review

**Attributes**
- id : string
- ProductId : string
- CustomerId : string
- Rating : int
- Comment : string

**Actions**
- Submit : string userId, string productId, int rating, string comment ⇒ bool
- GetForProduct : string productId ⇒ List<Review>
- AverageRating : string productId ⇒ double

---

## Menus

---

### src/Models/Menu/Menu.cs
**Namespace:** cmdDistrict.Models

#### abstract class Menu

**Attributes**
- ContextUserId : string?  _(null for guest flows)_

**Actions**
- Show : string? userId ⇒ void  
  _(Sets ContextUserId internally; shows options for that user context)_
- PrintOptions : void ⇒ void
- HandleSelection : string selection ⇒ bool

---

### Main Menu
**Child of Menu (guest or unauthenticated)**

**Functionality**
- Register User (returns new userId)
- Login (Customer / Admin) (returns userId, role)
- Browse Products (Guest)
- Exit

**Notes**
- On successful login/registration, call **GlobalMenuHolder.SwitchToRole(userId)** to route to the appropriate role menu.

---

### Customer Menu
**Child of Menu (requires userId)**

**Context**
- Requires **ContextUserId** (the logged-in customer’s id)

**Functionality**
- Browse Products
- Add Product to Cart (uses ContextUserId)
- View Cart (uses ContextUserId)
- Checkout (creates Order + Payment with ContextUserId)
- View Orders (ContextUserId)
- Add Review (ContextUserId)
- Deposit Wallet Funds (ContextUserId)
- Logout (clears ContextUserId and routes to Main)

---

### Administrator Menu
**Child of Menu (requires userId)**

**Context**
- Requires **ContextUserId** (the logged-in admin’s id)

**Functionality**
- Add Product (ContextUserId)
- Update Product (ContextUserId)
- Delete Product (ContextUserId)
- Adjust Inventory (ContextUserId)
- View All Orders (ContextUserId)
- Update Order Status (ContextUserId)
- Generate Report (ContextUserId)
- Logout (clears ContextUserId and routes to Main)

---

## Global Menu Holder

---

### src/Models/Menu/GlobalMenuHolder.cs
**Namespace:** cmdDistrict.Models

#### Global Menu Manager

**Attributes**
- Menus : Dictionary<string, Menu>
- CurrentMenu : Menu
- CurrentUserId : string?
- CurrentRole : string?  _(“Customer” | “Administrator” | null)_

**Actions**
- Register : Menu ⇒ void
- Switch : string key ⇒ void
- SwitchToRole : string userId ⇒ void  
  _(Sets CurrentUserId, resolves role via `User.GetRole(userId)`, sets CurrentRole, then Switches to `"customer"` or `"admin"` accordingly)_
- Run : void ⇒ void
- Bootstrap : void ⇒ void  
  _(Registers Main, Customer, Admin menus; sets starting menu to Main with `CurrentUserId = null`)_
