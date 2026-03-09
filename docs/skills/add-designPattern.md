# Skill:add-desginPattern

**Goal:** **suregical update Menus and adding**of a command desing pattern, whihc works with the items.

**Success Metric** 
- all menu fucntions point to command which executes everything
- all logic still works
- no bugs

**Failure Metric**
- atleast one success metric not met

## Files allowed to append 
- src/cmdDistrict/Models/Menu/*cs


## Guardrails
- abort if overall logic does not reach the same result
- all menu calls must be through commands.

## steps
- create a new DesiganPattern folder.
- create a command subfolder and in there a command interface which has an excute and dscription function. (name space cmdDistrict.designPattern)
- have commands for each menu option : **how to:**
go to each non abstract menu... read the file and see the commands to make and which actions they will wrap. (each command file stored in the desifn patter folder)
- prompting must be in the command as well.
- check if command flows the same and the menu option **true** swap with command **false** fix command and check it.
- evaluate changes and make sure all work correctly

## Help 
- check docs/specs/ implementation.md to see the logic flow
- check - src/cmdDistrict/Models/Menu/*cs to see how the flow of these menu items.

## Prompt template
```
Skill: augment-debug
Goal: **suregical update Menus and adding**of a command desing pattern, whihc works with the items.
Files allowed: src/cmdDistrict/Models/Menu/*cs
Rules:
- abort if overall logic does not reach the same result
- all menu calls must be through commands.
```
