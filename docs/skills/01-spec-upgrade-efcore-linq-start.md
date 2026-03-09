# Skill: 01-spec-upgrade-efcore-linq-start

**Goal:** Update **spec documents only** to adopt **EF Core (Entity Framework Core) with SQL Server** using **LINQ to Entities**; keep router method name **Start()** (not Bootstrap) and the **stores-only** pattern with **bool-only** method returns that set state internally (e.g., `Session`).

## Files allowed to change (docs only)
- docs/specs/spec.md
- docs/specs/plan.md
- docs/specs/implementation.md
- docs/specs/models.md
- docs/skills/README.md  *(index only; do not edit legacy skills)*

## Required doc changes (surgical)
1) **Persistence**  
   Replace any "in-memory/AppState/LINQ-over-Lists" language with: **Persistence uses EF Core + SQL Server provider** (`UseSqlServer`) so **LINQ queries are translated to SQL** and executed by SQL Server. *Entities call Stores.Ef; Menus call Entities; no services layer.*

2) **Returns & state**  
   Store methods return **`bool` only**; on success they **set values internally** (e.g., `Session.CurrentUserId`, `Session.CurrentRole`). Entities keep existing public signatures and read needed values from `Session`.

3) **Router name**  
   Change **`GlobalMenuHolder.Bootstrap()` → `GlobalMenuHolder.Start()`** in every snippet and reference.

4) **Folder diagram (implementation.md)**  
   Add:
   - `src/CmdDistrict/DataAccess/AppDbContext.cs` *(EF Core DbContext; `UseSqlServer`)*
   - `src/CmdDistrict/Common/Session.cs` *(holds current user/role)*
   - `src/CmdDistrict/Infrastructure/Stores.Ef/` *(EF-backed stores: `*StoreEf.cs`)*

5) **Plan.md risk**  
   Add risk: DB connectivity & TLS trust. Mitigation: local dev may use `Encrypt=True;TrustServerCertificate=True` in the connection string; ensure the container is running on `localhost:1433`.

## Guardrails
- Touch **only** the files listed above.  
- **Do not** modify any files under `src/` or any legacy skill files.  
- If a diff touches other paths, **abort** and print them.

## Prompt template
```
Skill: 01-spec-upgrade-efcore-linq-start
Goal: Update ONLY docs/specs to (a) EF Core + SQL Server (LINQ to Entities), (b) bool-only store methods with internal state set on success, and (c) GlobalMenuHolder.Start() rename.
Files allowed: docs/specs/{spec.md,plan.md,implementation.md,models.md}, docs/skills/README.md
Rules:
- No code edits. No legacy skills edits.
- Make surgical text changes only; keep structure and tone.
- Abort if other paths are touched.
Deliverable: updated docs with a short changelog.
```
