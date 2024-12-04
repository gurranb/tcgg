namespace TCGGAPI.Models;

public class Deck
{
    public int Id { get; set; }
    public List<CardDefinition> Cards { get; set; }
}