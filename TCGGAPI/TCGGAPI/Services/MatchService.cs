using TCGGAPI.DTO;
using TCGGAPI.Models;

namespace TCGGAPI.Services;

public class MatchService: IMatchService
{
    private readonly ICardService _cardService = new CardService();
    private readonly Random _random = new Random();
    private Match _match;
    private Board _board;
    private Deck _deck;
    private Player P1;
    private Player P2;

    public MatchService(ICardService cardService, Random random = null)
    {
        _cardService = cardService;
        _random = random ?? new Random();
    }

    public Match GetMatch()
    {
        return _match;
    }

    public Board GetBoard()
    {
        return _board;
    }

    public List<CardDefintion> GetPlayerHand(int playerId)
    {
        return GetPlayer(playerId).Hand;
    }

    public Match StartMatch(int coinToss)
    {
        int coinTossResult = _random.Next(0, 2);
        
        P1 = CreatePlayer(1, "Player 1");
        P2 = CreatePlayer(2, "Player 2");

        _match = new Match
        {
            Board = new Board{ Player1 = P1, Player2 = P2 },
            Player1 = P1,
            Player2 = P2
        };
        
        _board = _match.Board;
        
        P1.MatchDeck = GenerateDeck();
        P2.MatchDeck = GenerateDeck();
        
        _match.Board.CurrentPlayerId = coinToss == coinTossResult ? P1.Id : P2.Id;

        return _match;
    }
    
    private Deck GenerateDeck()
    {
        var card = new CardDefintion
        {
            Id = 1,
            Attack = 1,
            Health = 1,
            Rarity = 0,
            Name = "Human"
        };

        var card2 = new CardDefintion
        {
            Id = 2,
            Attack = 3,
            Health = 2,
            Rarity = 0,
            Name = "Knight"
        };

        var card3 = new CardDefintion
        {
            Id = 3,
            Attack = 2,
            Health = 3,
            Rarity = Rarity.Rare,
            Name = "Archer"
        };

        var cards = Enumerable.Repeat(card, 5)
            .Concat(Enumerable.Repeat(card2, 3))
            .Concat(Enumerable.Repeat(card3, 2))
            .ToList();

        return new Deck { Cards = cards };

    }

    public void EndTurn(int playerId)
    {
        _match.Board.CurrentPlayerId = playerId == 1 ? 2 : 1;
        _match.Board.Turns++;
    }
    
    public void RestartMatch(int coinToss)
    {
        StartMatch(coinToss);
    }
    
    private void CheckTurn(int playerId)
    {
        if (_match.Board.CurrentPlayerId != playerId)
        {
            throw new InvalidOperationException("Inte din tur");
        }
    }
    
    private Player CreatePlayer(int id, string name)
    {
        return new Player{Id = id, Name = name, Health = 10};   
    }
    private void CheckHealth()
    {
        if (_match.Player1.Health <= 0) EndGame(P2);
        if (_match.Player2.Health <= 0) EndGame(P1);
    }
   
    private void EndGame(Player winner)
    {
        Console.WriteLine($"Game Over. {winner.Name} wins!");
        _match.Status = "Game Over";
        _match.Winner = winner;
    }
    
    private void CheckGameStatus()
    {
        if (_match.Status == "Game Over")
            throw new InvalidOperationException("Game Over.");
    }

    public void EnsureValidTurn(int playerId)
    {
        CheckGameStatus();
        CheckTurn(playerId);
    }
    
    public CardDefintion DrawCard(int playerId)
    {
        var card = _cardService.GetCard(playerId, _match);
        var hand = GetPlayer(playerId).Hand;
        
        hand.Add(card);
        return card;
    }

    public CardDefintion DrawRandomCard(int playerId)
    {

        var card = _cardService.GetRandomCard(playerId, _match);
        var hand = GetPlayer(playerId).Hand;
        
        hand.Add(card);

        return card;
    }
    
    public CardDefintion PlayCardToBoard(int playerId, int cardId)
    {

        var player = GetPlayer(playerId);
        var card = player.Hand.FirstOrDefault(c => c.Id == cardId) 
                   ?? throw new InvalidOperationException("Card not found in hand.");

        player.Hand.Remove(card);

        var targetField = player == P1 ? _match.Board.Player1Field : _match.Board.Player2Field;
        targetField.Add(card);

        return card;
    }
    
    public void AttackCard(int attackCardId, int defenseCardId, int playerId)
    {
        _cardService.AttackCard(attackCardId, defenseCardId, playerId, _match);
    }
    
    public Player AttackPlayer(int playerId, int cardId)
    {

        var enemy = _cardService.AttackPlayer(playerId, cardId, _match);
        
        CheckHealth();

        return enemy;
    }
    
    private Player GetPlayer(int playerId) =>
        playerId == P1.Id ? P1 : P2;

    private Player GetEnemy(int playerId) =>
        playerId == P1.Id ? P2 : P1;

}

public interface IMatchService
{
    Match GetMatch();
    Match StartMatch(int coinToss);
    
    void EndTurn(int playerId);
    
    void RestartMatch(int coinToss);

    CardDefintion DrawCard(int playerId);
    
    void AttackCard(int attackCardId, int defenseCardId, int playerId);
    
    Player AttackPlayer(int playerId, int cardId);
    
    CardDefintion PlayCardToBoard(int playerId, int cardId);

    void EnsureValidTurn(int playerId);
    
    List<CardDefintion> GetPlayerHand(int playerId);

    Board GetBoard();
    
    CardDefintion DrawRandomCard(int playerId);
}