# Skill: wire-up-application

**description:** Combine **models + LINQ actions + menus** into a runnable console app. Optionally **seed** demo data. Validate basic flows end‑to‑end using manual tests.

## When to use
First-time integration, after adding entities/menus/actions, or before a demo.

## Conventions
- No services, no DB — everything lives in `AppState` lists
- Optional seeding in `Program` or a `Seed` helper

## Files to create/update
```
src/CmdDistrict/Program.cs
src/CmdDistrict/Common/AppState.cs         // lists initialized
src/CmdDistrict/Common/Seed.cs (optional)  // demo data
```

## Steps
1) Initialize `AppState` lists (`new List<T>()`).
2) (Optional) Seed: create one Admin, one Customer, some Products.
3) Bootstrap menus: `GlobalMenuHolder.Bootstrap()`; then `GlobalMenuHolder.Run()`.
4) Validate flows: register/login, browse, cart, checkout (with & without funds), admin product mgmt, status updates, reporting.
5) Verify failure paths: wrong password, duplicate email, invalid rating, negative stock, invalid transitions.
6) Document notes under `docs/specs/implementation.md` Manual Test Cases.

## How to use this skill in VS Code
1) Open `docs/skills/wire-up-application.md`.
2) Copy the **Prompt template** below into Copilot Chat/Claude.
3) Ask the assistant to ensure `AppState` initialization and generate an optional `Seed.cs`.
4) Build & run the app; walk through test cases in `docs/specs/implementation.md#10-manual-test-cases`.

## Prompt template
```text
Skill: wire-up-application
Goal: Make the console app runnable end-to-end (local-only, LINQ).
Tasks:
- Ensure AppState lists are initialized
- (Optional) Create Seed.cs to add a few demo users/products
- Bootstrap GlobalMenuHolder and run
- Validate flows & failure paths per the manual tests

Links:
- Actions & flows spec: ../specs/implementation.md#7-model-actions--implementation-descriptions-local-step-by-step-with-failure-cases
- Menus & routing spec: ../specs/implementation.md#8-menus--behavior--routing-local-step-by-step-with-failure-cases
- Manual tests: ../specs/implementation.md#10-manual-test-cases
```
