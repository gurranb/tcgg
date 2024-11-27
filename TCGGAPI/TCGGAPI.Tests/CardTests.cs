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
    private List<CardDefintion> _field1;
    private List<CardDefintion> _field2;
    private List<CardDefintion> _graveyard1;
    private List<CardDefintion> _graveyard2;

    

    public CardTests()
    {
        _cardService = new CardService();

        _matchService = new MatchService(_cardService);

        _gameManager = new GameManager(_matchService);
        
        _player1 = new Player {Id = 1, Name = "Player 1", Health = 10, Graveyard = _graveyard1, MatchDeck = _deck};
        _player2 = new Player {Id = 2, Name = "Player 2", Health = 10, Graveyard = _graveyard2, MatchDeck = _deck};
        
        _deck = new Deck();
        _deck.Cards = new List<CardDefintion>
        {
            new CardDefintion { Id = 1, Attack = 1, Health = 1, Rarity = 0, Name = "Human" },
            new CardDefintion { Id = 2, Attack = 2, Health = 2, Rarity = 0, Name = "Beast" },
        };

        _field1 = new List<CardDefintion>();
        _field2 = new List<CardDefintion>();
        _graveyard1 = new List<CardDefintion>();
        _graveyard2 = new List<CardDefintion>();

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
    public void DrawCard_ShouldReturnCard_WhenCardExist()
    {
        _gameManager.StartMatch(1);
        var match = _matchService.GetMatch();
        var playerId = match.Player1.Id;
    
        var drawnCard = _matchService.DrawCard(playerId);
        
        var card = _cardService.GetCard(playerId, match);
        
        Assert.Equal(drawnCard.Name, card.Name);
        Assert.Equal(drawnCard.Id, card.Id);
    }

    // [Fact]
    // public void DrawCard_ShouldThrowException_WhenCardDoesNotExist()
    // {
    //     // Arrange
    //     var mockGameManager = new Mock<IGameManager>();
    //     var mockMatchService = new Mock<IMatchService>();
    //     var mockCardService = new Mock<ICardService>();
    //     
    //     var playerId = 1;
    //     var card = new CardDefintion();
    //     var match = new Match();
    //
    //     mockGameManager.Setup(gm => gm.StartMatch(It.IsAny<int>()));
    //     mockMatchService.Setup(ms => ms.GetMatch()).Returns(match);
    //     mockGameManager.Setup(gm => gm.DrawCard(playerId)).Returns(card);
    //     mockCardService.Setup(cs => cs.GetCard(playerId, match)).Returns(card);
    //     
    //     // Act
    //     mockGameManager.Object.StartMatch(1);
    //     var drawnCard = mockGameManager.Object.DrawCard(playerId);
    //     var retrievedCard = mockCardService.Object.GetCard(playerId, match);
    //     
    //     // Assert
    //     Assert.Equal(drawnCard.Name, retrievedCard.Name);
    //     Assert.Equal(drawnCard.Id, retrievedCard.Id);
    // }

    [Fact]
    public void AttackCard_ShouldUpdateCardHealth()
    {
        // Arrange
        
        var attackCard = new CardDefintion { Id = 1, Attack = 5, Health = 10 };
        var defenseCard = new CardDefintion { Id = 2, Attack = 2, Health = 11 };
        
        _field1.Add(attackCard);
        _field2.Add(defenseCard);
        
        // Act
        _cardService.AttackCard(1,2,1,_match);
        
        // Assert 
        Assert.Equal(8, attackCard.Health);
        Assert.Equal(6, defenseCard.Health);
    }

    [Fact]
    public void AttackPlayer_ShouldUpdatePlayerHealth()
    {
        // Arrange 
        
        var card = new CardDefintion {Id = 1, Health = 10, Attack = 5};
        _field1.Add(card);
        
        // Act
        var actual = _cardService.AttackPlayer(1, 1, _match);
        
        // Assert
        Assert.Equal(5, actual.Health);
    }

    [Fact]
    public void AttackPlayer_ShouldUpdatePlayerHealthToZero()
    {
        // Arrange 

        var card = new CardDefintion { Id = 1, Health = 1, Attack = 11 };
        _field1.Add(card);

        // Act
        var actual = _cardService.AttackPlayer(1, 1, _match);

        // Assert
        Assert.Equal(0, actual.Health);
    }

    [Fact]
    public void CardDeath_ShouldRemoveCardFromFieldAndMoveToGraveyard()
    {
        var card = new CardDefintion {Id = 1, Attack = 1, Health = 0, Name = "Human"};
        _field1.Add(card);
        
        var method = typeof(CardService).GetMethod("CardDeath", BindingFlags.NonPublic | BindingFlags.Instance);
        method.Invoke(_cardService, new object[] { card, _field1, _graveyard1 });
        
        Assert.Empty(_field1);
        Assert.Single(_graveyard1);
        Assert.Equal(card, _graveyard1[0]);
    }
}