using System.ComponentModel.DataAnnotations;

namespace TCGGAPI.Models;

public class Player
{
    public int Id { get; set; }
    [MaxLength(25)]
    public string? Name { get; set; } 
    public int Health { get; set; }
    public List<Card> Hand { get; set; } = new List<Card>();
    
    
    
    // todo: move to User model
    public List<Deck> Decks { get; set; } = new List<Deck>();
    public Deck MatchDeck { get; set; } = new Deck();
    
    public List<PlayerCard> PlayerCards { get; set; } = new List<PlayerCard>();
    public int MatchDeckId { get; set; }
    
    // public int id
    // public int cardId 
    // public int playerId 
    
    
}
