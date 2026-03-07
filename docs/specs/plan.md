# Cmd district — Plan (plan.md)

## 1. Scope & Milestones : per milestone validate with me.

- **M1: Core Features**
  - Menu loop + input validation
  - all Actions of models in ./models.md

- **M3: Error Handling & Polish**
  - safe updates and creations
  - Friendly messages
  - Comments & cleanup

- **M4: Test Pass**
  - Manual test cases from spec
  - Verify acceptance criteria

## 2. Risks & Mitigations
- **Risk**: enity may not be present→ **Mitigation**: Helper `FindEntity()` guards all enity operations before they are executed.
- **Risk**: user unauthorized → **Mitigation**: helper `ValidateUser` guards from unathorized users and send back to login menu if unathorized with redirecion message.


## 3. Definition of Done
- All acceptance criteria in `spec.md` met.
- Code compiles on .NET 8 and runs `dotnet run` cleanly.
- Docs updated (this plan + implementation notes).

## 4. Links
- ./spec.md
- ./implementation.md