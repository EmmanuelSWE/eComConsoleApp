## Skill : Repair-signin
**Goal:** **suregical update of :** Checkout function implementation in src/CmdDistrict/Models/Menu/MainMenu.cs

## Files allowed to append (Update that function only)
src/CmdDistrict/Models/Menu/MainMenu.cs


## Guardrails
- abort if any other function implementation is cahnged and revert to original.
- abort if return type changed revert to original.
- do not change function signature abort if signature changed.
- do not change coding format


## Must do:
- **strict** update implementation to make use case description of `User.Signin` possible
- **strict** reference documentations porvided as source for update decision.

## expected changes
- input Prompt for Address
- capture and setting address after creation of user
- no bugs
- logic fits new signin useCase.




## Help 
- check docs/specs/ spec.md to see the overview of project to get understanding of flow.
- Check the docs/specs/implementation.md to make sure logic of code follows 


## Prompt template
```
Skill:  Repair-signin
Goal: **suregical update of :** Checkout function implementation in src/CmdDistrict/Models/Menu/MainMenu.cs
Files allowed: src/CmdDistrict/Models/Menu/MainMenu.cs
Rules:
- abort if any other function implementation is cahnged and revert to original.
- abort if return type changed revert to original.
- do not change function signature abort if signature changed.
- do not change coding format
```
