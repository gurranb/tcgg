using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TCGGAPI.Models;

namespace TCGGAPI;

public class GameManager
{
    public Random Random = new Random();
    public Player P1 { get; set; }
    public Player P2 { get; set; }
    public Match Match { get; set; }

    public GameManager()
    {
        
    }

    public Match StartMatch(int coinToss)
    {
        var coinTossResult = Random.Next(0, 2);
        P1 = CreatePlayer(1, "p1");
        P2 = CreatePlayer(2, "p2");
        // TODO: Config match
        Match = new Match();
        Match.Board = new Board() { Player1 = P1, Player2 = P2 };
        Match.Player1 = P1;
        Match.Player2 = P2;
        if (coinToss == coinTossResult)
        {
            Match.Board.CurrentPlayerId = P1.Id;
        }
        else
        {
            Match.Board.CurrentPlayerId = P2.Id;
        }
        
        return Match;
    }
    
    public void EndTurn(int playerId)
    {
        CheckGameStatus();
        CheckTurn(playerId);
        
        Match.Board.CurrentPlayerId = playerId == 1 ? 2 : 1;
        Match.Board.Turns++;
    }

    private Player CreatePlayer(int id, string name)
    {
        return new Player{Id = id, Name = name, Health = 10};   
    }
    
    public void RestartMatch(int coinToss)
    {
        StartMatch(coinToss);
    }

    public void CheckTurn(int playerId)
    {
        if (Match.Board.CurrentPlayerId != playerId)
        {
            throw new InvalidOperationException("Inte din tur");
        }
    }

    public CardDefintion DrawCard(int playerId)
    {
        CheckGameStatus();
        CheckTurn(playerId);

        // TODO: Add card to board
        // TODO: Validate correct player

        // TODO: get actual card from db
        var card = new CardDefintion
        {
            Id = 1,
            Name = "Human",
            Health = Random.Next(1,3),
            Attack = 1,
            Rarity = 0
        };

        // TODO: which player?
        if (playerId == 1)
            P1.Hand.Add(card);
        else if (playerId == 2)
            P2.Hand.Add(card);

        // TODO: Return card
        return card;
    }

    public CardDefintion PlayCardToBoard(int playerId, int cardId)
    {
        CheckGameStatus();
        CheckTurn(playerId);

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

    public void AttackCard(int attackCardId, int defenseCardId, int playerId)
    {
    CheckGameStatus();
    CheckTurn(playerId);


    var player = GetPlayer(playerId);
    var attackerCard = new CardDefintion();
    var defenderCard = new CardDefintion();

    if (Match.Player1 == player)
    {
        attackerCard = Match.Board.Player1Field.FirstOrDefault(c => c.Id == attackCardId);
        defenderCard = Match.Board.Player2Field.FirstOrDefault(c => c.Id == defenseCardId);
    }
    else if (Match.Player2 == player)
    {
        attackerCard = Match.Board.Player2Field.FirstOrDefault(c => c.Id == attackCardId);
        defenderCard = Match.Board.Player1Field.FirstOrDefault(c => c.Id == defenseCardId);        
    }


    if (attackerCard == null || defenderCard == null)
    {
        throw new ArgumentException("Error. Invalid card for attack");
    }

    attackerCard.Health -= defenderCard.Attack;
    defenderCard.Health -= attackerCard.Attack;

    if (attackerCard.Health <= 0 && defenderCard.Health <= 0)
    {
        if (Match.Board.Player1 == player)
        {
            Match.Board.Player1Field.Remove(attackerCard);
            Match.Board.Player2Field.Remove(defenderCard);
        }
        else
        {
            Match.Board.Player2Field.Remove(attackerCard);
            Match.Board.Player1Field.Remove(defenderCard);
        }
    }
    else if (attackerCard.Health <= 0)
    {
        if (Match.Board.Player1 == player)
        {
            Match.Board.Player1Field.Remove(attackerCard);
        }
        else
        {
            Match.Board.Player2Field.Remove(attackerCard);
        }
    }
    else if (defenderCard.Health <= 0)
    {
        if (Match.Board.Player1 == player)
        {
            Match.Board.Player2Field.Remove(defenderCard);
        }
        else
        {
            Match.Board.Player1Field.Remove(defenderCard);
        }
    }
}

    public Player AttackPlayer(int playerId, int cardId)
    {
        CheckGameStatus();
        CheckTurn(playerId);

        var player = GetPlayer(playerId);
        var enemy = GetEnemy(playerId);
        var card = new CardDefintion();

        if (Match.Player1.Id == playerId)
        {
            card = Match.Board.Player1Field.FirstOrDefault(x => x.Id == cardId);
        }
        else if (Match.Player2.Id == playerId)
        {
            card = Match.Board.Player2Field.FirstOrDefault(x => x.Id == cardId);
        }

        if (card != null && card.Attack > 0)
        {
            enemy.Health -= card.Attack;
            if (enemy.Health <= 0)
            {
                enemy.Health = 0;
            }
        }

        CheckHealth();
        return enemy;
    }

    public void CheckHealth()
    {
        if (Match.Player1.Health <= 0)
        {
            EndGame(Match.Player2);
        }
        else if (Match.Player2.Health <= 0)
        {
            EndGame(Match.Player1);
        }
    }

    public void CheckGameStatus()
    {
        if(Match.Status == "Game Over")
        {
            throw new InvalidOperationException("Game Over");
        }
    }

    private Player GetPlayer(int playerId)
    {
        return Match.Player1.Id == playerId ? Match.Player1 : Match.Player2;
    }
    
    private Player GetEnemy(int playerId)
    {
        return Match.Player1.Id == playerId ? Match.Player2 : Match.Player1;
    }

    public void EndGame(Player winner)
    {
        Console.WriteLine("Game Over. Winner, winner, chicken dinner! " + winner.Name + " wins!");
        Match.Status = "Game Over";
        Match.WinnerId = winner.Id;
        Match.Winner = winner;
    }
}