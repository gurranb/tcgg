namespace TCGGAPI.Models;

public class Card
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Health { get; set; }
    public int Attack { get; set; }
    public List<PlayerCard> PlayerCards { get; set; } = new List<PlayerCard>();
}