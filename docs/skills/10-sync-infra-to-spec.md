# Skill: 10-sync-infra-to-spec (EF Core LINQ)

**Goal:** Create minimal EF Core infrastructure as specified in the updated docs:
- `AppDbContext.cs` (DbContext using `UseSqlServer`)
- `Session.cs` (holds CurrentUserId/CurrentRole; Set/Clear)
- `Infrastructure/Stores.Ef/` folder (add `.gitkeep`)

## Files allowed to change
- src/CmdDistrict/DataAccess/AppDbContext.cs           *(create)*
- src/CmdDistrict/Common/Session.cs                    *(create)*
- src/CmdDistrict/Infrastructure/Stores.Ef/.gitkeep    *(create)*

## Guardrails
- No edits to Entities or Menus.
- Do not delete `AppState`.

## Prompt template
```
Skill: 10-sync-infra-to-spec (EF Core LINQ)
Goal: Create AppDbContext.cs, Session.cs, and Stores.Ef folder only.
Files allowed: [src/CmdDistrict/DataAccess/AppDbContext.cs, src/CmdDistrict/Common/Session.cs, src/CmdDistrict/Infrastructure/Stores.Ef/.gitkeep]
Rules:
- No entity/menu edits. No AppState removal.
- AppDbContext must call options.UseSqlServer(connectionString).
Deliverable: created files and short changelog.
```
