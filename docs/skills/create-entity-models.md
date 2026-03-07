# Skill: create-entity-models

**description:** Scaffold strongly-typed **C# entity classes and enums** that match the domain (User, Customer, Administrator, Product, Cart, CartItem, Order, OrderItem, Payment, Review). Enforce **id format (date+random)**, **namespaces**, and **.NET 8** conventions. **No services, no database.**

## When to use
Use this skill when adding or refactoring **domain entities** to match the spec.

## Conventions
- Runtime: **.NET 8**, C# 12
- Namespace: **`cmdDistrict.Models.Entities`**
- **Id format (User only):** `$"{DateProvider.UtcNow:yyyyMMddHHmmss}-{Random(6 digits)}"`
- Others: use `Guid.NewGuid().ToString()` unless the spec says otherwise
- One **public class per file**; path matches namespace
- Behavior is **not** implemented here (see `implement-model-actions-linq`)

## Files to create
```
src/CmdDistrict/Models/Entities/Enums.cs
src/CmdDistrict/Models/Entities/User.cs
src/CmdDistrict/Models/Entities/Customer.cs
src/CmdDistrict/Models/Entities/Administrator.cs
src/CmdDistrict/Models/Entities/Product.cs
src/CmdDistrict/Models/Entities/CartItem.cs
src/CmdDistrict/Models/Entities/Cart.cs
src/CmdDistrict/Models/Entities/OrderItem.cs
src/CmdDistrict/Models/Entities/Order.cs
src/CmdDistrict/Models/Entities/Payment.cs
src/CmdDistrict/Models/Entities/Review.cs
```

## Steps
1) Define `OrderStatus` and `PaymentStatus` enums in `Enums.cs`.
2) Create **`User`** (abstract) with: `Id` (date+random string), `Name`, `Email`, `Password`, `Role`.
3) Create **`Customer : User`** with: `WalletBalance`, `DefaultShippingAddress`.
4) Create **`Administrator : User`** with: `PermissionLevel`.
5) Create **`Product`, `CartItem`, `Cart`, `OrderItem`, `Order`, `Payment`, `Review`** with properties per spec.
6) Ensure namespaces / file names match exactly.
7) Do **not** implement behavior here (that is Skill 2).

## How to use this skill in VS Code
1) Open `docs/skills/create-entity-models.md`.
2) Copy the **Prompt template** below into Copilot Chat/Claude.
3) Replace placeholders with your entity list (if partial) and run.
4) The assistant should create files in the exact paths under `src/CmdDistrict/Models/Entities/`.
5) Build with `dotnet run` and fix any compile errors.

## Prompt template
```text
Skill: create-entity-models
Goal: Scaffold C# entity classes & enums for CmdDistrict (local, no DB, no services).
Constraints:
- .NET 8; C# 12; namespace cmdDistrict.Models.Entities
- One public class per file; do NOT rename or move files
- Implement ONLY properties and any simple constructors; no behavior yet

Entities to create/update: [User, Customer, Administrator, Product, CartItem, Cart, OrderItem, Order, Payment, Review]
Special rules:
- User.Id => $"{DateProvider.UtcNow:yyyyMMddHHmmss}-{Random(6 digits)}"
- Others may use Guid for Id unless SDS says otherwise

Links:
- Models & actions spec: ../specs/implementation.md#7-model-actions--implementation-descriptions-local-step-by-step-with-failure-cases
- Menu & routing spec: ../specs/implementation.md#8-menus--behavior--routing-local-step-by-step-with-failure-cases
```
