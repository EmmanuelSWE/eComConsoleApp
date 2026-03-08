# Skill: debug-check

**Goal:** Have a proper console documentation for every store class and and initial console message to see if conected to sql server.

## Files allowed to append (add only NO removal of code allowed)
- src/cmdDistrict/infrastructure/Stores.Ef/*.cs
- src/CmdDistrict/DataAccess/*.cs

## Guardrails
- abort if any code removed and revert to original.
- no removal of coding just adding

## Console.WriteLine-Format 
### how to write 
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

## Help 
- check docs/specs/ implementation.md to see the expected return of functions

## Prompt template
```
Skill: debug-check
Goal: Have a proper console documentation for every store class and and initial console message to see if conected to sql server.
Files allowed: src/cmdDistrict/infrastructure/Stores.Ef/*.cs (only add debug messages), asrc/CmdDistrict/DataAccess/*.cs(only add debug messages)
Rules:
- abort if any code removed and revert to original.
- no removal of coding just adding
- Follow Console.WriteLine-Format
```
