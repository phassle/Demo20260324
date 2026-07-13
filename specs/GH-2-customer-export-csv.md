# YT-1234: Customer Export CSV

## User story
As an administrator
I want to export customers as a CSV file
so that I can share customer data with external compliance tools.

## Acceptance criteria (BDD)

```gherkin
Scenario: Admin exports customers as CSV
  Given there are 50 customers in the database
  And I am logged in as an admin
  When I send GET /api/v2/customers/export
  Then the response status is 200
  And the Content-Type is "text/csv"
  And the CSV contains 50 rows plus a header

Scenario: Non-admin gets 403
  Given I am logged in as a regular user
  When I send GET /api/v2/customers/export
  Then the response status is 403

Scenario: Export with more than 10,000 customers
  Given there are 15,000 customers in the database
  And I am logged in as an admin
  When I send GET /api/v2/customers/export
  Then the response status is 200
  And the CSV contains 10,000 rows plus a header
  And the response includes header "X-Truncated: true"

Scenario: Empty database returns header only
  Given there are 0 customers in the database
  And I am logged in as an admin
  When I send GET /api/v2/customers/export
  Then the response status is 200
  And the CSV contains 0 rows plus a header
```

## Constraints
- Max 10,000 rows per export (truncate with header)
- Backend: `backend/src/Controllers/`, `backend/src/Services/`
- Frontend: `frontend/src/pages/CustomersPage.tsx` (add Export button)
- Stop: Don't touch auth middleware (`backend/src/Middleware/`)

## Verification
- `cd backend && dotnet test` — all green
- `cd backend && dotnet build` — no warnings
- `cd frontend && npm run lint` — no errors
