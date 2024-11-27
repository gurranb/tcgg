using TCGGAPI.Models;

namespace TCGGAPI.DTO;

public class BoardDto
{
    public PlayerDto Player1 { get; set; }
    public PlayerDto Player2 { get; set; }
    public List<CardDefintion> Player1Field { get; set; } = new List<CardDefintion>();
    public List<CardDefintion> Player2Field { get; set; } = new List<CardDefintion>();
    public int Turns { get; set; } 
    
}