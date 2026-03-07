# Cmd District Skills — Index & VS Code Usage

This folder contains **modular skill specs** you can copy into Copilot Chat / Claude / or your internal prompting docs to scaffold and evolve the **Cmd District** console app.

> **Everything is local & LINQ-only** — no services, no database.

## Files

- [`create-entity-models.md`](./create-entity-models.md)
- [`implement-model-actions-linq.md`](./implement-model-actions-linq.md)
- [`create-console-menus.md`](./create-console-menus.md)
- [`wire-up-application.md`](./wire-up-application.md)

## Where the main specs live
- **Specs folder:** `docs/specs/`
  - [`spec.md`](../specs/spec.md)
  - [`plan.md`](../specs/plan.md)
  - [`implementation.md`](../specs/implementation.md) — see §7 and §8 for detailed flows

---

## How to use these skills in VS Code (step-by-step)

1) **Open the workspace**  
   - `File → Open Folder…` and select your repo root.

2) **Recommended extensions**  
   - *C# Dev Kit* (ms-dotnettools.csdevkit)  
   - *.NET Install Tool* (ms-dotnettools.vscode-dotnet-runtime)  
   - *EditorConfig for VS Code* (EditorConfig.EditorConfig) — if you add an `.editorconfig`

3) **Navigate to a skill**  
   - Open `docs/skills/*.md` and copy the **Prompt template** from the relevant skill.

4) **Run the skill with Copilot Chat / Claude**  
   - Open the chat panel.  
   - Paste the Prompt template and **fill in your entity names / paths** where indicated.  
   - Ask it to **generate files directly into** your project paths (the prompts specify exact folders and file names).

5) **Review and stage files**  
   - Use the *Source Control* panel (⌃` on macOS / Ctrl+Shift+G on Windows) to review diffs.  
   - Ensure file names **exactly match** the paths in each skill.

6) **Build & Run**  
   ```bash
   cd src/CmdDistrict
   dotnet run
   ```

7) **Iterate**  
   - If something needs to change, re-open the corresponding skill file and re-run the refined prompt.  
   - Keep your core contracts aligned with `docs/specs/implementation.md` §7 (Model Actions) and §8 (Menus).

---

## Quick links to spec sections
- **Model Actions:** [`docs/specs/implementation.md#7-model-actions--implementation-descriptions-local-step-by-step-with-failure-cases`](../specs/implementation.md#7-model-actions--implementation-descriptions-local-step-by-step-with-failure-cases)  
- **Menus & Routing:** [`docs/specs/implementation.md#8-menus--behavior--routing-local-step-by-step-with-failure-cases`](../specs/implementation.md#8-menus--behavior--routing-local-step-by-step-with-failure-cases)
