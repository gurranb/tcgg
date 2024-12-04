using TCGGAPI.Models;

namespace TCGGAPI.DTO;

public class BoardDto
{
    public PlayerDto Player1 { get; set; }
    public PlayerDto Player2 { get; set; }
    public List<CardDefinition> Player1Field { get; set; } = new List<CardDefinition>();
    public List<CardDefinition> Player2Field { get; set; } = new List<CardDefinition>();
    public int Turns { get; set; } 
    
}