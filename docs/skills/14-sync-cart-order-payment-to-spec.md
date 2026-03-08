# Skill: 14-sync-cart-order-payment-to-spec (EF Core LINQ)

**Goal:** Implement Cart + Order + Payment behaviors using EF Core LINQ stores (happy path first).  
Keep bool-only returns; set any transient values via small context holders if needed.

## Files allowed to change
- src/CmdDistrict/Infrastructure/Stores.Ef/CartStoreEf.cs
- src/CmdDistrict/Infrastructure/Stores.Ef/OrderStoreEf.cs
- src/CmdDistrict/Infrastructure/Stores.Ef/PaymentStoreEf.cs
- src/CmdDistrict/Models/Entities/Cart.cs
- src/CmdDistrict/Models/Entities/Order.cs
- src/CmdDistrict/Models/Entities/Payment.cs

## Guardrails
- Throw only where the spec mandates; else return false/null.
- Use EF Core LINQ and `SaveChanges()`; no raw SQL unless necessary.

## Prompt template
```
Skill: 14-sync-cart-order-payment-to-spec (EF Core LINQ)
Goal: Implement Cart/Order/Payment to EF Core LINQ via Stores.Ef (bool-only) as per specs.
Files allowed: Stores.Ef/{Cart,Order,Payment}StoreEf.cs and Entities/{Cart,Order,Payment}.cs
Rules:
- Minimal diffs; abort if other paths touched.
Deliverable: diffs + changelog.
```
