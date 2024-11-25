using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TCGGAPI.Models;

namespace TCGGAPI;

public class GameManager
{
    public Player P1 { get; set; }
    public Player P2 { get; set; }

    public Match Match { get; set; }

    public GameManager()
    {
        // TODO: Config Players, Add matchDeck
        P1 = new Player();
        P1.Id = 1;
        P1.Name = "Player 1";

        P2 = new Player();
        P2.Id = 2;
        P2.Name = "Player 2";


        Match = new Match();


        Match.Player1 = P1;
        Match.Player2 = P2;
    }

    public static void StartMatch()
    {
        // TODO: Config match
    }

    public void TakeTurn(Match match, Player player)
    {
        // TODO: Config turn
    }

    public Card DrawCard(int PlayerId)
    {
        // TODO: Add card to board
        // TODO: Validate correct player

        // TODO: get actual card from db
        Random rnd = new Random();
        var card = new Card
        {
            Id = 1,
            Name = "Human",
            Health = 1,
            Attack = 1
        };

        // TODO: which player?
        if (PlayerId == 1)
            P1.Hand.Add(card);
        else if (PlayerId == 2)
            P2.Hand.Add(card);

        // TODO: Return card
        return card;
    }

    public Card PlayCardToBoard(int playerId, int cardId)
    {
        // TODO: Validate correct player
        if (playerId == 1)
        {
            var card = P1.Hand.FirstOrDefault(c => c.Id == cardId);

            if (card != null)
            {
                P1.Hand.Remove(card);
                // TODO: Add card to board

                Match.Board.Player1Field.Add(card);
                return card;
            }
        }
        else if (playerId == 2)
        {
            var card = P2.Hand.FirstOrDefault(c => c.Id == cardId);

            if (card != null)
            {
                P2.Hand.Remove(card);
                // TODO: Add card to board

                Match.Board.Player2Field.Add(card);
                return card;
            }
        }
        // TODO: Validate card is in hand
        // TODO: Remove card from hand


        return null;
    }

    public Card AttackCard(int attackCardId, int defenseCardId, int playerId)
    {
        // TODO: Config attack
        var attackerCard = Match.Board.Player1Field.FirstOrDefault(c => c.Id == attackCardId);
        var defenderCard = Match.Board.Player2Field.FirstOrDefault(c => c.Id == defenseCardId);
        
        if (attackCardId != null && defenseCardId != null)
        {
            Match.Board.Player2Field.Remove(defenderCard);
        }
        // TODO: Validate cards on board
        // TODO: Attack logic
        // TODO: 
        return new Card();
    }

    public void AttackLogic(Card attackerCard, Card defenderCard)
    {
        // TODO: Different logics

        // var p1Attack = attackerCard.Attack;
        // var p1Health = attackerCard.Health;
        //
        // var p2Attack = defenderCard.Attack;
        // var p2Health = defenderCard.Health;
        //
        // p1Health -= p2Attack;
        // p2Health -= p1Attack;
        //
        // if (p1Health <= 0)
        // {
        //     return 
        // }
        
        
    }
}