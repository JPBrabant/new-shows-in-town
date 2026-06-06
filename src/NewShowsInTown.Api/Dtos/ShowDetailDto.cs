namespace NewShowsInTown.Api.Dtos;

public record ShowDetailDto(
    int Id,
    string Title,
    string Language,
    string? Subtitle,
    string? Description,
    string? ImageUrl,
    string? EventUrl,
    string? Category,
    int VenueId,
    string VenueName,
    List<ShowTimeDto> ShowTimes
);

public record ShowTimeDto(int Id, DateTime StartsAt);
