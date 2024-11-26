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

        // Configure the Player1 relationship
        modelBuilder.Entity<Board>()
            .HasOne(b => b.Player1)
            .WithMany()
            .HasForeignKey(b => b.Player1Id)
            .OnDelete(DeleteBehavior.Restrict); // No cascade delete

        // Configure the Player2 relationship
        modelBuilder.Entity<Board>()
            .HasOne(b => b.Player2)
            .WithMany()
            .HasForeignKey(b => b.Player2Id)
            .OnDelete(DeleteBehavior.Restrict); // No cascade delete

        // Seed data example (optional)
        modelBuilder.Entity<Card>().HasData(
            new Card { Id = 1, Name = "Human", Health = 1, Attack = 1 },
            new Card { Id = 2, Name = "Beast", Health = 2, Attack = 1 },
            new Card { Id = 3, Name = "Elf", Health = 1, Attack = 2 }
        );

        // Create a deck of cards
        // modelBuilder.Entity<Player>().HasData(
        //     new Player { Id = 1, Name = "Player 1", MatchDeckId = 1 },
        //     new Player { Id = 2, Name = "Player 2", MatchDeckId = 1 }
        // );

    }
}