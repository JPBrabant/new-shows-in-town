namespace NewShowsInTown.Shared.Dtos;

public record ShowSummaryDto(
    int Id,
    string Title,
    string? Subtitle,
    string? ImageUrl,
    string? Category,
    string VenueName,
    DateTime? NextShowTime
);
