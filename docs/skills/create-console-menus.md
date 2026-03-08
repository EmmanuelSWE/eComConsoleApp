# Skill: create-console-menus

**description:** Scaffold **Menu**, **MainMenu**, **CustomerMenu**, **AdminMenu**, and **GlobalMenuHolder** with **ContextUserId** and **role-based routing**. All actions call the LINQ implementations from Skill 2.

## When to use
When you need the **console UI** or new menu pages/actions.

## Conventions
- Namespace: `cmdDistrict.Models`
- Base Menu: `Key`, `Title`, `ContextUserId`, `Show/PrintOptions/HandleSelection`
- Router: `GlobalMenuHolder` holds `CurrentUserId`, `CurrentRole`, and `SwitchToRole(userId)`
- All Customer/Admin actions must pass `ContextUserId`
- Friendly prompts; re-prompt on invalid input


## Files to create
```
src/CmdDistrict/Models/Menu/Menu.cs
src/CmdDistrict/Models/Menu/MainMenu.cs
src/CmdDistrict/Models/Menu/CustomerMenu.cs
src/CmdDistrict/Models/Menu/AdminMenu.cs
src/CmdDistrict/Models/Menu/GlobalMenuHolder.cs
src/CmdDistrict/Program.cs
```

## Steps
1) Implement **Menu** base: `Show()` sets context → `PrintOptions()` → read → `HandleSelection()`.
2) Implement **MainMenu**: Register, Login, Browse (guest), Exit → on auth success call `GlobalMenuHolder.SwitchToRole(userId)`.
3) Implement **CustomerMenu**: Browse, Add to Cart, View Cart, Checkout (insufficient funds → prompt to deposit), View Orders, Add Review, Deposit, Logout.
4) Implement **AdminMenu**: Add/Update/Delete Product, Adjust Inventory, View All Orders, Update Status, Generate Report, Logout.
5) Implement **GlobalMenuHolder**: register menus, hold `CurrentUserId/Role`, route by `User.GetRole(userId)`.
6) **Program.cs**: bootstrap and run loop.

## How to use this skill in VS Code
1) Open `docs/skills/create-console-menus.md`.
2) Copy the **Prompt template** below into Copilot Chat/Claude.
3) Ask the assistant to generate/overwrite files under `src/CmdDistrict/Models/Menu/` and `src/CmdDistrict/Program.cs`.
4) Build with `dotnet run`; navigate menus and validate behaviors in §8.

## Prompt template
```text
Skill: create-console-menus
Goal: Scaffold console menus wired to LINQ entity methods (local-only).
Create/Update:
- Menu.cs, MainMenu.cs, CustomerMenu.cs, AdminMenu.cs, GlobalMenuHolder.cs under src/CmdDistrict/Models/Menu
- Program.cs entry point under src/CmdDistrict

Rules:
- All role-specific actions must use ContextUserId
- Main routes by role via GlobalMenuHolder.SwitchToRole(UserId)
- Re-prompt on invalid input; friendly messages

Link: ../specs/implementation.md#8-menus--behavior--routing-local-step-by-step-with-failure-cases
```
