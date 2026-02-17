# NPI System — Blazor WebAssembly + .NET 8 API

## Architecture

```
NPI.sln
├── src/
│   ├── NPI.Shared          ← DTOs, Enums (shared between client & server)
│   ├── NPI.Data            ← EF Core, Entities, Repository, Unit of Work, SQLite
│   ├── NPI.Server          ← ASP.NET Core Web API (controllers, AutoMapper)
│   └── NPI.Client          ← Blazor WASM (MVVM ViewModels, Razor components)
└── tests/
    └── NPI.Tests           ← xUnit + Moq + FluentAssertions
```

### Design Patterns Used

| Pattern | Where | Details |
|---------|-------|---------|
| **MVVM** | `NPI.Client/ViewModels/` | `ViewModelBase` with `INotifyPropertyChanged`, computed properties, commands |
| **Repository** | `NPI.Data/Repositories/` | Generic `IRepository<T>` + specialized `INpiRecordRepository` with LINQ |
| **Unit of Work** | `NPI.Data/UnitOfWork/` | `IUnitOfWork` wraps all repositories, manages transactions and `SaveChangesAsync()` |
| **LINQ** | Throughout Data layer | Predicate filters, eager loading, paging, aggregation, ordering |
| **DTO Mapping** | `NPI.Server/Mapping/` | AutoMapper profiles for Entity ↔ DTO conversion |

---

## Tech Stack

- **.NET 8** — API + Blazor WASM
- **Entity Framework Core 8** — ORM with SQLite provider
- **SQLite** — Lightweight file-based database (`npi.db`)
- **AutoMapper** — Entity ↔ DTO mapping
- **Blazor WebAssembly** — SPA client with MVVM pattern
- **xUnit + Moq + FluentAssertions** — Unit testing

---

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dot.net/download)

### Run the API

```bash
cd src/NPI.Server
dotnet run
```
→ API at `https://localhost:7200` — Swagger at `/swagger`

The database (`npi.db`) is auto-created and seeded on first run.

### Run the Blazor Client

```bash
cd src/NPI.Client
dotnet run
```
→ App at `https://localhost:7100`

### Run Tests

```bash
cd tests/NPI.Tests
dotnet test
```

---

## API Endpoints

### NPI Records
| Method | Route | Description |
|--------|-------|-------------|
| GET | `/api/npirecords` | List all (with search & status filter) |
| GET | `/api/npirecords/{id}` | Full record with all children |
| GET | `/api/npirecords/{id}/setup` | Setup tab data only |
| GET | `/api/npirecords/{id}/bom` | Labor & BOM tab data only |
| GET | `/api/npirecords/{id}/packaging` | Packaging tab data only |
| GET | `/api/npirecords/{id}/docs` | Documents tab data only |
| POST | `/api/npirecords` | Create new record |
| PUT | `/api/npirecords/{id}` | Update header fields |
| DELETE | `/api/npirecords/{id}` | Delete record |
| GET | `/api/npirecords/{id}/labor-total/{category}` | LINQ aggregate |

### Child Entities
| Method | Route | Description |
|--------|-------|-------------|
| GET | `.../setup-questions` | List setup questions |
| PUT | `.../setup-questions/{id}/toggle` | Toggle Y/N |
| GET | `.../pilot-requirements` | List pilot reqs |
| PUT | `.../pilot-requirements/{id}/toggle` | Toggle Y/N |
| GET | `.../planner-questions` | List planner questions |
| PUT | `.../planner-questions/{id}/toggle` | Toggle Y/N |
| GET/POST/PUT/DELETE | `.../packaging-components` | Full CRUD |
| GET/POST/DELETE | `.../notes` | Notes management |
| GET | `.../changelog` | Change log |

---

## MVVM Pattern

The Blazor WASM client implements full MVVM:

```
┌─────────────┐    binds to    ┌──────────────────┐    calls    ┌─────────────┐
│ .razor View │ ←──────────── │  ViewModel (VM)   │ ─────────→ │  INpiService │
│  (UI/HTML)  │ ──────────→   │  INotifyProperty  │            │  HttpClient  │
│             │    commands    │  Computed Props   │            │  → REST API  │
└─────────────┘               └──────────────────┘            └─────────────┘
```

**ViewModelBase** provides:
- `INotifyPropertyChanged` for automatic UI refresh
- `IsBusy` / `ErrorMessage` state
- `ExecuteAsync()` wrapper with error handling
- `SetProperty()` helper for change notification

**NpiDetailViewModel** provides:
- All tab data as computed LINQ properties
- Commands: `LoadAsync`, `ToggleSetupQuestionAsync`, `SaveRecordAsync`, `AddNoteAsync`, etc.
- `SwitchTab()`, `ToggleTheme()` state management

---

## Repository + UoW Pattern

```csharp
// Generic repository with LINQ
IRepository<T> {
    FindAsync(predicate, orderBy, includes...)   // LINQ filtering
    GetPagedAsync(page, pageSize, predicate...)  // Paged LINQ
    CountAsync(predicate)                        // LINQ aggregate
}

// Specialized repository
INpiRecordRepository : IRepository<NpiRecord> {
    GetFullRecordAsync(id)                  // Split query eager loading
    SearchAsync(term, status, page, size)   // Dynamic LINQ search
    GetTotalLaborCostAsync(id, category)    // LINQ Sum()
}

// Unit of Work
IUnitOfWork {
    NpiRecords, SetupQuestions, Notes...    // All repositories
    SaveChangesAsync()                      // Single commit point
    BeginTransactionAsync()                 // Transaction support
}
```

---

## Project Structure

```
src/NPI.Shared/
  Enums/NpiEnums.cs              — All enums
  DTOs/NpiRecordDto.cs           — Main DTO
  DTOs/ChildDtos.cs              — All child DTOs + ApiResponse<T>

src/NPI.Data/
  Entities/BaseEntity.cs         — Base + Auditable abstract classes
  Entities/NpiRecord.cs          — Aggregate root
  Entities/ChildEntities.cs      — All child entities
  Context/NpiDbContext.cs         — EF Core context + Fluent API config
  Repositories/Repository.cs     — Generic IRepository<T> + implementation
  Repositories/NpiRecordRepository.cs — Specialized with LINQ queries
  UnitOfWork/UnitOfWork.cs        — UoW with lazy repos + transactions
  Seed/NpiSeedData.cs            — Seed data matching UI prototype
  DataServiceRegistration.cs     — DI extension method

src/NPI.Server/
  Controllers/NpiRecordsController.cs  — Main CRUD controller
  Controllers/ChildControllers.cs      — Toggle, packaging, notes, changelog
  Mapping/NpiMappingProfile.cs         — AutoMapper profile
  Program.cs                           — Host setup + SQLite + CORS + seed

src/NPI.Client/
  Services/NpiService.cs         — INpiService + HttpClient implementation
  ViewModels/ViewModelBase.cs    — MVVM base with INotifyPropertyChanged
  ViewModels/NpiDetailViewModel.cs — Main detail page ViewModel
  ViewModels/NpiListViewModel.cs   — List page ViewModel
  Components/Shared/YnToggle.razor       — Reusable Y/N toggle
  Components/Shared/CollapsibleSection.razor — Collapsible card
  Components/Shared/Badge.razor          — Status badge
  Components/Pages/NpiDetail.razor       — Detail page (all 4 tabs)
  Components/Pages/NpiList.razor         — List / search page
  wwwroot/css/app.css            — Full dark/light theme CSS

tests/NPI.Tests/
  RepositoryTests.cs             — 15 unit tests covering CRUD, LINQ, UoW
```
