namespace NewShowsInTown.Shared.Models;

public class Show
{
    public int             Id        { get; set; }
    public required string Title     { get; set; }
    public string?         Subtitle  { get; set; }
    public int             VenueId   { get; set; } // Foreign key to Venue
    public required Venue  Venue     { get; set; } // Navigation property to Venue
    public List<ShowTime>  ShowTimes { get; set; } = [];
}