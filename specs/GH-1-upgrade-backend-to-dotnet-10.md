## Plan: Upgrade Backend to .NET 10

Upgrade backend projects from .NET 8 to `.NET 10` by changing the target frameworks to `net10.0`, pinning the SDK with `global.json`, and validating restore, build, test, and runtime behavior with explicit smoke checks.

**Target Version**
1. Target framework: `net10.0`
2. SDK policy: pin the installed `.NET 10` SDK in `global.json` at repo root
3. Preview SDKs are out of scope

**Steps**
1. Confirm the installed .NET 10 SDK from repo root.
2. Run `dotnet --list-sdks` and `dotnet --version`.
3. If no `.NET 10` SDK is installed, stop the task and install the latest stable `.NET 10` SDK before editing project files.
4. Create or update `global.json` at repo root to pin the chosen `.NET 10` SDK version. This step is mandatory before restore/build/test.
5. Change `<TargetFramework>` in `backend/src/PortCom.Demo.csproj` from `net8.0` to `net10.0`.
6. Change `<TargetFramework>` in `backend/tests/PortCom.Demo.Tests.csproj` from `net8.0` to `net10.0`.
7. Run `dotnet restore backend/PortCom.Demo.sln` from repo root and inspect warnings/errors for package compatibility.
8. If restore fails because of package compatibility, update only the packages that block restore/build/test. Do not perform unrelated dependency upgrades.
9. Run `dotnet build backend/PortCom.Demo.sln` from repo root.
10. Run `dotnet test backend/tests/PortCom.Demo.Tests.csproj` from repo root.
11. Start the API with `dotnet run --project backend/src/PortCom.Demo.csproj`.
12. Execute the smoke tests listed below against the running API.
13. Update the .NET version mention in `README.md` from .NET 8 to .NET 10.
14. If the repository contains CI or build pipeline files at execution time, update their SDK version/pinning in the same change set. If none exist, record that explicitly in the PR or issue.

**Dependency Update Rule**
1. Update packages only when they block restore, build, or tests under `net10.0`.
2. Prefer the smallest viable version change.
3. Avoid broad package modernization in this task.
4. Document every package version change in the PR or issue.

**Relevant files**
- `backend/src/PortCom.Demo.csproj` — update `TargetFramework`; check package references for compatibility.
- `backend/tests/PortCom.Demo.Tests.csproj` — update `TargetFramework`; validate test package compatibility.
- `global.json` — pin the exact .NET 10 SDK version used for the upgrade.
- `backend/src/Program.cs` — verify middleware and controller mapping still behave correctly after the upgrade.
- `README.md` — update the stated .NET version.

**Smoke Tests**
1. `curl -i http://localhost:5000/api/v2/customers`
Expected: `200 OK` and a JSON array response.
2. `curl -i http://localhost:5000/api/v2/customers/1`
Expected: `200 OK` or `404 Not Found`, but not `500`.
3. `curl -i http://localhost:5000/api/v2/menuitems`
Expected: `200 OK` and a JSON array response.
4. `curl -i http://localhost:5000/api/v2/foodorders`
Expected: `200 OK` and a JSON array response.
5. `curl -i http://localhost:5000/api/v2/payments`
Expected: `200 OK` and a JSON array response.
6. `curl -i http://localhost:5000/api/v2/customerreviews`
Expected: `200 OK` and a JSON array response.
7. `curl -i http://localhost:5000/api/v2/customers/export`
Expected: authorization-related response only, such as `403 Forbidden`; not `500`.

**Verification**
1. `global.json` exists at repo root and pins a `.NET 10` SDK.
2. `dotnet restore backend/PortCom.Demo.sln` succeeds.
3. `dotnet build backend/PortCom.Demo.sln` succeeds.
4. `dotnet test backend/tests/PortCom.Demo.Tests.csproj` succeeds.
5. The API starts under `.NET 10` without startup exceptions.
6. All smoke tests complete without `5xx` responses.
7. `README.md` reflects `.NET 10`.

**Scope**
- Included: backend projects, SDK pinning, README update, and any existing CI/build SDK pinning discovered during execution.
- Excluded: frontend feature work and unrelated dependency upgrades.
