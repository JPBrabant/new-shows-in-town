namespace NewShowsInTown.Shared.Models;

public class Venue
{
    public int             Id             { get; set; }
    public required string Name           { get; set; }
    public required string StreetAddress  { get; set; }
    public required string City           { get; set; }
    public required string PostalCode     { get; set; }
    public required string Province       { get; set; }
    public required string? WebsiteUrl    { get; set; }
    public List<Show>  Shows              { get; set; } = [];
}