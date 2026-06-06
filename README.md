# new-shows-in-town

NewShowsInTown is a .NET solution for aggregating local entertainment events from multiple venues. It combines an ASP.NET Core API, a Blazor WebAssembly frontend, shared models, and source-specific collectors to bring show information together.

## What this project does

- Fetches and aggregates local shows, movies, concerts, and venue information
- Exposes those entities through an API
- Presents the information in a web frontend
- Uses Python for web scraping where that is the most practical approach
- Uses .NET for direct API integrations where the source exposes an API

## Current architecture

- **API**: ASP.NET Core app with Entity Framework Core and SQL Server
- **Web**: Blazor WebAssembly app using MudBlazor
- **Shared**: shared models such as `Show`, `ShowTime`, and `Venue`
- **Scrapers**:
  - Python-based web scraper using `httpx` and `BeautifulSoup`
  - .NET function for somes venus to query their APIs directly

## Repository structure

```text
new-shows-in-town/
├── README.md
├── NewShowsInTown.slnx
├── src/
│   ├── NewShowsInTown.Api/
│   │   ├── Program.cs
│   │   ├── AppDbContext.cs
│   │   └── appsettings.json
│   ├── NewShowsInTown.Web/
│   │   ├── Program.cs
│   │   ├── App.razor
│   │   └── Pages/
│   ├── NewShowsInTown.Shared/
│   │   ├── Show.cs
│   │   ├── ShowTime.cs
│   │   └── Venue.cs
│   ├── NewShowsInTown.Scraper.SalleAndreMathieu/
│   │   └── main.py
│   ├── NewShowsInTown.Scraper.CineplexLaval/
│   │   └── TODO
│   ├── NewShowsInTown.Scraper.PlaceBell/
│   │   └── TODO
│   ├── NewShowsInTown.Scraper.BordelComedieClub/
│   │   └── TODO
│   └── NewShowsInTown.Scraper.OVMF/
│       └── TODO
```

## Infrastructure

The project run on Azure with the following setup:

- **Azure SQL Database** for persistent storage of shows, venues, and related data
- **Azure Web App** for the ASP.NET Core API
- **Azure Static Web App** for the Blazor WebAssembly frontend
- **Azure Functions** for the scrapers and scheduled collection jobs

## Sources currently planned or in progress

- Salle André-Mathieu (co-motion)
  - Python web scraping approach using BeautifulSoup to parse the event page at https://co-motion.ca/spectacles/
- Place Bell (evenko)
  - Planned .NET integration to query the public API directly from the venue source
  - Reference request pattern: an Algolia-style search request to https://placebell.ca/api/algolia/search?query=abc123 (a GET with abc123 base64 encoded)

  ```json
  {
    "filters": {
      "vue": "grille",
      "displayMode": "grid",
      "type": ["evenko_show", "show"],
      "search": ""
    },
    "options": {
      "hitsPerPage": 20,
      "page": 0
    },
    "indexName": "master_evenko_en-CA",
    "venueSlug": "place-bell"
  }
  ```

- Cineplex Laval — TODO
- Bordel Comédie Club — TODO
- OVMF (Salle Marguerite-Bourgeoys) — TODO

