## Project: NewShowsInTown

An event aggregation platform that collects show/event data from multiple local venues (Montreal area).

### Stack

- **.NET 10** across all projects
- **ASP.NET Core** for the API with **Entity Framework Core**. **SQL Server** (Azure SQL) and **Minimal APIs** for data access and exposure.
- **Blazor WebAssembly** + **MudBlazor** for the frontend (Azure Static Web Apps)
- **Python** (httpx + BeautifulSoup) for web scraping where venues don't expose APIs
- **Azure Functions** for scrapers and scheduled collection jobs

### Naming & structure conventions

- **API project**: `src/NewShowsInTown.Api/`
- **Frontend project**: `src/NewShowsInTown.Web/`
- **Shared models**: `src/NewShowsInTown.Shared/` — contains `Show.cs`, `ShowTime.cs`, `Venue.cs`
- **Scrapers**: Each venue gets its own project under `src/NewShowsInTown.Scraper.<VenueName>/`
  - Python scrapers use a `pyproject.toml` with `uv` as the package manager
  - .NET scrapers use a standard `.csproj` referencing `NewShowsInTown.Shared`
- **Solution file**: `NewShowsInTown.slnx`

### Coding conventions

- All data models that cross project boundaries (API ↔ scrapers ↔ frontend) must live in `NewShowsInTown.Shared`
- Scrapers output shows matching the `Show` / `ShowTime` / `Venue` shape from Shared
- Use `uv run --project <scraper-path> <scraper-path>/main.py` to execute Python scrapers
- API endpoints should be minimal and RESTful — focus on querying shows by venue, date, and type
- Don't hesitate to split code into multiple files and/or classes — favor smaller, focused files over large monolithic ones

### Infrastructure (Azure)

- Azure SQL Database for persistent storage
- Azure Web App for the ASP.NET Core API
- Azure Static Web App for the Blazor WASM frontend
- Azure Functions for scrapers and scheduled jobs

### Venues tracked

| Venue | Scraper project | Approach | Status |
|---|---|---|---|
| Salle André-Mathieu | `Scraper.SalleAndreMathieu` | Python (BeautifulSoup) | Implemented |
| Place Bell | `Scraper.PlaceBell` | .NET (Algolia-style API) | Planned |
| Cineplex Laval | `Scraper.CineplexLaval` | TBD | Planned |
| Bordel Comédie Club | `Scraper.BordelComedieClub` | TBD | Planned |
| OVMF (Salle Marguerite-Bourgeoys) | `Scraper.OVMF` | TBD | Planned |
