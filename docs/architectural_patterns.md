# Architectural Patterns

## Backend: Controller → Service → Model

Every domain entity follows the same 3-layer pattern:

```
Controller (REST)  →  IService (interface)  →  Service (implementation)  →  Model
```

**Adding a new entity** — replicate this pattern:
1. Model in `backend/src/Models/` — POCO with public properties
2. Interface in `backend/src/Services/I{Name}Service.cs` — async methods returning `Task<>`
3. Implementation in `backend/src/Services/{Name}Service.cs` — in-memory `List<T>` with seed data
4. Controller in `backend/src/Controllers/{Name}Controller.cs` — `[ApiController]`, route `api/v2/[controller]`
5. Register in `backend/src/Program.cs` — `AddScoped<IService, Service>()`

**Reference implementations:**
- Model: `backend/src/Models/Customer.cs:1-12`
- Interface: `backend/src/Services/ICustomerService.cs:1-11`
- Service: `backend/src/Services/CustomerService.cs:1-41`
- Controller: `backend/src/Controllers/CustomersController.cs:1-59`
- DI registration: `backend/src/Program.cs:10-14`

## Backend: In-Memory Data (Demo Only)

All services use `static readonly List<T>` with `Enumerable.Range()` seed data.
No database. No EF Core. This is intentional for the demo.
See `backend/src/Services/CustomerService.cs:11-22` for the pattern.

## Backend: Soft Delete

Entities with `IsDeleted` property must be filtered in all query methods.
- Correct: `backend/src/Services/CustomerService.cs:25` — `GetAllAsync` filters `IsDeleted`
- Known bug: `GetByIdAsync` does NOT filter — see `backend/src/Services/CustomerService.cs:28`

## Backend: Auth via Middleware

`backend/src/Middleware/AuthMiddleware.cs` — sets `HttpContext.Items["UserRole"]` and `["UserId"]`.
Demo hardcodes `"admin"`. Production would validate JWT from Azure AD.
Controllers check role via `HttpContext.Items["UserRole"]` — see `backend/src/Controllers/CustomersController.cs:40-42`.

**STOP RULE: Never modify AuthMiddleware without explicit approval.**

## Backend: CSV Export Pattern

Export endpoints return `text/csv` with `X-Truncated` header for large datasets.
Service returns `(IEnumerable<T>, bool Truncated)` tuple.
- Service: `backend/src/Services/CustomerService.cs:34-40`
- Controller: `backend/src/Controllers/CustomersController.cs:37-53`

## Frontend: Composable Pattern

Every API resource follows: `api module` → `composable` → `page component`.

```
services/api.ts (axios calls)  →  composables/use{Name}.ts  →  pages/{Name}Page.vue
```

**Adding a new resource to frontend:**
1. Add API methods in `frontend-vue/src/services/api.ts`
2. Create composable wrapping `useApiData` — see `frontend-vue/src/composables/useCustomers.ts:1-8`
3. Create page component using the composable

**Base composable:** `frontend-vue/src/composables/useApiData.ts:1-25`
- Generic `useApiData<T>` handles loading/error/abort
- All API calls go through single axios instance with `baseURL: '/api/v2'`

## Frontend: Routing

All routes in `frontend-vue/src/router.ts:14-24`. Each route maps to a page component.
Add new pages to this file + import the component.

## Frontend: Dev Proxy

Vite proxies `/api` → `http://localhost:5000` — see `frontend-vue/vite.config.ts:8-12`.
Backend and frontend run on separate ports (5000 and 3000).

## Integrations

`integrations/compliance-engine/` — standalone compliance validator, mock only.
Not referenced by main backend. Requires compliance team approval for changes.

## Specs

BDD spec files live in `specs/`. Named `{TICKET}-{slug}.md`.
Used by the `refine-story` agent to formalize requirements.
