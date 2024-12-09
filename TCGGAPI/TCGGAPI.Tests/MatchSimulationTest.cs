using System.Runtime.CompilerServices;
using TCGGAPI.Models;
using TCGGAPI.Services;
using Xunit.Abstractions;

namespace TCGGAPI.Tests;

[Collection("Match Simulation")]
public class MatchSimulationTest
{
    private GameManager _gameManager;
    private readonly IMatchService _matchService;
    private readonly ICardService _cardService;
    private Match _match;
    private Player _currentPlayer;
    private Player _player1;
    private Player _player2;
    private ITestOutputHelper _outputHelper;

    public MatchSimulationTest(ITestOutputHelper outputHelper)
    {
        _cardService = new CardService();
        _matchService = new MatchService(_cardService);
        _gameManager = new GameManager(_matchService);
        _outputHelper = outputHelper;
    }

    private void StartMatchTest()
    {
        _gameManager.StartMatch(1);
        _match = _matchService.GetMatch();
        Assert.NotNull(_match);
        _player1 = _match.Player1;
        _player2 = _match.Player2;
        
        _player1.Hand.Clear();
        _player2.Hand.Clear();

        _player1.MatchDeck = GenerateTestDeck();
        _player2.MatchDeck = GenerateTestDeck();
        
        var currentPlayerId = _match.Board.CurrentPlayerId;
        var currentPlayer = currentPlayerId == _player1.Id ? _player1 : _player2;
        var enemyPlayer = currentPlayerId == _player1.Id ? _player2 : _player1;

        currentPlayer.Hand.AddRange(currentPlayer.MatchDeck.Cards.Take(3).ToList());
        enemyPlayer.Hand.AddRange(enemyPlayer.MatchDeck.Cards.Take(4).ToList());
        currentPlayer.MatchDeck.Cards.RemoveRange(0,3);
        enemyPlayer.MatchDeck.Cards.RemoveRange(0,4);

        _outputHelper.WriteLine("Match started.");
    }

    private Deck GenerateTestDeck()
    {
        var cards = new List<CardDefinition>
        {
            new CardDefinition { Id = 1, Attack = 1, Health = 1, Rarity = 0, Name = "Human 1" },
            new CardDefinition { Id = 2, Attack = 1, Health = 1, Rarity = 0, Name = "Human 2" },
            new CardDefinition { Id = 3, Attack = 1, Health = 1, Rarity = 0, Name = "Human 3" },
            new CardDefinition { Id = 4, Attack = 1, Health = 1, Rarity = 0, Name = "Human 4" },
            new CardDefinition { Id = 5, Attack = 1, Health = 1, Rarity = 0, Name = "Human 5" },
            new CardDefinition { Id = 6, Attack = 1, Health = 1, Rarity = 0, Name = "Human 6" },
            new CardDefinition { Id = 7, Attack = 1, Health = 1, Rarity = 0, Name = "Human 7" },
            new CardDefinition { Id = 8, Attack = 1, Health = 1, Rarity = 0, Name = "Human 8" },
            new CardDefinition { Id = 9, Attack = 1, Health = 1, Rarity = 0, Name = "Human 9" },
            new CardDefinition { Id = 10, Attack = 1, Health = 1, Rarity = 0, Name = "Human 10" },
        };
        return new Deck { Cards = cards };
    }

    private Tuple<List<CardDefinition>, List<CardDefinition>> GetPlayersHandTest()
    {
        return new Tuple<List<CardDefinition>, List<CardDefinition>>(_player1.Hand, _player2.Hand);
    }

    private void CheckPlayerHandTest()
    {
        var (player1Hand, player2Hand) = GetPlayersHandTest();
        var currentPlayer = _match.Board.CurrentPlayerId;

        var expectedPlayer1Hand = _player1.Id == currentPlayer ? 3 : 4;
        var expectedPlayer2Hand = _player2.Id == currentPlayer ? 3 : 4;

        Assert.NotNull(player1Hand);
        Assert.NotNull(player2Hand);
        Assert.Equal(expectedPlayer1Hand, player1Hand.Count);
        Assert.Equal(expectedPlayer2Hand, player2Hand.Count);
    }

    private void CheckPlayerTest()
    {
        Assert.Equal(10, _match.Player1.Health);
        Assert.Equal(10, _match.Player2.Health);
        Assert.NotNull(_match.Player1.MatchDeck);
        Assert.NotNull(_match.Player2.MatchDeck);
        Assert.Empty(_match.Player1.Graveyard);
        Assert.Empty(_match.Player2.Graveyard);
    }

    private Player GetCurrentPlayer()
    {
        return _match.Board.CurrentPlayerId == _match.Player1.Id ? _match.Player1 : _match.Player2;
    }

    private void GetCurrentPlayerTest()
    {
        _currentPlayer = GetCurrentPlayer();
        Assert.NotEqual(0, _currentPlayer.Id);
        _outputHelper.WriteLine($"Current Player ID: {_currentPlayer.Id}");
    }

