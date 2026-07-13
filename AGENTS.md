# AGENTS.md — PortCom.Demo

Be extremely concise. Sacrifice grammar for concision.
At the end of each plan, list unresolved questions.

## What

Restaurant management platform — food ordering, customers, payments, reviews, menu management.

| Layer | Stack | Location |
|-------|-------|----------|
| Backend | C# / .NET 8, Serilog, xUnit + Moq | `backend/` |
| Frontend | Vue 3, TypeScript, Vite, Axios, vue-router | `frontend-vue/` |
| Integrations | Standalone C# modules (compliance engine) | `integrations/` |
| Specs | BDD spec files per ticket | `specs/` |

All backend data is **in-memory** (no database). Demo/workshop project.

## Commands

```bash
# Backend
cd backend && dotnet build
cd backend && dotnet run                    # → http://localhost:5000
cd backend && dotnet test                   # xUnit tests
cd backend && dotnet format                 # code style

# Frontend
cd frontend-vue && npm install
cd frontend-vue && npm run dev              # → http://localhost:3000
cd frontend-vue && npm run build            # production build
```

Frontend proxies `/api` → backend via Vite — see `frontend-vue/vite.config.ts:8-12`.

## Architecture

Full patterns with file:line references → [`docs/architectural_patterns.md`](docs/architectural_patterns.md)

**Backend:** Controller → IService → Service → Model. All services registered as scoped DI in `backend/src/Program.cs:10-14`. Route convention: `api/v2/[controller]`.

**Frontend:** `api.ts` → `composable` → `page`. Generic `useApiData<T>` composable handles loading/error/abort (`frontend-vue/src/composables/useApiData.ts:3`).

## API Endpoints

| Method | Route | Controller |
|--------|-------|------------|
| GET | `/api/v2/customers` | `CustomersController.cs:22` |
| GET | `/api/v2/customers/{id}` | `CustomersController.cs:29` |
| GET | `/api/v2/customers/export` | `CustomersController.cs:37` (admin-only) |
| GET | `/api/v2/foodorders` | `FoodOrdersController.cs:18` |
| GET | `/api/v2/menuitems` | `MenuItemsController.cs` |
| GET | `/api/v2/customerreviews` | `CustomerReviewsController.cs` |
| GET | `/api/v2/payments` | `PaymentsController.cs` |

## Stop Rules

1. **AuthMiddleware** (`backend/src/Middleware/AuthMiddleware.cs`) — NEVER modify without explicit approval. Shared across all endpoints. Production validates JWT from Azure AD.
2. **Compliance engine** (`integrations/compliance-engine/`) — requires compliance team approval.
3. **Database migrations** — `dotnet ef database update` is denied in permissions. No direct DB changes.

## Conventions

- Linters handle code style (`dotnet format` runs as PostToolUse hook on Write/Edit)
- New backend entities: Model → IService → Service → Controller → register in Program.cs
- New frontend resources: api.ts method → composable → page → add route in `router.ts`
- Tests live in `backend/tests/` — xUnit with Moq. Test file: `{Service}Tests.cs`
- Spec files in `specs/` — named `{TICKET}-{slug}.md`

## Hooks (`.claude/settings.json`)

| Event | Action |
|-------|--------|
| PostToolUse (Write/Edit) | `dotnet format --verify-no-changes` |
| Stop | `dotnet test && dotnet build` |

## Known Issues

- `CustomerService.GetByIdAsync` doesn't filter `IsDeleted` — returns soft-deleted customers (`backend/src/Services/CustomerService.cs:28`)
- Frontend CSV export call is commented out (`frontend-vue/src/services/api.ts:19-21`)

## Deep Dives

- [Architectural Patterns](docs/architectural_patterns.md) — full Controller→Service→Model pattern, composable pattern, auth, export, soft-delete details with file:line references
