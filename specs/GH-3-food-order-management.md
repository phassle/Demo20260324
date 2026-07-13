# YT-1235: Food Order Management

## User story
As a staff member
I want to create, update, and cancel food orders
so that I can manage the full order lifecycle from placement through delivery.

## BDD Scenarios

```gherkin
Feature: Food Order Management

  # --- Create order ---

  Scenario: Staff creates a new order with valid items
    Given the menu contains items with IDs [101, 102, 103] and known prices
    And I am logged in as staff
    When I send POST /api/v2/orders with customer name "Alice" and item IDs [101, 102]
    Then the response status is 201
    And the response body contains a new order ID
    And the order status is "Pending"
    And the total equals the server-calculated sum of item prices for [101, 102]

  Scenario: Staff creates an order with an invalid menu item ID
    Given the menu does not contain item with ID 999
    And I am logged in as staff
    When I send POST /api/v2/orders with customer name "Bob" and item IDs [999]
    Then the response status is 422
    And the response body contains an error referencing the invalid item ID

  Scenario: Staff creates an order with an empty item list
    Given I am logged in as staff
    When I send POST /api/v2/orders with customer name "Carol" and an empty item list
    Then the response status is 422
    And the response body contains a validation error

  Scenario: Staff creates an order without a customer name
    Given I am logged in as staff
    When I send POST /api/v2/orders with no customer name and item IDs [101]
    Then the response status is 422
    And the response body contains a validation error for customer name

  # --- Advance status ---

  Scenario: Staff advances a Pending order to Preparing
    Given there is an order with ID "ord-1" in status "Pending"
    And I am logged in as staff
    When I send PATCH /api/v2/orders/ord-1/status with body {"status": "Preparing"}
    Then the response status is 200
    And the order status is "Preparing"

  Scenario: Staff advances a Preparing order to Ready
    Given there is an order with ID "ord-2" in status "Preparing"
    And I am logged in as staff
    When I send PATCH /api/v2/orders/ord-2/status with body {"status": "Ready"}
    Then the response status is 200
    And the order status is "Ready"

  Scenario: Staff advances a Ready order to Delivered
    Given there is an order with ID "ord-3" in status "Ready"
    And I am logged in as staff
    When I send PATCH /api/v2/orders/ord-3/status with body {"status": "Delivered"}
    Then the response status is 200
    And the order status is "Delivered"

  Scenario: Staff attempts an illegal status transition
    Given there is an order with ID "ord-4" in status "Pending"
    And I am logged in as staff
    When I send PATCH /api/v2/orders/ord-4/status with body {"status": "Delivered"}
    Then the response status is 422
    And the response body describes the invalid transition

  Scenario: Staff cancels an order that is Pending
    Given there is an order with ID "ord-5" in status "Pending"
    And I am logged in as staff
    When I send PATCH /api/v2/orders/ord-5/status with body {"status": "Cancelled"}
    Then the response status is 200
    And the order status is "Cancelled"

  Scenario: Staff attempts to cancel a Delivered order
    Given there is an order with ID "ord-6" in status "Delivered"
    And I am logged in as staff
    When I send PATCH /api/v2/orders/ord-6/status with body {"status": "Cancelled"}
    Then the response status is 422
    And the response body indicates the order cannot be cancelled

  Scenario: PATCH on a non-existent order
    Given no order with ID "ord-999" exists
    And I am logged in as staff
    When I send PATCH /api/v2/orders/ord-999/status with body {"status": "Preparing"}
    Then the response status is 404

  # --- Frontend: order list with status buttons ---

  Scenario: Status transition button advances order on click
    Given the order list page is open
    And an order row shows status "Pending"
    When the staff clicks the "Mark Preparing" button on that row
    Then the row status updates to "Preparing"
    And the available action button changes to "Mark Ready"

  Scenario: Delivered and Cancelled orders show no transition buttons
    Given the order list page is open
    And an order row shows status "Delivered"
    Then no status transition button is visible for that row

  # --- Frontend: new order form ---

  Scenario: Staff submits new-order form with items selected
    Given the new-order form is open
    And the menu is loaded with at least one item
    When the staff enters customer name "Dave" and selects two menu items
    And clicks Submit
    Then a POST request is sent to /api/v2/orders
    And the new order appears in the order list with status "Pending"
    And the displayed total matches the server-returned total

  Scenario: New-order form blocks submission when no items are selected
    Given the new-order form is open
    When the staff enters a customer name but selects no items
    Then the Submit button is disabled or shows a validation message

  # --- Server-side total calculation ---

  Scenario: Total is calculated server-side from menu item prices
    Given menu item 101 costs 9.50 and item 102 costs 4.00
    And I am logged in as staff
    When I send POST /api/v2/orders with customer name "Eve" and item IDs [101, 102]
    Then the response body contains total 13.50
    And the client-provided price (if any) is ignored
```

## Constraints
- Allowed status transitions: Pending → Preparing → Ready → Delivered; any state → Cancelled (except Delivered)
- Total is always calculated server-side; client total field is ignored
- Affected files: `frontend/src/pages/FoodOrderPage.tsx`, `backend/src/Controllers/FoodOrdersController.cs`, `backend/src/Services/FoodOrderService.cs`
- Stop: do not touch `backend/src/Middleware/AuthMiddleware.cs`
- Stop: do not modify `integrations/compliance-engine/`

## Verification
- `cd backend && dotnet test` — all green
- `cd backend && dotnet build` — no warnings
- `cd frontend && npm run lint` — no errors