    private List<CardDefinition> GetField(int playerId)
    {
        return playerId == _match.Player1.Id ? _match.Board.Player1Field : _match.Board.Player2Field;
    }

    private void PlayCardTest()
    {
        _currentPlayer = GetCurrentPlayer();
        _gameManager.PlayCardToBoard(_currentPlayer.Id, _currentPlayer.Hand[0].Id);
        var field = GetField(_currentPlayer.Id);
        _outputHelper.WriteLine($"Player {_currentPlayer.Id} plays a card.");
        _outputHelper.WriteLine(field[0].Name + " HP: " + field[0].Health + " ATK: " + field[0].Attack);
        Assert.NotEqual(field[0].Id, _currentPlayer.Hand[0].Id);
    }

    private void EndTurnTest()
    {
        _currentPlayer = GetCurrentPlayer();
        _gameManager.EndTurn(_currentPlayer.Id);
        _outputHelper.WriteLine("Player " + _currentPlayer.Id + " ends turn.");
        CheckForWinner();
    }

    private List<CardDefinition> GetEnemyField()
    {
        _currentPlayer = GetCurrentPlayer();
        return _currentPlayer.Id == _match.Player1.Id ? _match.Board.Player2Field : _match.Board.Player1Field;
    }

    private List<CardDefinition> GetCurrentPlayerField()
    {
        _currentPlayer = GetCurrentPlayer();
        return _currentPlayer.Id == _match.Player1.Id ? _match.Board.Player1Field : _match.Board.Player2Field;
    }

    private void AttackCard()
    {
        var field1 = GetCurrentPlayerField();
        var field2 = GetEnemyField();

        _outputHelper.WriteLine("Player " + _currentPlayer.Id + " attacks " + field2[0].Name + " with " + field1[0].Name);
        _gameManager.AttackCard(field1[0].Id, field2[0].Id, _currentPlayer.Id);
    }

    private void StartTurn()
    {
        GetCurrentPlayerTest();
        _gameManager.StartTurn(_match.Board.CurrentPlayerId);
    }

    private void AttackPlayer()
    {
        var currentPlayerField = GetCurrentPlayerField();
        var enemy = _currentPlayer == _player1 ? _player2 : _player1;
        var previousHealth = enemy.Health;
        _matchService.AttackPlayer(_currentPlayer.Id, currentPlayerField[0].Id);
        _outputHelper.WriteLine($"{enemy.Name} goes from {previousHealth} health to {enemy.Health} health.");
    }
    
    private void CheckForWinner()
    {
        if (_player1.Health <= 0)
        {
            _outputHelper.WriteLine($"{_player2.Name} wins!");
            Assert.True(_match.WinnerId == _player2.Id);
        }
        else if (_player2.Health <= 0)
        {
            _outputHelper.WriteLine($"{_player1.Name} wins!");
            Assert.True(_match.WinnerId == _player1.Id);
        }
    }

     [Fact]
public void SimulateMatchN1()
{
    StartMatchTest();
    
    CheckPlayerHandTest();
    CheckPlayerTest();

    // Player 1 Turn 1
    StartTurn();
    for (int i = 0; i < 2; i++)
    {
        PlayCardTest();
    }
    EndTurnTest();
    
    // Player 2 Turn 1
    StartTurn();
    PlayCardTest();
    EndTurnTest();

    // Player 1 Turn 2
    StartTurn();
    AttackCard();
    EndTurnTest();   
    
    // Player 2 Turn 2
    StartTurn();
    PlayCardTest();
    EndTurnTest();
    
    // Player 1 Turn 2
    StartTurn();
    PlayCardTest();
    EndTurnTest();   
    
    // Player 2 Turn 2
    StartTurn();
    PlayCardTest();
    AttackPlayer();
    EndTurnTest();
    
    // Player 1 Turn 2
    StartTurn();
    AttackPlayer();
    EndTurnTest();   
    
    // Player 2 Turn 2
    StartTurn();
    AttackPlayer();
    EndTurnTest();
    
    // Player 1 Turn 2
    StartTurn();
    PlayCardTest();
    AttackPlayer();
    AttackPlayer();
    AttackPlayer();
    EndTurnTest();   
    
    // Player 2 Turn 2
    StartTurn();
    PlayCardTest();
    AttackCard();
    EndTurnTest();
    
    // Player 1 Turn 2
    StartTurn();
    PlayCardTest();
    AttackPlayer();
    AttackPlayer();
    AttackPlayer();
    EndTurnTest();   
    
    // Player 2 Turn 2
    StartTurn();
    PlayCardTest();
    AttackCard();
    EndTurnTest();
    
    // Player 1 Turn 2
    StartTurn();
    PlayCardTest();
    AttackPlayer();
    AttackPlayer();
    AttackPlayer();
    EndTurnTest();   
    }
}
