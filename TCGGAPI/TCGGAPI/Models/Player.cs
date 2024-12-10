using System.ComponentModel.DataAnnotations;

namespace TCGGAPI.Models;

public class Player
{
    public int Id { get; set; }
    public int Health { get; set; }
    public string Name { get; set; }
    public int MatchDeckId { get; set; }
    public bool HasPlayedCard { get; set; } 
    public List<CardDefinition> Hand { get; set; } = new List<CardDefinition>();
    public List<CardDefinition> Graveyard { get; set; } = new List<CardDefinition>();
    public Deck MatchDeck { get; set; } = new Deck();

}
