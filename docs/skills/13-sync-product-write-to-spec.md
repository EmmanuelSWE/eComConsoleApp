# Skill: 13-sync-product-write-to-spec (EF Core LINQ)

**Goal:** Implement Admin Product **create/update/delete** via EF Core LINQ stores.

## Files allowed to change
- src/CmdDistrict/Infrastructure/Stores.Ef/ProductStoreEf.cs  *(update write ops)*
- src/CmdDistrict/Models/Entities/Product.cs                  *(update write method bodies)*

## Guardrails
- LINQ to Entities only; call `SaveChanges()`.
- Bool-only returns; minimal diffs.

## Prompt template
```
Skill: 13-sync-product-write-to-spec (EF Core LINQ)
Goal: Implement Product write ops via Stores.Ef (bool-only).
Files allowed: Stores.Ef/ProductStoreEf.cs, Entities/Product.cs (write methods only)
Rules:
- Abort if other paths touched.
Deliverable: diffs + changelog.
```
