# Skill: 99-cleanup-remove-appstate

**Goal:** After all EF Core LINQ slices compile & pass manual tests, remove `AppState` and dangling references per updated specs.

## Files allowed to change
- src/CmdDistrict/Common/AppState.cs  *(delete)*
- any files still referencing `AppState.`  *(replace with store-backed logic already implemented)*
- docs/specs/implementation.md  *(note removal)*

## Guardrails
- Abort if `AppState.` references remain after planned edits — list file/line hits.
- Do not change menu/entity contracts.

## Prompt template
```
Skill: 99-cleanup-remove-appstate
Goal: Remove AppState safely after full EF Core LINQ migration; clean up references and docs note.
Files allowed: Common/AppState.cs (delete), any files with AppState refs, docs/specs/implementation.md (note)
Rules:
- Abort and list locations if any AppState refs remain unresolved.
Deliverable: diffs + changelog.
```
