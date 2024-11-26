using System.ComponentModel.DataAnnotations.Schema;

namespace TCGGAPI.Models;

public class Board
{
    public int Id { get; set; }
    
    public Player Player1 { get; set; } 
    public int? Player1Id { get; set; }
    public List<CardDefintion> Player1Field { get; set; } = new List<CardDefintion>();

    public Player Player2 { get; set; }
    public List<CardDefintion> Player2Field { get; set; } = new List<CardDefintion>();
    public int? Player2Id { get; set; }
    public int CurrentPlayerId { get; set; }
    public int Turns { get; set; } 

}