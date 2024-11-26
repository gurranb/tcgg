using System.ComponentModel.DataAnnotations;

namespace TCGGAPI.Models;

public class Player
{
    public int Id { get; set; }
    public int Health { get; set; }
    public string Name { get; set; }
    public int MatchDeckId { get; set; }
    public List<CardDefintion> Hand { get; set; } = new List<CardDefintion>();
    public List<CardDefintion> Graveyard { get; set; } = new List<CardDefintion>();
    public Deck MatchDeck { get; set; } = new Deck();
}
