namespace TCGGAPI.Models;

public class Board
{
    public int Id { get; set; }
    public Player Player1 { get; set; } 
    public Player Player2 { get; set; }
    public int Turns { get; set; }
    public List<Card> CombatZone { get; set; }
}