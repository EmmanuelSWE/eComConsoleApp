# Skill: augment-debug

**Goal:** **suregical update of :** Debug messsages to output the id of created Entities and message of completion or failure.

**Success Metric** 
- no business logic changed
- console debug messages resemble the new foramt exactly

**Failure Metric**
- atleast one success metric not met

## Files allowed to append (add only NO removal of code allowed)
- src/cmdDistrict/infrastructure/Stores.Ef/*.cs
- src/CmdDistrict/DataAccess/*.cs

## Guardrails
- abort if any code removed and revert to original.
- no removal of coding actual business logic,just altering console messages

## Console.WriteLine-Format 
### original format
`Console.WriteLine( {message} : result) `
psuedo code example : `Console.WriteLine($"connect success or failure: {connection_result}");`
- 1. initial db connection check message :
    1. writeLine : connection type :
    2. writeLine : connection success or failure :

- 2. Linq query : 
    1. writeLine : function name is : 
    2. writeLine : Arguments are : 
    3. writeLine : expected return : 
    4. writeLine : actualReturn :
    5. WriteLine : Outcome : failed/passed/incountered an error 


### new format
`Console.WriteLine( {message} : result) `
psuedo code example : `Console.WriteLine($"connect success or failure: {connection_result}");`
- 1. initial db connection check message :
    1. writeLine : connection type :
    2. writeLine : connection success or failure :

- 2. Linq query : 
    1. WriteLine : Outcome : failed/passed/incountered an error 
    2. WriteLine : EntityMade : objecName : idValue

## Help 
- check docs/specs/ implementation.md to see the expected return of functions
- check ./debug-check to see how the previous implemented the skill

## Prompt template
```
Skill: augment-debug
Goal:  **suregical update of :** Debug messsages to output the id of created Entities and message of completion or failure.
Files allowed: src/cmdDistrict/infrastructure/Stores.Ef/*.cs (only append debug messages), asrc/CmdDistrict/DataAccess/*.cs(only append debug messages)
Rules:
- abort if any code removed and revert to original.
- no removal of coding actual business logic,just altering console messages
- Follow Console.WriteLine-Format and make them the new format
```
