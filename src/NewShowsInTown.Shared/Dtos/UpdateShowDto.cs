namespace NewShowsInTown.Shared.Dtos;

public record UpdateShowDto(
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
