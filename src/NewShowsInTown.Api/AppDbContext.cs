using Microsoft.EntityFrameworkCore;
using NewShowsInTown.Shared.Models;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Venue> Venues { get; set; }
}