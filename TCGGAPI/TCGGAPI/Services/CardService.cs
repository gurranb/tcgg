using TCGGAPI.Models;

namespace TCGGAPI.Services;

public class CardService : ICardService
{
    // Retrieves a card from the player's deck
    public CardDefinition GetCard(int playerId, Match match)
    {
        var deck = GetPlayerDeck(playerId, match);
        var card = deck.Cards.FirstOrDefault();
        
        return card;
    }

    // Retrieves a random card from the player's deck
    public CardDefinition GetRandomCard(int playerId, Match match)
    {
        var deck = GetPlayerDeck(playerId, match);
        var random = new Random();
        if(deck.Cards.Count == 0) return null;
        var card = deck.Cards[random.Next(deck.Cards.Count)];
       
        return card;
    }

    // Attacks a defense card with an attacking card
    public void AttackCard(int attackCardId, int defenseCardId, int playerId, Match match)
    {
        var (attackerField, defenderField) = GetField(playerId, match);
        var (attackerGraveyard, defenderGraveyard) = GetGraveyard(playerId, match);

        var attackerCard = GetCardFromField(attackCardId, attackerField);
        var defenderCard = GetCardFromField(defenseCardId, defenderField);

        if (attackerCard.DeployedTurn == match.Board.Turns)
        {
            throw new InvalidOperationException("Attacking card has already been deployed this turn.");
        }
        
        CheckCardHasAttacked(attackerCard);
        
        PerformAttack(attackerCard, defenderCard);
        SetCardHasAttacked(attackerCard);
        CardDeath(attackerCard, attackerField, attackerGraveyard);
        CardDeath(defenderCard, defenderField, defenderGraveyard);
    }

    // Attacks a player directly using a specified card
    public Player AttackPlayer(int playerId, int cardId, Match match)
    {
        var attackerField = GetField(playerId, match).attackerField;
        var card = GetCardFromField(cardId, attackerField)
                   ?? throw new InvalidOperationException("Attacking card not found.");
        
        if (card.DeployedTurn == match.Board.Turns)
        {
            throw new InvalidOperationException("Attacking card has already been deployed this turn.");
        }

        CheckCardHasAttacked(card);
        var enemy = GetEnemy(playerId, match);
        enemy.Health -= card.Attack;
        SetCardHasAttacked(card);

        if (enemy.Health <= 0)
        {
            enemy.Health = 0;
        }

        return enemy;
    }

    // Retrieves the opposing player based on player ID
    private Player GetEnemy(int playerId, Match match) =>
        playerId == match.Player1.Id ? match.Player2 : match.Player1;

    // Gets the fields of the attacker and defender
    private (List<CardDefinition> attackerField, List<CardDefinition> defenderField) GetField(int playerId, Match match)
    {
        return playerId == match.Player1.Id ? (match.Board.Player1Field, match.Board.Player2Field) : (match.Board.Player2Field, match.Board.Player1Field);
    }

    // Gets the graveyards of the attacker and defender
    private (List<CardDefinition> attackerGraveyard, List<CardDefinition> defenderGraveyard) GetGraveyard(int playerId, Match match)
    {
        return playerId == match.Player1.Id ? (match.Player1.Graveyard, match.Player2.Graveyard) : (match.Player2.Graveyard, match.Player1.Graveyard);
    }
    
    // Retrieves a card from the specified field
    private CardDefinition GetCardFromField(int cardId, List<CardDefinition> field)
    {
        var card = field.FirstOrDefault(x => x.Id == cardId);
        if (card == null)
            throw new InvalidOperationException("No card found");
        return card;
    }

    // Performs an attack between two cards
    private void PerformAttack(CardDefinition attackerCard, CardDefinition defenderCard)
    {
        attackerCard.Health -= defenderCard.Attack;
        defenderCard.Health -= attackerCard.Attack;
    }
    
    // Retrieves the player's deck from the match
    private Deck GetPlayerDeck(int playerId, Match match)
    {
        return playerId == match.Player1.Id ? match.Player1.MatchDeck : match.Player2.MatchDeck;
    }

    // Handles the death of a card
    private void CardDeath(CardDefinition card, List<CardDefinition> field, List<CardDefinition> graveyard)
    {
        if (card.Health <= 0)
        {
            field.Remove(card);
            graveyard.Add(card);
        }
    }

    // Marks a card as having attacked this turn
    private void SetCardHasAttacked(CardDefinition card)
    {
        card.HasAttacked = true;
    }

    // Checks if a card has already attacked this turn
    private void CheckCardHasAttacked(CardDefinition card)
    {
        if (card.HasAttacked)
        {
            throw new InvalidOperationException("Card has already attacked this turn.");
        }
    }
}   

public interface ICardService
{
    CardDefinition GetCard(int playerId, Match match);
    void AttackCard(int attackCardId, int defenseCardId, int playerId, Match match);
    Player AttackPlayer(int playerId, int cardId, Match match);
    CardDefinition GetRandomCard(int playerId, Match match);
}