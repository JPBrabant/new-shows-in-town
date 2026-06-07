using Microsoft.EntityFrameworkCore;
using NewShowsInTown.Shared.Dtos;

namespace NewShowsInTown.Api.Endpoints;

public static class VenueEndpoints
{
    public static void MapVenueEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/venues")
            .WithTags("Venues");

        group.MapGet("/", GetAllVenues)
            .WithName("GetAllVenues");
    }

    private static async Task<IResult> GetAllVenues(AppDbContext db)
    {
        var venues = await db.Venues
            .AsNoTracking()
            .OrderBy(v => v.Name)
            .Select(v => new VenueDto(v.Id, v.Name))
            .ToListAsync();

        return Results.Ok(venues);
    }
}
