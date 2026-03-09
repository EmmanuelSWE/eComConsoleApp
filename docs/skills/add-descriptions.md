# Skill: add-descriptions

**Goal:** Have an output menu option to describe all functionality of the page.

## files to change 
 - src/cmdDistrict/Models/Menu/ * cs

## success 
- all menus have a list of commands
- all menues must have a description function

## failure 
- manu class logic destroyed
- menu class is not compilable

## steps 
- in menu (base)class have a list of Command interface type from the src/CmdDistrict/DesgnPattern/command folder 
- add a describe function in base class menu which iterates through lists and neatley prints all the ListItems descriptions since they are commands
- now each child meanu item will have its own list of items 
- remove the creation of commands in the switch cases of menus just execute the correct list command
- add a new option at the end of each menu called describe menu... which invokes the describe function and describe all items in the specific menu's list.

## Guardrails
- Do not remove anything else
- use same coding standards
-  Do not change any other file


## Help 
- check docs/specs/ implementation.md to see the expected return of functions

## Prompt template
```
Skill: add-descriptions
Goal: Have an output menu option to describe all functionality of the page.
Files allowed: - src/cmdDistrict/Models/Menu/ * cs
Rules:
- Do not remove anything else
- use same coding standards
-  Do not change any other file
```
