using System.Reflection;
using Moq;
using TCGGAPI.Models;
using TCGGAPI.Services;
using Match = TCGGAPI.Models.Match;


namespace TCGGAPI.Tests;

public class CardTests
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

    private string test123;

    public CardTests()
    {
        _cardService = new CardService();

        _matchService = new MatchService(_cardService);

        _gameManager = new GameManager(_matchService);
        
        _player1 = new Player {Id = 1, Name = "Player 1", Health = 10, Graveyard = _graveyard1, MatchDeck = _deck};
        _player2 = new Player {Id = 2, Name = "Player 2", Health = 10, Graveyard = _graveyard2, MatchDeck = _deck};
        
        _deck = new Deck();
        _deck.Cards = new List<CardDefinition>
        {
            new CardDefinition { Id = 1, Attack = 1, Health = 1, Rarity = 0, Name = "Human" },
            new CardDefinition { Id = 2, Attack = 2, Health = 2, Rarity = 0, Name = "Beast" },
        };

        _field1 = new List<CardDefinition>();
        _field2 = new List<CardDefinition>();
        _graveyard1 = new List<CardDefinition>();
        _graveyard2 = new List<CardDefinition>();

        _match = new Match
        {
            Player1 = _player1,
            Player2 = _player2,
            
            Board = new Board
            {
                Player1Field = _field1,
                Player2Field = _field2,
            }
        };
    }
    
    [Fact]
    public void DrawCard_ShouldReturnCard_WhenCardExists()
    {
        // Arrange
        _gameManager.StartMatch(1);
        var match = _matchService.GetMatch();
        var playerId = match.Player1.Id;

        var card1 = new CardDefinition { Id = 1, Attack = 1, Health = 0, Name = "Human" };
        var card2 = new CardDefinition { Id = 2, Attack = 3, Health = 2, Name = "Archer" };
    
        match.Player1.MatchDeck.Cards.Add(card1);
        match.Player1.MatchDeck.Cards.Add(card2);
    
        var drawnCard = _matchService.DrawCard(playerId);

        var actualHand = _matchService.GetPlayerHand(playerId);
    
        Assert.Contains(actualHand, c => c.Id == drawnCard.Id && c.Name == drawnCard.Name);
    
        var expectedPosition = actualHand.Count - 1; 
        var drawnCardInHand = actualHand[expectedPosition];
    
        Assert.Equal(drawnCard.Name, drawnCardInHand.Name);
        Assert.Equal(drawnCard.Id, drawnCardInHand.Id);
    }

    [Fact]
    public void AttackCard_ShouldUpdateCardHealth()
    {
        // Arrange
        
        var attackCard = new CardDefinition { Id = 1, Attack = 5, Health = 10 };
        var defenseCard = new CardDefinition { Id = 2, Attack = 2, Health = 11 };
        
        _field1.Add(attackCard);
        _field2.Add(defenseCard);
        
        // Act
        _match.Board.Turns = 3;
        _cardService.AttackCard(1,2,1,_match);
        
        // Assert 
        Assert.Equal(8, attackCard.Health);
        Assert.Equal(6, defenseCard.Health);
    }

    [Fact]
    public void AttackPlayer_ShouldUpdatePlayerHealth()
    {
        // Arrange 
        
        var card = new CardDefinition {Id = 1, Health = 10, Attack = 5};
        _field1.Add(card);
        

        _match.Board.Turns = 3;// Act
        var actual = _cardService.AttackPlayer(1, 1, _match);
        
        // Assert
        Assert.Equal(5, actual.Health);
    }

    [Fact]
    public void AttackPlayer_ShouldUpdatePlayerHealthToZero()
    {
        // Arrange 

        var card = new CardDefinition { Id = 1, Health = 1, Attack = 11 };
        _field1.Add(card);

        // Act
        _match.Board.Turns = 3;
        var actual = _cardService.AttackPlayer(1, 1, _match);

        // Assert
        Assert.Equal(0, actual.Health);
    }

    [Fact]
    public void CardDeath_ShouldRemoveCardFromFieldAndMoveToGraveyard()
    {
        var card = new CardDefinition {Id = 1, Attack = 1, Health = 0, Name = "Human"};
        _field1.Add(card);
        
        var method = typeof(CardService).GetMethod("CardDeath", BindingFlags.NonPublic | BindingFlags.Instance);
        method.Invoke(_cardService, new object[] { card, _field1, _graveyard1 });
        
        Assert.Empty(_field1);
        Assert.Single(_graveyard1);
        Assert.Equal(card, _graveyard1[0]);
    }

    [Fact]
    public void DisplayHand_ShouldReturnHand()
    {
        // Arrange
        _gameManager.StartMatch(1);
        var match = _matchService.GetMatch();
    
        // Create unique cards for players
        var card1 = new CardDefinition { Id = 1, Attack = 1, Health = 0, Name = "Human" };
        var card2 = new CardDefinition { Id = 2, Attack = 3, Health = 2, Name = "Elf" };

        // Clear hands to avoid any pre-existing cards
        match.Player1.Hand.Clear();
        match.Player2.Hand.Clear();

        // Add cards to players' hands
        match.Player1.Hand.Add(card1);
        match.Player2.Hand.Add(card2);
    
        // Act
        var player1Hand = _matchService.GetPlayerHand(match.Player1.Id);
        var player2Hand = _matchService.GetPlayerHand(match.Player2.Id);

        var actualP1 = player1Hand[0];
        var actualP2 = player2Hand[0];
    
        // Assert
        Assert.Equal(card1.Name, actualP1.Name);
        Assert.Equal(card2.Name, actualP2.Name);
    }
}