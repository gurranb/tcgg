using TCGGAPI.DTO;
using TCGGAPI.Models;

namespace TCGGAPI.Services;

public class MatchService : IMatchService
{
    private readonly ICardService _cardService;
    private readonly Random _random;
    private Match _match;
    private Board _board;
    private Deck _deck;
    private Player P1;
    private Player P2;

    // Constructor to initialize the MatchService with a card service and optional random instance
    public MatchService(ICardService cardService, Random random = null)
    {
        _cardService = cardService;
        _random = random ?? new Random();
    }

    // Retrieves the current match
    public Match GetMatch()
    {
        return _match;
    }

    // Retrieves the current game board
    public Board GetBoard()
    {
        return _board;
    }

    // Gets the hand of cards for the specified player
    public List<CardDefinition> GetPlayerHand(int playerId)
    {
        return GetPlayer(playerId).Hand;
    }

    // Starts a new match with the given coin toss
    public Match StartMatch(int coinToss)
    {
        int coinTossResult = _random.Next(0, 2);

        P1 = CreatePlayer(1, "Player 1");
        P2 = CreatePlayer(2, "Player 2");

        _match = new Match
        {
            Board = new Board { Player1 = P1, Player2 = P2, Player1Id = P1.Id, Player2Id = P2.Id, Turns = 1},
            Player1 = P1,
            Player2 = P2
        };

        _board = _match.Board;

        P1.MatchDeck = GenerateDeck();
        P2.MatchDeck = GenerateDeck();

        // Draw initial cards for both players
        for (int i = 1; i <= 3; i++)
        {
            DrawRandomCard(P1.Id);
            DrawRandomCard(P2.Id);
        }

        // Determine current player based on coin toss
        _match.Board.CurrentPlayerId = coinToss == coinTossResult ? P1.Id : P2.Id;
        StartTurn(_match.Board.CurrentPlayerId);
        return _match;
    }

    // Generates a deck of cards for players
    private Deck GenerateDeck()
    {
        var cards = new List<CardDefinition>();

        // Add basic cards to the deck
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
                Id = _random.Next(1, 1000),
                Attack = 3,
                Health = 2,
                Rarity = 0,
                Name = "Knight"
            });
        }

        // Add rare cards to the deck
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

    // Starts the turn for the specified player
    public void StartTurn(int playerId)
    {
        CheckGameStatus();
        var player = GetPlayer(playerId);
        player.HasPlayedCard = false;
        if (player.MatchDeck.Cards.Count > 0 && player.MatchDeck != null)
        {
            var card = DrawRandomCard(playerId);

            // If hand exceeds limit, move card to graveyard
            if (player.Hand.Count > 7)
            {
                player.Hand.Remove(card);
                player.Graveyard.Add(card);
            }
        }

        CheckHealth();
    }

    // Ends the turn for the specified player
    public void EndTurn(int playerId)
    {
        _match.Board.CurrentPlayerId = playerId == 1 ? 2 : 1;
        _match.Board.Turns++;
        var player = GetPlayer(playerId);
        var playerField = playerId == _match.Board.Player1.Id ? _match.Board.Player1Field : _match.Board.Player2Field;

        // Reset attack status for cards on the field
        foreach (var card in playerField)
        {
            if (card.HasAttacked)
            {
                card.HasAttacked = false;
            }
        }
    }

    // Restarts the match with the specified coin toss
    public void RestartMatch(int coinToss)
    {
        StartMatch(coinToss);
    }

    // Checks if it's the player's turn
    private void CheckTurn(int playerId)
    {
        if (_match.Board.CurrentPlayerId != playerId)
        {
            throw new InvalidOperationException("It's not your turn");
        }
    }

    // Creates a new player
    private Player CreatePlayer(int id, string name)
    {
        return new Player { Id = id, Name = name, Health = 10 };
    }

    // Checks health status of players
    private void CheckHealth()
    {
        if (_match.Player1.Health <= 0) EndGame(P2);
        if (_match.Player2.Health <= 0) EndGame(P1);
    }

    // Ends the game and declares a winner
    private void EndGame(Player winner)
    {
        Console.WriteLine($"Game Over. {winner.Name} wins!");
        _match.Status = "Game Over";
        _match.Winner = winner;
        _match.WinnerId = winner.Id;
    }

    // Checks if the game is over before allowing further actions
    private void CheckGameStatus()
    {
        if (_match.Status == "Game Over")
        {
            Console.WriteLine("Game Over. No further actions can be taken.");
        }
    }

    // Ensures it's the player's turn and the game isn't over
    public void EnsureValidTurn(int playerId)
    {
        CheckGameStatus();
        CheckTurn(playerId);
    }

    // Draws a card for the specified player
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

    // Draws a random card for the specified player
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

    // Draws multiple cards for the specified player
    public List<CardDefinition> DrawMultipleCards(int playerId, int amount)
    {
        var cards = new List<CardDefinition>();
        var player = GetPlayer(playerId);

        if (player == null) throw new InvalidOperationException("Player not found.");

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

    // Plays a specified card to the board for the player
    public CardDefinition PlayCardToBoard(int playerId, int cardId)
    {
        var player = GetPlayer(playerId);
        CheckPlayerHasPlayedCard(player);
        var card = player.Hand.FirstOrDefault(c => c.Id == cardId) 
                   ?? throw new InvalidOperationException("Card not found in hand.");

        player.Hand.Remove(card);

        var targetField = player == P1 ? _match.Board.Player1Field : _match.Board.Player2Field;
        targetField.Add(card);

        card.DeployedTurn = _match.Board.Turns;
        player.HasPlayedCard = true;

        return card;
    }

    // Checks if the player has already played a card this turn
    private void CheckPlayerHasPlayedCard(Player player)
    {
        if (player.HasPlayedCard)
        {
            throw new InvalidOperationException("Player has already played a card this turn.");
        }
    }

    // Attacks a defense card with an attacking card
    public void AttackCard(int attackCardId, int defenseCardId, int playerId)
    {
        _cardService.AttackCard(attackCardId, defenseCardId, playerId, _match);
    }

    // Attacks a player using a specified card
    public Player AttackPlayer(int playerId, int cardId)
    {
        var enemy = _cardService.AttackPlayer(playerId, cardId, _match);
        CheckHealth();
        return enemy;
    }

    // Retrieves the player by ID
    private Player GetPlayer(int playerId) =>
        playerId == P1.Id ? P1 : P2;

    // Retrieves the opponent player
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