namespace TCGGAPI.Models;

public class Match
{
    public int Id { get; set; }
    public Board Board { get; set; } = new Board();
    public string Status { get; set; }
    public Player Player1 { get; set; }
    public Player Player2 { get; set; }
}