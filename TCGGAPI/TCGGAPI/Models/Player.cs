using System.ComponentModel.DataAnnotations;

namespace TCGGAPI.Models;

public class Player
{
    public int Id { get; set; }
    [MaxLength(25)]
    public string? Name { get; set; } 
    public int Health { get; set; }
    public bool? CurrentTurn { get; set; }
    public List<PlayerCard> PlayerCards { get; set; } = new List<PlayerCard>();
    public List<Deck> Decks { get; set; } = new List<Deck>();
    public Deck MatchDeck { get; set; } = new Deck();
    
    // public int id
    // public int cardId 
    // public int playerId 
    
}
