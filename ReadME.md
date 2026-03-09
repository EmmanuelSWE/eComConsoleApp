[Link to GitHub](https://github.com/EmmanuelSWE/eComConsoleApp)

# cmdDistrict

## What is cmdDistrict?

A C# console application that simulates the backend of a real e-commerce platform. Customers can browse products, manage their cart, place orders, and process payments — while administrators manage inventory, orders, and generate reports.

## Why cmdDistrict?

- Role-based access for Customers and Administrators.
- Wallet-based payment simulation — no external payment APIs.
- Persistent data via EF Core + SQL Server.
- Clean menu-driven interface navigable with arrow keys.

# Documentation

## Software Requirement Specification

### Overview

cmdDistrict is a console-based e-commerce backend built on .NET 8 and C# 12. It demonstrates object-oriented design, LINQ-to-Entities queries via EF Core, role-based routing, and structured input validation — all within a console environment.

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
.
├─ docs/
│  ├─ spec.md
│  ├─ plan.md
│  └─ implementation.md
│
└─ src/
   └─ CmdDistrict/
      ├─ CmdDistrict.csproj
      ├─ Program.cs
      │
      ├─ Common/
      │  ├─ Guard.cs
      │  ├─ Result.cs
      │  ├─ DateProvider.cs
      │  └─ Session.cs
      │
      ├─ DataAccess/
      │  └─ AppDbContext.cs
      │
      ├─ Models/
      │  ├─ Entities/
      │  │  ├─ Enums.cs
      │  │  ├─ User.cs
      │  │  ├─ Customer.cs
      │  │  ├─ Administrator.cs
      │  │  ├─ Product.cs
      │  │  ├─ CartItem.cs
      │  │  ├─ Cart.cs
      │  │  ├─ OrderItem.cs
      │  │  ├─ Order.cs
      │  │  ├─ Payment.cs
      │  │  └─ Review.cs
      │  │
      │  └─ Menu/
      │     ├─ Menu.cs
      │     ├─ MainMenu.cs
      │     ├─ CustomerMenu.cs
      │     ├─ AdminMenu.cs
      │     └─ GlobalMenuHolder.cs
      │
      └─ Infrastructure/
         └─ Stores.Ef/
            ├─ UserStoreEf.cs
            ├─ ProductStoreEf.cs
            ├─ CartStoreEf.cs
            ├─ OrderStoreEf.cs
            └─ ReviewStoreEf.cs
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

**3. Set your connection string**

In `appsettings.json` or your environment:
```
Server=localhost,1433;Database=CmdDistrict;User Id=sa;Password=<your_sa_pw>;Encrypt=True;TrustServerCertificate=True;
```

**4. Apply migrations & run**
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