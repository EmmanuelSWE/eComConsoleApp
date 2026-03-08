# Skill: 03-sync-infra-to-spec

**Goal:** Create the minimal SQL infra the specs now require, without changing menus or non-auth entities.
- Create `Db.cs` (ADO.NET helper).
- Create `Session.cs` (holds CurrentUserId/CurrentRole with Set/Clear).
- Create `Infrastructure/Stores.Sql/` folder (add `.gitkeep`).

## Files allowed to change
- src/CmdDistrict/DataAccess/Db.cs           *(create)*
- src/CmdDistrict/Common/Session.cs          *(create)*
- src/CmdDistrict/Infrastructure/Stores.Sql/.gitkeep  *(create)*

## Guardrails
- No edits to Entities or Menus.
- Do not delete `AppState`.
- Parameterized `SqlCommand` only (when you add any code in later phases).

## Prompt template
```
Skill: 03-sync-infra-to-spec
Goal: Create Db.cs, Session.cs, and Stores.Sql folder only (as per updated specs).
Files allowed: [src/CmdDistrict/DataAccess/Db.cs, src/CmdDistrict/Common/Session.cs, src/CmdDistrict/Infrastructure/Stores.Sql/.gitkeep]
Rules:
- No entity/menu edits. No AppState removal.
- Minimal code. Db.Open() reads CMD_SQLSERVER_CS with a safe dev fallback.
Deliverable: created files and short changelog.
```
