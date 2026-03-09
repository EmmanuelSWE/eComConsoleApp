[Link to GitHub](https://github.com/EmmanuelSWE/eComConsoleApp)

# cmdDistrict — Phase Two

## What is cmdDistrict?

A C# console application that simulates the backend of a real e-commerce platform. Customers can browse products, manage their cart, place orders, and process payments — while administrators manage inventory, orders, and generate reports.

Phase two introduces the **Command design pattern** across all menus, decoupling menu layout from business logic entirely. It also adds **inline option descriptions** so users always know what each menu choice does before selecting it.

## Why cmdDistrict?

- Role-based access for Customers and Administrators.
- Wallet-based payment simulation — no external payment APIs.
- Persistent data via EF Core + SQL Server.
- Clean menu-driven interface navigable with arrow keys.
- Every menu option now shows a short description of what it does.
- Business logic is fully decoupled from menu layout via the Command pattern.

## Phase Two — What Changed

### Command Design Pattern

Every menu action is now an isolated command class that encapsulates exactly one piece of business logic. Menus hold a list of commands and call `.Execute()` — they have no knowledge of what the command actually does internally.

```
Menu
 └── holds List<ICommand>
       ├── PlaceOrderCommand    → calls Order.PlaceFromCart(...)
       ├── AddToCartCommand     → calls Cart.AddItem(...)
       ├── DepositCommand       → calls Customer.Deposit(...)
       └── ...
```

**What this gives you:**

- **Decoupled layout from logic** — the menu can be restructured, reordered, or restyled without touching any business logic. Refactoring a menu is purely a UI concern.
- **Isolated responsibility** — each command owns one operation. Debugging `CheckoutCommand` means opening one file, not tracing through a long menu class.
- **Easy to extend** — adding a new feature means writing a new command class and registering it in the menu. No existing commands are modified, so nothing breaks.
- **Consistent error handling** — each command wraps its own try/catch, keeping failures isolated so the menu loop always stays stable.

### Menu Option Descriptions

Each menu now includes a dedicated **Describe Options** entry as a selectable menu option. When chosen, it prints a description of every available action in that menu so the user understands what each option does before committing to one.

```
  > Describe Options

    Browse Products     — View the full product catalog with names, prices, and stock.
    Add to Cart         — Select a product and quantity to add to your cart.
    Checkout            — Pay for your current cart items using your wallet balance.
    ...
```

Each command class exposes its own description string, keeping it co-located with the logic it describes and making it trivial to update.

# Documentation

## Software Requirement Specification

### Overview

cmdDistrict is a console-based e-commerce backend built on .NET 8 and C# 12. It demonstrates object-oriented design, LINQ-to-Entities queries via EF Core, role-based routing, the Command design pattern, and structured input validation — all within a console environment.

### Components and Functional Requirements

**1. User Authentication**
- Users can register as a Customer or Administrator.
- Users can log in and are routed to their role-specific menu automatically.
- Users can log out and return to the main menu.

**2. Customer Features**
- Browse and search the product catalog.
- Add products to cart, update quantities, and remove items.
- Checkout using wallet balance; prompted to deposit if funds are insufficient.
- View order history and track order status.
- Submit product reviews and ratings.
- Deposit funds into wallet.

**3. Administrator Features**
- Add, update, and delete products.
- Adjust product inventory stock levels.
- View all orders and update order status with transition rules enforced.
- View low-stock products.
- Generate sales reports over a specified date range.

# Architecture

## Folder Layout

```
CmdDistrict/
├───docs/
│   ├───skills/
│   └───specs/
│
└───src/
    └───CmdDistrict/
        ├───Common/
        │   ├─ Guard.cs
        │   ├─ Result.cs
        │   ├─ DateProvider.cs
        │   └─ Session.cs
        │
        ├───DataAccess/
        │   └─ AppDbContext.cs
        │
        ├───DesignPattern/
        │   └───Command/
        │       ├─ ICommand.cs
        │       ├─ LoginCommand.cs
        │       ├─ RegisterCommand.cs
        │       ├─ AddToCartCommand.cs
        │       ├─ CheckoutCommand.cs
        │       ├─ PlaceOrderCommand.cs
        │       ├─ DepositCommand.cs
        │       ├─ ViewOrdersCommand.cs
        │       ├─ AddReviewCommand.cs
        │       ├─ AddProductCommand.cs
        │       ├─ UpdateProductCommand.cs
        │       ├─ DeleteProductCommand.cs
        │       ├─ AdjustInventoryCommand.cs
        │       ├─ UpdateOrderStatusCommand.cs
        │       ├─ GenerateReportCommand.cs
        │       └─ LogoutCommand.cs
        │
        ├───Infrastructure/
        │   └───Stores.Ef/
        │       ├─ UserStoreEf.cs
        │       ├─ ProductStoreEf.cs
        │       ├─ CartStoreEf.cs
        │       ├─ OrderStoreEf.cs
        │       └─ ReviewStoreEf.cs
        │
        └───Models/
            ├───Entities/
            │   ├─ Enums.cs
            │   ├─ User.cs
            │   ├─ Customer.cs
            │   ├─ Administrator.cs
            │   ├─ Product.cs
            │   ├─ CartItem.cs
            │   ├─ Cart.cs
            │   ├─ OrderItem.cs
            │   ├─ Order.cs
            │   ├─ Payment.cs
            │   └─ Review.cs
            │
            └───Menu/
                ├─ Menu.cs
                ├─ MainMenu.cs
                ├─ CustomerMenu.cs
                ├─ AdminMenu.cs
                └─ GlobalMenuHolder.cs
```

## Tech Stack

- **.NET 8** Console Application
- **C# 12**
- **EF Core** — `Microsoft.EntityFrameworkCore.SqlServer`
- **SQL Server** via Docker

# Running the Application

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [Docker](https://www.docker.com/) (for SQL Server)
- Git

## Setup

**1. Clone the repo**
```bash
git clone https://github.com/EmmanuelSWE/eComConsoleApp
cd eComConsoleApp
```

**2. Start SQL Server via Docker**
```bash
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=<your_sa_pw>" \
  -p 1433:1433 --name sqlserver -d mcr.microsoft.com/mssql/server:2022-latest
```

**3. Create the database**

Connect to your SQL Server instance and run the following SQL to create the `CmdDistrict` database and all required tables:

```sql
CREATE DATABASE CmdDistrict;
GO

USE CmdDistrict;
GO

-- Users (base table for Customer and Administrator)
CREATE TABLE dbo.Users (
    Id                      NVARCHAR(36)   NOT NULL CONSTRAINT PK_Users PRIMARY KEY,
    Name                    NVARCHAR(200)  NOT NULL,
    Email                   NVARCHAR(320)  NOT NULL CONSTRAINT UQ_Users_Email UNIQUE,
    Password                NVARCHAR(256)  NOT NULL,
    Role                    NVARCHAR(50)   NOT NULL,  -- 'Customer' | 'Administrator'
    WalletBalance           DECIMAL(18,2)  NULL,      -- Customer only
    DefaultShippingAddress  NVARCHAR(500)  NULL,      -- Customer only
    PermissionLevel         NVARCHAR(100)  NULL       -- Administrator only
);
GO

-- Products
CREATE TABLE dbo.Products (
    Id          NVARCHAR(36)    NOT NULL CONSTRAINT PK_Products PRIMARY KEY,
    Name        NVARCHAR(200)   NOT NULL,
    Description NVARCHAR(1000)  NOT NULL,
    Price       DECIMAL(18,2)   NOT NULL,
    Stock       INT             NOT NULL
);
GO

-- Carts
CREATE TABLE dbo.Carts (
    Id          NVARCHAR(36) NOT NULL CONSTRAINT PK_Carts PRIMARY KEY,
    CustomerId  NVARCHAR(36) NOT NULL CONSTRAINT FK_Carts_Users FOREIGN KEY REFERENCES dbo.Users(Id)
);
GO

-- CartItems
CREATE TABLE dbo.CartItems (
    Id          NVARCHAR(36)   NOT NULL CONSTRAINT PK_CartItems PRIMARY KEY,
    CartId      NVARCHAR(36)   NOT NULL CONSTRAINT FK_CartItems_Carts FOREIGN KEY REFERENCES dbo.Carts(Id),
    ProductId   NVARCHAR(36)   NOT NULL CONSTRAINT FK_CartItems_Products FOREIGN KEY REFERENCES dbo.Products(Id),
    ProductName NVARCHAR(200)  NOT NULL,
    UnitPrice   DECIMAL(18,2)  NOT NULL,
    Quantity    INT            NOT NULL
);
GO

-- Orders
CREATE TABLE dbo.Orders (
    Id          NVARCHAR(36)   NOT NULL CONSTRAINT PK_Orders PRIMARY KEY,
    CustomerId  NVARCHAR(36)   NOT NULL CONSTRAINT FK_Orders_Users FOREIGN KEY REFERENCES dbo.Users(Id),
    Total       DECIMAL(18,2)  NOT NULL,
    Status      INT            NOT NULL  -- 0=Pending | 1=Paid | 2=Shipped | 3=Delivered | 4=Cancelled
);
GO

-- OrderItems
CREATE TABLE dbo.OrderItems (
    Id          NVARCHAR(36)   NOT NULL CONSTRAINT PK_OrderItems PRIMARY KEY,
    OrderId     NVARCHAR(36)   NOT NULL CONSTRAINT FK_OrderItems_Orders FOREIGN KEY REFERENCES dbo.Orders(Id),
    ProductId   NVARCHAR(36)   NOT NULL,
    CartId      NVARCHAR(36)   NOT NULL,
    ProductName NVARCHAR(200)  NOT NULL,
    UnitPrice   DECIMAL(18,2)  NOT NULL,
    Quantity    INT            NOT NULL
);
GO

-- Payments
CREATE TABLE dbo.Payments (
    Id          NVARCHAR(36)   NOT NULL CONSTRAINT PK_Payments PRIMARY KEY,
    OrderId     NVARCHAR(36)   NOT NULL CONSTRAINT FK_Payments_Orders FOREIGN KEY REFERENCES dbo.Orders(Id),
    CustomerId  NVARCHAR(36)   NOT NULL CONSTRAINT FK_Payments_Users FOREIGN KEY REFERENCES dbo.Users(Id),
    Amount      DECIMAL(18,2)  NOT NULL,
    Status      INT            NOT NULL  -- 0=Pending | 1=Captured | 2=Failed | 3=Refunded
);
GO

-- Reviews
CREATE TABLE dbo.Reviews (
    Id          NVARCHAR(36)    NOT NULL CONSTRAINT PK_Reviews PRIMARY KEY,
    ProductId   NVARCHAR(36)    NOT NULL CONSTRAINT FK_Reviews_Products FOREIGN KEY REFERENCES dbo.Products(Id),
    CustomerId  NVARCHAR(36)    NOT NULL CONSTRAINT FK_Reviews_Users FOREIGN KEY REFERENCES dbo.Users(Id),
    Rating      INT             NOT NULL,  -- 1 to 5
    Comment     NVARCHAR(1000)  NOT NULL
);
GO
```

**4. Set your connection string**

In `appsettings.json` or your environment:
```
Server=localhost,1433;Database=CmdDistrict;User Id=sa;Password=<your_sa_pw>;Encrypt=True;TrustServerCertificate=True;
```

**5. Apply migrations & run**
```bash
cd src/CmdDistrict
dotnet ef database update
dotnet run
```

## Development

1. Go to the GitHub repo:
### [Link to repo](https://github.com/EmmanuelSWE/eComConsoleApp)
2. Clone the repo.
3. Follow the setup steps above.