# Cmd district — Specification (spec.md)

## 0. Description
This document defines the specification for the Online Shopping Backend System, implemented
as a C# Console Application. The project simulates the backend logic of a real e commerce
platform where customers can browse products, manage shopping carts, place orders, and process
payments, while administrators manage products, inventory, and orders. The system is designed to
demonstrate strong use of object oriented programming, backend logic design, LINQ-to-Entities queries via EF Core,
error handling, and clean architecture within a console based environment. Two submissions are
required for this project. 

## 1. Overview
The application represents the backend operations of an online shopping platform. Users interact
with the system through a structured console interface that allows them to perform operations such
as browsing products, managing carts, placing orders, and generating reports. The system
supports two main roles: Customer – shops for products and places orders Administrator 
manages the platform inventory and orders

## 2. Goals
- Simple, robust, beginner-friendly.
- Clear menu loop with input validation.
- Clean separation of concerns: Menus → Entities → Stores.Ef.

## 3. Non-Goals
- No GUI.

## 4. Functional Requirements
- **FR1**:  User registration and login
- **FR2**: Customer and Administrator role separation.
- **FR3**: Product catalog management
- **FR4**:  Shopping cart functionality.
- **FR5**: Order placement and tracking.
- **FR6**: Payment simulation through wallet balance.
- **FR6**:  Inventory management and stock tracking.
- **FR6**: Administrative reporting and analytics.
- **FR6**: Product review and rating system.
- **FR6**: Exception handling and input validation.
## 5. Non-Functional Requirements
- **NFR1**: Code should compile on .NET 8.
- **NFR2**: Code style: strict type checking
- **NFR3**: Input validation must prevent crashes from invalid inputs on menu

## 6. UX / Flow
Conosle Wil Show :
- Main Menu:
                     === Main Menu ===

● Register              ○ Login               ○ Exit
   ________


- Customer Menu:
                   === Customer Menu ===

                 Hello {Customer_Name}

● Browse Products       ○ Search Products       ○ Add Product to Cart     ○ View Cart
   _________________

○ Update Cart           ○ Checkout              ○ View Wallet Balance     ○ Add Wallet Funds

○ View Order History    ○ Track Orders          ○ Review Products         ○ Logout


- Administrator Menu:
                === Administrator Menu ===

● Add Product           ○ Update Product        ○ Delete Product          ○ Restock Product
   ____________

○ View Products         ○ View Orders           ○ Update Order Status     ○ View Low Stock Products

○ Generate Sales Reports     ○ Logout



Move Using arrow keys and press enter to 

## 7. Architecture / Design
link to that : ./models.md


## 8. Error Handling
- invalid input → retry prompt.
- Unexpected exceptions → show a friendly message and remain in menu loop.

## 9. Testing Strategy (Manual for now)
- Menu choice validation 
- input choice validations
- Each operation with expected values.
- Multiple sequential operations before exit.

## 10. Acceptance Criteria
- **AC1**: everything functions as it should
- **AC2**: App never crashes on bad input or error.
- **AC3**: User can perform multiple operations until exit.
- **AC4**: model opertions functions as they should. 


## 11. Links
- ./plan.md
- ./implementation.md