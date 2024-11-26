namespace TCGGAPI.Models;

public class User
{
    public int Id { get; set; }
    public string UserName { get; set; }
    public List<CardDefintion> Decks { get; set; } = new List<CardDefintion>();
    public int Wins { get; set; }
    public int Losses { get; set; }
    public int Draws { get; set; }
}