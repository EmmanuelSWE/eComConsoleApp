# Skill: 01-spec-upgrade-sql-bool-start

**Goal:** Update specs (docs only) to adopt **SQL Server (Docker) via ADO.NET**, **bool-only store methods**, and the router name **Start()** (not Bootstrap).  
**Do not** change any code or existing legacy skill files.

## Files allowed to change (docs only)
- docs/specs/spec.md
- docs/specs/plan.md
- docs/specs/implementation.md
- docs/specs/models.md
- docs/skills/README.md  *(index only; do not edit legacy skills)*

## Required doc changes (surgical)
1) **Persistence**  
   Replace “in-memory / AppState / LINQ” with **“SQL Server (Docker) via ADO.NET (Microsoft.Data.SqlClient)”** and clarify: **Entities call Stores.Sql; Menus call Entities; no services/ORMs.**

2) **Returns & state**  
   All **Store methods return `bool`**; on success they **set state internally** (e.g., `Session.CurrentUserId` + `Session.CurrentRole`). Entities keep existing signatures and read values from the state when needed.

3) **Router name**  
   Change **`GlobalMenuHolder.Bootstrap()` → `GlobalMenuHolder.Start()`** in every snippet and reference.

4) **Folder diagram (implementation.md)**  
   Add `src/CmdDistrict/DataAccess/Db.cs`, `src/CmdDistrict/Common/Session.cs`, and `src/CmdDistrict/Infrastructure/Stores.Sql/`.

5) **Plan.md risk**  
   Add risk: DB connectivity & TLS trust; mitigation: local dev may use `Encrypt=True;TrustServerCertificate=True`.

## Guardrails
- Touch **only** the files above.  
- **Do not** modify any files under `src/` or any legacy skill files.  
- If a diff touches other paths, **abort** and print them.

## Prompt template
```
Skill: 01-spec-upgrade-sql-bool-start
Goal: Update ONLY docs/specs to (a) SQL Server via ADO.NET, (b) bool-only store methods with internal state set on success, and (c) GlobalMenuHolder.Start() rename.
Files allowed: docs/specs/{spec.md,plan.md,implementation.md,models.md}, docs/skills/README.md
Rules:
- No code edits. No legacy skills edits.
- Make surgical text changes only; keep structure and tone.
- Abort if other paths are touched.
Deliverable: updated docs with a short changelog at end of response.
```
