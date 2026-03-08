## Skill : Repaire Entity
**Goal:** **suregical update of :** Checkout function decsription in docs/specs/implementation.md  

## Files allowed to append (Update that function only)
src/CmdDistrict/Models/Menu/MainMenu.cs


## Guardrails
- abort if any other function description is cahnged and revert to original.
- abort if return type changed revert to original.
- **strict** Change whats written in format


## Format:
### what to change
#### 7.1.2 Signin(name, email, password, role) ⇒ bool
1. user inputs signin
2. Prompted for name : inputs
3. Prompted for email : inputs
4. Prompted for password : inputs 


5. validation check if email name and password input (add must not be empty)
6. Store query : use UserStoreEf to check email uniqueness. 
7. create user instance (email, name, password (hashed)) and wallet instance for user.
8. save user instance and wallet via Store (SQL INSERT).
9. Store returns `bool`; on success, sets `Session.CurrentUserId` and `Session.CurrentRole` internally. 

### Change to : 
1. user inputs signin
2. Prompted for name : inputs
3. Prompted for email : inputs
4. Prompted for password : inputs
5. Prompted for Role : inputs {1 for Customer 2 for Adminstrator}
6. compare if Role is 2 : make admin account 
7.   Prompted for DefaultShippingAddress : inputs
6. validation check if email name and password input (add must not be empty) if Role `==` Customer `&&` DefaultShippingAddress must not be null
9. Store query : use UserStoreEf to check email uniqueness. 
10. create user instance (email, name, password (hashed)) and wallet instance for user.
11. if Role `==` Customer set user.DefaultShippingAddress to DefaultShippingAddress
12. save user instance and wallet via Store (SQL INSERT).
13. Store returns `bool`; on success, sets `Session.CurrentUserId` and `Session.CurrentRole` internally.

## Help 
- check docs/specs/ spec.md to see the overview of project to get understanding of flow.

## Prompt template
```
Skill:  repair entity
Goal: **suregical update of :** Checkout function decsription in docs/specs/implementation.md
Files allowed: docs/specs/implementation.md 
Rules:
- abort if any other function description is cahnged and revert to original.
- abort if return type changed revert to original.
- **strict** Change whats written in format
```
