using Microsoft.EntityFrameworkCore;
using NewShowsInTown.Shared.Dtos;
using NewShowsInTown.Shared.Models;

namespace NewShowsInTown.Api.Endpoints;

public static class ShowEndpoints
{
    public static void MapShowEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/shows")
            .WithTags("Shows");

        group.MapGet("/", GetAllShows)
            .WithName("GetAllShows");

        group.MapGet("/{id:int}", GetShow)
            .WithName("GetShow");

        group.MapPost("/", CreateShow)
            .WithName("CreateShow");

        group.MapPut("/{id:int}", UpdateShow)
            .WithName("UpdateShow");

        group.MapDelete("/{id:int}", DeleteShow)
            .WithName("DeleteShow");

        group.MapPost("/upsert", UpsertShow)
            .WithName("UpsertShow");
    }

    private static async Task<IResult> GetAllShows(AppDbContext db, int? venueId = null)
    {
        var query = db.Shows
            .AsNoTracking()
            .Include(s => s.Venue)
            .Include(s => s.ShowTimes)
            .AsQueryable();

        if (venueId.HasValue)
            query = query.Where(s => s.VenueId == venueId.Value);

        var shows = await query
            .OrderBy(s => s.Title)
            .Select(s => new ShowSummaryDto(
                s.Id,
                s.Title,
                s.Subtitle,
                s.ImageUrl,
                s.Category,
                s.Venue.Name,
                s.ShowTimes
                    .Where(st => st.StartsAt >= DateTime.UtcNow)
                    .OrderBy(st => st.StartsAt)
                    .Select(st => (DateTime?)st.StartsAt)
                    .FirstOrDefault()
            ))
            .ToListAsync();

        return Results.Ok(shows);
    }

    private static async Task<IResult> GetShow(AppDbContext db, int id)
    {
        var show = await db.Shows
            .AsNoTracking()
            .Include(s => s.Venue)
            .Include(s => s.ShowTimes)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (show is null)
            return Results.NotFound();

        var dto = MapToDetailDto(show);
        return Results.Ok(dto);
    }

    private static async Task<IResult> CreateShow(AppDbContext db, CreateShowDto dto)
    {
        var venueExists = await db.Venues.AnyAsync(v => v.Id == dto.VenueId);
        if (!venueExists)
            return Results.BadRequest(new { Error = $"Venue with Id {dto.VenueId} not found." });

        var show = new Show
        {
            Title       = dto.Title,
            Language    = dto.Language,
            Subtitle    = dto.Subtitle,
            Description = dto.Description,
            ImageUrl    = dto.ImageUrl,
            EventUrl    = dto.EventUrl,
            Category    = dto.Category,
            VenueId     = dto.VenueId,
            Venue       = null!, // Satisfy `required` — EF uses VenueId, not the nav property
            ShowTimes   = dto.ShowTimes.Select(st => new ShowTime
            {
                StartsAt = st.StartsAt,
                Show     = null! // Satisfy `required` — EF sets this via foreign key
            }).ToList()
        };

        db.Shows.Add(show);
        await db.SaveChangesAsync();

        // Load venue for the response DTO
        await db.Entry(show).Reference(s => s.Venue).LoadAsync();

        var response = MapToDetailDto(show);
        return Results.Created($"/shows/{show.Id}", response);
    }

    private static async Task<IResult> UpdateShow(AppDbContext db, int id, UpdateShowDto dto)
    {
        var show = await db.Shows
            .Include(s => s.ShowTimes)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (show is null)
            return Results.NotFound();

        var venueExists = await db.Venues.AnyAsync(v => v.Id == dto.VenueId);
        if (!venueExists)
            return Results.BadRequest(new { Error = $"Venue with Id {dto.VenueId} not found." });

        show.Title       = dto.Title;
        show.Language    = dto.Language;
        show.Subtitle    = dto.Subtitle;
        show.Description = dto.Description;
        show.ImageUrl    = dto.ImageUrl;
        show.EventUrl    = dto.EventUrl;
        show.Category    = dto.Category;
        show.VenueId     = dto.VenueId;

        // Replace all showtimes (full sync from scraper)
        db.ShowTimes.RemoveRange(show.ShowTimes);
        show.ShowTimes = dto.ShowTimes.Select(st => new ShowTime
        {
            StartsAt = st.StartsAt,
            Show     = null! // Satisfy `required` — EF sets this via foreign key
        }).ToList();

        await db.SaveChangesAsync();

        // Load venue for the response DTO
        await db.Entry(show).Reference(s => s.Venue).LoadAsync();

        var response = MapToDetailDto(show);
        return Results.Ok(response);
    }

    private static async Task<IResult> DeleteShow(AppDbContext db, int id)
    {
        var show = await db.Shows.FindAsync(id);
        if (show is null)
            return Results.NotFound();

        db.Shows.Remove(show);
        await db.SaveChangesAsync();

        return Results.NoContent();
    }

    private static async Task<IResult> UpsertShow(AppDbContext db, CreateShowDto dto)
    {
        var venueExists = await db.Venues.AnyAsync(v => v.Id == dto.VenueId);
        if (!venueExists)
            return Results.BadRequest(new { Error = $"Venue with Id {dto.VenueId} not found." });

        // Match by title + venue — fallback for scrapers without external IDs
        var existing = await db.Shows
            .Include(s => s.ShowTimes)
            .FirstOrDefaultAsync(s => s.Title == dto.Title && s.VenueId == dto.VenueId);

        if (existing is not null)
        {
            // Update
            existing.Title       = dto.Title;
            existing.Language    = dto.Language;
            existing.Subtitle    = dto.Subtitle;
            existing.Description = dto.Description;
            existing.ImageUrl    = dto.ImageUrl;
            existing.EventUrl    = dto.EventUrl;
            existing.Category    = dto.Category;

            db.ShowTimes.RemoveRange(existing.ShowTimes);
            existing.ShowTimes = dto.ShowTimes.Select(st => new ShowTime
            {
                StartsAt = st.StartsAt,
                Show     = null!
            }).ToList();

            await db.SaveChangesAsync();
            await db.Entry(existing).Reference(s => s.Venue).LoadAsync();

            var updated = MapToDetailDto(existing);
            return Results.Ok(updated);
        }

        // Insert
        var show = new Show
        {
            Title       = dto.Title,
            Language    = dto.Language,
            Subtitle    = dto.Subtitle,
            Description = dto.Description,
            ImageUrl    = dto.ImageUrl,
            EventUrl    = dto.EventUrl,
            Category    = dto.Category,
            VenueId     = dto.VenueId,
            Venue       = null!,
            ShowTimes   = dto.ShowTimes.Select(st => new ShowTime
            {
                StartsAt = st.StartsAt,
                Show     = null!
            }).ToList()
        };

        db.Shows.Add(show);
        await db.SaveChangesAsync();
        await db.Entry(show).Reference(s => s.Venue).LoadAsync();

        var created = MapToDetailDto(show);
        return Results.Created($"/shows/{show.Id}", created);
    }

    private static ShowDetailDto MapToDetailDto(Show show)
    {
        return new ShowDetailDto(
            show.Id,
            show.Title,
            show.Language,
            show.Subtitle,
            show.Description,
            show.ImageUrl,
            show.EventUrl,
            show.Category,
            show.VenueId,
            show.Venue.Name,
            show.ShowTimes
                .OrderBy(st => st.StartsAt)
                .Select(st => new ShowTimeDto(st.Id, st.StartsAt))
                .ToList()
        );
    }
}
