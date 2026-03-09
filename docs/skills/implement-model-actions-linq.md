# Skill: implement-model-actions-linq

**description:** Implement **local-only** behavior using **LINQ** over `AppState` static lists (no services, no stores). Implement **Login/Sign/Logout/GetRole**, Customer/Admin actions, Product CRUD, Cart ops, Order lifecycle, Payment charge/refund, Reviews — **exactly** as in `docs/specs/implementation.md` §7 with numbered flows and failure cases.

## When to use
After entities exist and you want the **actual behavior** implemented.

## Conventions
- Data holders: `AppState.Users`, `AppState.Products`, `AppState.Carts`, `AppState.Orders`, `AppState.Reviews`
- Timestamps: `DateProvider.UtcNow`
- LINQ only: `SingleOrDefault`, `FirstOrDefault`, `Any`, `Where`, `Select`, `SelectMany`, `GroupBy`, `OrderBy*`, `Sum`, `Average`
- Errors: throw `InvalidOperationException` only where the spec says throw; otherwise return `false`/`null`

## Files to create/update
```
src/CmdDistrict/Common/AppState.cs          // public static lists
src/CmdDistrict/Common/DateProvider.cs      // already referenced
src/CmdDistrict/Models/Entities/*.cs        // add methods implementing §7 flows
```

## Steps
1) Create `AppState` with public static `List<T>` for Users, Products, Carts, Orders, Reviews.
2) Implement **User** actions: Login, Sign (date+random id), Logout, GetRole using LINQ on `AppState.Users`.
3) Implement **Customer**: Deposit, ViewOrders, ViewCart (GetOrCreate) via LINQ.
4) Implement **Administrator**: AdjustInventory, ListAllOrders, GenerateReport via LINQ & aggregates.
5) Implement **Product**: Create/Update/Delete/Find/Search via LINQ.
6) Implement **Cart**: AddItem/RemoveItem/Clear/GetTotal with stock checks (decrement at **order placement** only).
7) Implement **Order**: PlaceFromCart (re-validate stock, snapshot, decrement, total), Cancel (restock rules), TrackStatus, UpdateStatus (state machine).
8) Implement **Payment**: ChargeWallet (insufficient funds → return Failed; menu will prompt to deposit), Refund (admin) credits wallet.
9) Implement **Review**: Submit (rating bounds), GetForProduct, AverageRating.
10) Keep code local — no external deps or services.

## How to use this skill in VS Code
1) Open `docs/skills/implement-model-actions-linq.md`.
2) Copy the **Prompt template** below into Copilot Chat/Claude.
3) Ensure `AppState.cs` is created and referenced in your code.
4) Ask the assistant to implement methods directly in each entity file under `src/CmdDistrict/Models/Entities/` without renaming files.
5) Build with `dotnet run` and test flows per §7.

## Prompt template
```text
Skill: implement-model-actions-linq
Goal: Implement all entity behaviors using LINQ over AppState (no DB, no services).
Create/Update:
- src/CmdDistrict/Common/AppState.cs with static lists: Users, Products, Carts, Orders, Reviews
- Implement methods in entity files to match the exact flows & failures in §7

Strict rules:
- Use DateProvider.UtcNow for timestamps
- Use LINQ only (SingleOrDefault, Where, Select, Sum, Any, GroupBy, OrderBy*)
- Throw InvalidOperationException where §7 says throw; otherwise return false/null
- Do not rename files or types

Link: ../specs/implementation.md#7-model-actions--implementation-descriptions-local-step-by-step-with-failure-cases
```
