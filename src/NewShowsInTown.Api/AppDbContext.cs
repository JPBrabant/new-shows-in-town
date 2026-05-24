using Microsoft.EntityFrameworkCore;
using NewShowsInTown.Shared.Models;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Show> Shows { get; set; }
    public DbSet<ShowTime> ShowTimes { get; set; }
    public DbSet<Venue> Venues { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Venue>().HasData(
            new Venue { Id = 1, Name = "Salle André-Mathieu",   Province = "Québec", City = "Laval", StreetAddress = "475 Bd de l'Avenir",    PostalCode = "H7N 5H9" },
            new Venue { Id = 2, Name = "Cinéma Cineplex Laval", Province = "Québec", City = "Laval", StreetAddress = "2800 Ave du Cosmodôme", PostalCode = "H7T 2X1" },
            new Venue { Id = 3, Name = "Place Bell",            Province = "Québec", City = "Laval", StreetAddress = "1950 Rue Claude-Gagné", PostalCode = "H7N 0E4" }
        );
    }
}