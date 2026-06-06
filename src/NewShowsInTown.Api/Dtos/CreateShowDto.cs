namespace NewShowsInTown.Api.Dtos;

public record CreateShowDto(
    string Title,
    string Language,
    int VenueId,
    string? Subtitle,
    string? Description,
    string? ImageUrl,
    string? EventUrl,
    string? Category,
    List<CreateShowTimeDto> ShowTimes
);

public record CreateShowTimeDto(DateTime StartsAt);
