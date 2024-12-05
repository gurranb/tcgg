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
    private Deck _deck;
    private Player _currentPlayer;
    private Player _player1;
    private Player _player2;
    private List<CardDefinition> _field1;
    private List<CardDefinition> _field2;
    private List<CardDefinition> _graveyard1;
    private List<CardDefinition> _graveyard2;
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
        _outputHelper.WriteLine("Match started.");
        
    }

    private Tuple<List<CardDefinition>, List<CardDefinition>>GetPlayersHandTest()
    {
        return new Tuple<List<CardDefinition>, List<CardDefinition>>(_player1.Hand, _player2.Hand);
    }

    private void CheckPlayerHandTest()
    {
        var (player1Hand, player2Hand) = GetPlayersHandTest();
        Assert.NotNull(player1Hand);
        Assert.NotNull(player2Hand);
        Assert.Equal(3, player1Hand.Count);
        Assert.Equal(3, player2Hand.Count);
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

    private void DrawCardTest()
    {
        _currentPlayer = GetCurrentPlayer();
        var card = _gameManager.DrawRandomCard(_currentPlayer.Id);
        Assert.NotNull(_currentPlayer.Hand[3]);
        _outputHelper.WriteLine($"Player {_currentPlayer.Id} draws a card.");
        _outputHelper.WriteLine(card.Name + " HP: " + card.Health + " ATK: " + card.Attack);
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
        _field1 = GetCurrentPlayerField();
        _field2 = GetEnemyField();
        
        _outputHelper.WriteLine("Player " + _currentPlayer.Id + " attacks " + _field2[0].Name + " with " + _field1[0].Name);

        _gameManager.AttackCard(_field1[0].Id, _field2[0].Id, _currentPlayer.Id);
    }

    private void AttackPlayer()
    {
        var currentPlayerField = GetCurrentPlayerField();
        var enemy = _currentPlayer == _match.Player1 ? _match.Player2 : _match.Player1;
        _outputHelper.WriteLine($"{enemy.Name} has {enemy.Health} health.");
        _matchService.AttackPlayer(_currentPlayer.Id, currentPlayerField[0].Id);
        _outputHelper.WriteLine($"{enemy.Name} has {enemy.Health} health.");
        if (enemy.Health <= 0)
        {
            EndMatchTest();   
        }
    }
    
    private void EndMatchTest()
    {
        var winner = _match.Winner;
        _outputHelper.WriteLine($"{winner.Name} wins the match.");
        _outputHelper.WriteLine("Match ended.");
    }

    [Fact]
    public void SimulateMatchN1()
    {
        // Check if match starts correctly
        StartMatchTest();
        CheckPlayerHandTest();
        CheckPlayerTest();
        
        // First player's turn
        GetCurrentPlayerTest();
        DrawCardTest();
        PlayCardTest();
        EndTurnTest();

        // Second player's turn
        GetCurrentPlayerTest();
        DrawCardTest();
        PlayCardTest();
        EndTurnTest();

        while (_match.Winner == null)
        {
             // First player's turn
                    GetCurrentPlayerTest();
                    DrawCardTest();
                    PlayCardTest();
                    AttackCard();
                    if (_match.Winner != null) // Check if the match has ended
                    {
                        EndMatchTest();
                        break; // Exit the loop
                    }
                    EndTurnTest();
                    
                    // Second player's turn
                    GetCurrentPlayerTest();
                    DrawCardTest();
                    PlayCardTest();
                    AttackPlayer();
                    if (_match.Winner != null) // Check if the match has ended
                    {
                        EndMatchTest();
                        break; // Exit the loop
                    }
                    EndTurnTest();
                    
        }
    }
}