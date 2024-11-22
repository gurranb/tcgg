using Microsoft.EntityFrameworkCore;
using TCGGAPI.Models;

namespace TCGGAPI.Data;

public class TCGGDBContext : DbContext
{
    public TCGGDBContext(DbContextOptions<TCGGDBContext> options) : base(options) { }
    
    public DbSet<Card> Cards { get; set; }
    public DbSet<Deck> Decks { get; set; }
    public DbSet<Player> Players { get; set; }
    public DbSet<Board> Boards { get; set; }
    public DbSet<Match> Matches { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Card>().HasData(
            new Card
            {
                Id = 1,
                Name = "Human",
                Health = 1,
                Attack = 1
            },
            new Card
            {
                Id = 2,
                Name = "Beast",
                Health = 2,
                Attack = 1
            },
            new Card
            {
                Id = 3,
                Name = "Elf",
                Health = 1,
                Attack = 2
            }
        );
        
    }
}