namespace TCGGAPI.Models;

public class CardDefinition : Card
{
    public Rarity Rarity { get; set; }
    public int DeployedTurn { get; set; }
}

public enum Rarity
{
    Common,
    Uncommon,
    Rare,
    Mythic  
}