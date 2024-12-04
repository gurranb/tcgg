namespace TCGGAPI.Models;

public class User
{
    public int Id { get; set; }
    public string UserName { get; set; }
    public List<CardDefinition> Decks { get; set; } = new List<CardDefinition>();
    public int Wins { get; set; }
    public int Losses { get; set; }
    public int Draws { get; set; }
}