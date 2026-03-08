# Skill: 11-sync-user-auth-to-spec-bool-only (EF Core LINQ)

**Goal:** Migrate **User** Register/Login/GetRole/Logout to EF Core LINQ per specs:
- Store methods are **bool-only** and **set Session** on success.
- Entities keep signatures; Menus unchanged.
- No changes outside the **User** slice.

## Files allowed to change
- src/CmdDistrict/Infrastructure/Stores.Ef/UserStoreEf.cs   *(create)*
- src/CmdDistrict/Models/Entities/User.cs                   *(update method bodies only)*

## Required behavior
- `UserStoreEf.Login(email, password) : bool` → sets `Session(userId, role)` on success.
- `UserStoreEf.Sign(name, email, password, role) : bool` → sets `Session(newUserId, role)` on success.
- `UserStoreEf.ResolveRoleFor(userId) : bool` → sets `Session(userId, role)` on success.
- `User.Login(...) : bool` → calls store.
- `User.Sign(...) : bool` → calls store.
- `User.GetRole(userId) : string` → try resolve, then return `Session.CurrentRole ?? ""`.
- `User.Logout() : bool` → `Session.Clear(); return true;`

## Guardrails
- Do not modify Menus or other entities.
- No ORMs beyond EF Core; use LINQ to Entities (DbSet<T> queries) and `SaveChanges()`.
- Do not rename/move files.
- Do not delete `AppState`.

## Prompt template
```
Skill: 11-sync-user-auth-to-spec-bool-only (EF Core LINQ)
Goal: Implement ONLY User slice using EF Core LINQ Stores (bool-only; Session set on success).
Files allowed: Stores.Ef/UserStoreEf.cs (create), Models/Entities/User.cs (update bodies only)
Rules:
- No other file changes; abort if other paths touched.
- Queries must be LINQ on DbSet<T> (UseSqlServer-backed).
Deliverable: code diffs + short changelog.
```
