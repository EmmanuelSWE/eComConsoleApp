# Skill: 12-sync-product-read-to-spec (EF Core LINQ)

**Goal:** Implement **read-only** product paths with EF Core LINQ; Admin writes come next.

## Files allowed to change
- src/CmdDistrict/Infrastructure/Stores.Ef/ProductStoreEf.cs  *(create)*
- src/CmdDistrict/Models/Entities/Product.cs                  *(update read-only method bodies)*

## Notes
- Keep **bool-only** returns; if you must surface objects, you may set a small read context holder (similar to `Session`) or accept `out` parameters.

## Guardrails
- Do not implement create/update/delete here.
- Do not edit Cart/Order code.

## Prompt template
```
Skill: 12-sync-product-read-to-spec (EF Core LINQ)
Goal: Migrate ONLY Product read paths to EF Core LINQ via Stores.Ef, keeping bool-only pattern.
Files allowed: Stores.Ef/ProductStoreEf.cs (create), Entities/Product.cs (update read methods)
Rules:
- No admin writes in this phase.
- Abort if other paths touched.
Deliverable: diffs + changelog.
```
