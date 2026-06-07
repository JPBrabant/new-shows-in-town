namespace NewShowsInTown.Shared.Models;

public class Show
{
    public int Id                   { get; set; }
    public required string Title    { get; set; }
    public string? Subtitle         { get; set; }
    public required string Language { get; set; } = "French"; // Default to French, as we're in Montreal
    public string? Description      { get; set; }
    public string? ImageUrl         { get; set; }
    public string? EventUrl         { get; set; }
    public string? Category         { get; set; } // "Concert", "Comedy", "Theater", "Movie"
    public int VenueId              { get; set; } // Foreign key to Venue
    public required Venue Venue     { get; set; } // Navigation property to Venue
    public List<ShowTime> ShowTimes { get; set; } = [];
}