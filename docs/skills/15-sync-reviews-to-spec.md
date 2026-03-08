# Skill: 15-sync-reviews-to-spec (EF Core LINQ)

**Goal:** Implement Reviews (submit/list/average) via EF Core LINQ stores.

## Files allowed to change
- src/CmdDistrict/Infrastructure/Stores.Ef/ReviewStoreEf.cs
- src/CmdDistrict/Models/Entities/Review.cs

## Prompt template
```
Skill: 15-sync-reviews-to-spec (EF Core LINQ)
Goal: Implement Review behaviors to EF Core LINQ via Stores.Ef (bool-only).
Files allowed: Stores.Ef/ReviewStoreEf.cs, Entities/Review.cs
Rules:
- Minimal diffs; abort if other paths touched.
Deliverable: diffs + changelog.
```
