using TCGGAPI.Models;
using TCGGAPI.Services;

namespace TCGGAPI.Tests;

[Collection("Match Simulation")]
public class MatchSimulationTest
{
    private GameManager _gameManager;
    private readonly IMatchService _matchService;
    private readonly ICardService _cardService;
    private Match _match;
    private Deck _deck;
    private Player _player1;
    private Player _player2;
    private List<CardDefinition> _field1;
    private List<CardDefinition> _field2;
    private List<CardDefinition> _graveyard1;
    private List<CardDefinition> _graveyard2;

    public MatchSimulationTest()
    {
        _cardService = new CardService();

        _matchService = new MatchService(_cardService);

        _gameManager = new GameManager(_matchService);

        // _player1 = new Player { Id = 1, Name = "Player 1", Health = 10, Graveyard = _graveyard1, MatchDeck = _deck };
        // _player2 = new Player { Id = 2, Name = "Player 2", Health = 10, Graveyard = _graveyard2, MatchDeck = _deck };

        // _deck = new Deck();
        // _deck.Cards = new List<CardDefinition>
        // {
        //     new CardDefinition { Id = 1, Attack = 1, Health = 1, Rarity = 0, Name = "Human" },
        //     new CardDefinition { Id = 2, Attack = 2, Health = 2, Rarity = 0, Name = "Beast" },
        // };

        // _field1 = new List<CardDefinition>();
        // _field2 = new List<CardDefinition>();
        // _graveyard1 = new List<CardDefinition>();
        // _graveyard2 = new List<CardDefinition>();

        // _match = new Match
        // {
        //     Player1 = _player1,
        //     Player2 = _player2,
        //
        //     Board = new Board
        //     {
        //         Player1Field = _field1,
        //         Player2Field = _field2,
        //     }
        // };
    }

    [Fact]
    public void StartIncorrectMatch()
    {
        // Start the game incorrect

        _gameManager.StartMatch(2);

        _match = _matchService.GetMatch();

        Assert.Null(_match);
        
    }

    [Fact]
    public void StartCorrectMatch()
    {
        // Start the game correct

        _gameManager.StartMatch(1);

        _match = _matchService.GetMatch();

        Assert.NotNull(_match);
    }

    [Fact]
    public void CheckPlayers()
    {
        
        // _gameManager.StartMatch(1);
        //
        // _match = _matchService.GetMatch();
        // check player hands
        _player1 = _match.Player1;
        _player2 = _match.Player2;
        var player1Hand = _gameManager.GetHand(_player1.Id).Count;
        var player2Hand = _gameManager.GetHand(_player2.Id).Count;

        Assert.Equal(3, player1Hand);
        Assert.Equal(3, player2Hand);
    }
    
}