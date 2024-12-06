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

    public List<CardDefinition> GetPlayerHand(int playerId)
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
            Board = new Board{ Player1 = P1, Player2 = P2 , Player1Id = P1.Id, Player2Id = P2.Id},
            Player1 = P1,
            Player2 = P2
        };
        
        _board = _match.Board;
        
        P1.MatchDeck = GenerateDeck();
        P2.MatchDeck = GenerateDeck();

        for (int i = 1; i <= 3; i++)
        {
            DrawRandomCard(P1.Id);
            DrawRandomCard(P2.Id);
            
        }
        
        _match.Board.CurrentPlayerId = coinToss == coinTossResult ? P1.Id : P2.Id;
        
        if (_match.Board.CurrentPlayerId == P1.Id)
        {
            DrawRandomCard(P2.Id);
        }
        else
        {
            DrawRandomCard(P1.Id);
        }
        return _match;
    }
    
    private Deck GenerateDeck()
    {
        var cards = new List<CardDefinition>();

        for (int i = 0; i <= 5; i++)
        {
            cards.Add(new CardDefinition
            {
                Id = _random.Next(1, 1000),
                Attack = 1,
                Health = 1,
                Rarity = 0,
                Name = "Human"
            });
      
            cards.Add(new CardDefinition
            {
                Id =  _random.Next(1, 1000),
                Attack = 3,
                Health = 2,
                Rarity = 0,
                Name = "Knight"
            });
        }

        for (int i = 0; i <= 2; i++)
        {
            cards.Add(new CardDefinition
            {
                Id = _random.Next(1, 1000),
                Attack = 2,
                Health = 3,
                Rarity = Rarity.Rare,
                Name = "Archer"
            });
        }

        return new Deck { Cards = cards };
    }

    public void StartTurn(int playerId)
    {
        CheckGameStatus();
        DrawRandomCard(playerId);
        CheckHealth();
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
    
    public CardDefinition DrawCard(int playerId)
    {
        var player = GetPlayer(playerId);
        var card = _cardService.GetCard(playerId, _match);
        var hand = player.Hand;
        var deck = player.MatchDeck;
        
        deck.Cards.Remove(card);
        hand.Add(card);
        
        return card;
    }

    public CardDefinition DrawRandomCard(int playerId)
    {

        var card = _cardService.GetRandomCard(playerId, _match);
        var player = GetPlayer(playerId);
        var hand = player.Hand;
        var deck = player.MatchDeck;

        deck.Cards.Remove(card);
        hand.Add(card);

        return card;
    }
    
    public List<CardDefinition> DrawMultipleCards(int playerId, int amount)
    {
        var cards = new List<CardDefinition>();
        var player = GetPlayer(playerId);
        if(player == null) throw new InvalidOperationException("Player not found.");
        var hand = player.Hand;
        var deck = player.MatchDeck;
        for (int i = 1; i <= amount; i++)
        {
            var card = _cardService.GetRandomCard(playerId, _match);
            deck.Cards.Remove(card);
            hand.Add(card);
            cards.Add(card);
        }

        return cards;
    }
    
    public CardDefinition PlayCardToBoard(int playerId, int cardId)
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
    
    void StartTurn(int playerId);
    
    void EndTurn(int playerId);
    
    void RestartMatch(int coinToss);

    CardDefinition DrawCard(int playerId);

    List<CardDefinition> DrawMultipleCards(int playerId, int amount);
    
    void AttackCard(int attackCardId, int defenseCardId, int playerId);
    
    Player AttackPlayer(int playerId, int cardId);
    
    CardDefinition PlayCardToBoard(int playerId, int cardId);

    void EnsureValidTurn(int playerId);
    
    List<CardDefinition> GetPlayerHand(int playerId);

    Board GetBoard();
    
    CardDefinition DrawRandomCard(int playerId);
}