namespace NewShowsInTown.Shared.Models;

public class ShowTime
{
    public int               Id       { get; set; }
    public int               ShowId   { get; set; } // Foreign key to Show
    public required Show     Show     { get; set; } // Navigation property to Show
    public required DateTime StartsAt { get; set; }
}