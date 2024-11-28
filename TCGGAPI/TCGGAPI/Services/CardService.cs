using TCGGAPI.Models;

namespace TCGGAPI.Services;

public class CardService : ICardService
{
    public CardDefintion GetCard(int playerId, Match match)
    {
        var deck = GetPlayerDeck(playerId, match);
        var card = deck.Cards.FirstOrDefault();
        deck.Cards.Remove(card);
        return card;
    }

    public CardDefintion GetRandomCard(int playerId, Match match)
    {
        var deck = GetPlayerDeck(playerId, match);
        var random = new Random();
        var card = deck.Cards[random.Next(deck.Cards.Count)];
        deck.Cards.Remove(card);
        return card;
    }

    public void AttackCard(int attackCardId, int defenseCardId, int playerId, Match match)
    {
        var (attackerField, defenderField) = GetField(playerId, match);
            
        var (attackerGraveyard, defenderGraveyard) = GetGraveyard(playerId, match);

        var attackerCard = GetCardFromField(attackCardId, attackerField);
        var defenderCard = GetCardFromField(defenseCardId, defenderField);

        PerformAttack(attackerCard, defenderCard);
        CardDeath(attackerCard, attackerField, attackerGraveyard);
        CardDeath(defenderCard, defenderField, defenderGraveyard);
    }
    public Player AttackPlayer(int playerId, int cardId, Match match)
    {

        var attackerField = GetField(playerId, match).attackerField;
        var card = GetCardFromField(cardId, attackerField)
                   ?? throw new InvalidOperationException("Attacking card not found.");

        var enemy = GetEnemy(playerId, match);
        enemy.Health -= card.Attack;

        if (enemy.Health <= 0)
        {
            enemy.Health = 0;
        }

        return enemy;
    }

    private Player GetEnemy(int playerId, Match match) =>
        playerId == match.Player1.Id ? match.Player2 : match.Player1;
    
    private (List<CardDefintion> attackerField, List<CardDefintion> defenderField) GetField(int playerId, Match match)
    {
        return playerId == match.Player1.Id ? (match.Board.Player1Field, match.Board.Player2Field) : (match.Board.Player2Field, match.Board.Player1Field);
    }

    private (List<CardDefintion> attackerGraveyard, List<CardDefintion> defenderGraveyard) GetGraveyard(int playerId,
        Match match)
    {
        return playerId == match.Player1.Id ? (match.Player1.Graveyard, match.Player2.Graveyard) : (match.Player2.Graveyard, match.Player1.Graveyard);
    }
    
    private CardDefintion GetCardFromField(int cardId, List<CardDefintion> field)
    {
        var card = field.FirstOrDefault(x => x.Id == cardId);
        if (card == null)
            throw new InvalidOperationException("No card found");
        return card;
    }

    private void PerformAttack(CardDefintion attackerCard, CardDefintion defenderCard)
    {
        attackerCard.Health -= defenderCard.Attack;
        defenderCard.Health -= attackerCard.Attack;
    }
    
    private Deck GetPlayerDeck(int playerId, Match match)
    {
        return playerId == match.Player1.Id ? match.Player1.MatchDeck : match.Player2.MatchDeck;
    }

    private void CardDeath(CardDefintion card, List<CardDefintion> field, List<CardDefintion> graveyard)
    {
        if (card.Health <= 0)
        {
            field.Remove(card);
            graveyard.Add(card);
        }
    }
}   

public interface ICardService
{
    CardDefintion GetCard(int playerId, Match match);
    void AttackCard(int attackCardId, int defenseCardId, int playerId, Match match);
    Player AttackPlayer(int playerId, int cardId, Match match);
    CardDefintion GetRandomCard(int playerId, Match match);
}
