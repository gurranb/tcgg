namespace TCGGAPI.Models;

public class CardDefintion : Card
{
    public Rarity Rarity { get; set; }
}

public enum Rarity
{
    Common,
    Uncommon,
    Rare,
    Mythic  
}