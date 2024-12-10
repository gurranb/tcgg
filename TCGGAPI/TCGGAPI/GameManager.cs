using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TCGGAPI.Models;
using TCGGAPI.Services;

namespace TCGGAPI;

public class GameManager : IGameManager
{
    private readonly IMatchService _matchService;

    // Constructor to initialize the GameManager with a match service
    public GameManager(IMatchService matchService)
    {
        _matchService = matchService;
    }

    // Retrieves the current match
    public Match GetMatch()
    {
        return _matchService.GetMatch();
    }

    // Retrieves the current game board
    public Board GetBoard()
    {
        return _matchService.GetBoard();
    }

    // Starts the turn for the specified player
    public void StartTurn(int playerId)
    {
        _matchService.EnsureValidTurn(playerId);
        _matchService.StartTurn(playerId);
    }

    // Validates the coin toss result (0 or 1)
    private bool IsCoinTossResult(int result)
    {
        return result == 1 || result == 0;
    }
    
    // Starts a new match with the specified coin toss result
    public string StartMatch(int coinToss)
    {
        if (!IsCoinTossResult(coinToss))
        {
            return "Invalid coin toss result. Must be 0 or 1.";
        }

        _matchService.StartMatch(coinToss);
        return "Match started successfully.";
    }

    // Restarts the match with the specified coin toss result
    public void RestartMatch(int coinToss)
    {
        _matchService.RestartMatch(coinToss);
    }
    
    // Ends the turn for the specified player
    public void EndTurn(int playerId)
    {
        _matchService.EnsureValidTurn(playerId);
        _matchService.EndTurn(playerId);
    }

    // Draws a card for the specified player
    public CardDefinition DrawCard(int playerId)
    {
        _matchService.EnsureValidTurn(playerId);
        return _matchService.DrawCard(playerId);
    }

    // Draws a random card for the specified player
    public CardDefinition DrawRandomCard(int playerId)
    {
        _matchService.EnsureValidTurn(playerId);
        return _matchService.DrawRandomCard(playerId);
    }

    // Draws multiple cards for the specified player
    public List<CardDefinition> DrawMultipleCards(int playerId, int amount)
    {
        _matchService.EnsureValidTurn(playerId);
        return _matchService.DrawMultipleCards(playerId, amount);
    }
    
    // Gets the player's hand of cards
    public List<CardDefinition> GetHand(int playerId) 
    {
        return _matchService.GetPlayerHand(playerId);
    }

    // Attacks a defense card with an attacking card
    public void AttackCard(int attackCardId, int defenseCardId, int playerId)
    {
        _matchService.EnsureValidTurn(playerId);
        _matchService.AttackCard(attackCardId, defenseCardId, playerId);
    }

    // Attacks a player using a specified card
    public Player AttackPlayer(int playerId, int cardId)
    {
        _matchService.EnsureValidTurn(playerId);
        return _matchService.AttackPlayer(playerId, cardId);
    }

    // Plays a specified card to the board for the player
    public void PlayCardToBoard(int playerId, int cardId)
    {
        _matchService.EnsureValidTurn(playerId);
        _matchService.PlayCardToBoard(playerId, cardId);
    }
}

public interface IGameManager
{
    Match GetMatch();
    string StartMatch(int coinToss);
    void StartTurn(int playerId);
    void RestartMatch(int coinToss);
    void EndTurn(int playerId);
    CardDefinition DrawCard(int playerId);
    List<CardDefinition> DrawMultipleCards(int playerId, int amount);
    void AttackCard(int attackCardId, int defenseCardId, int playerId);
    Player AttackPlayer(int playerId, int cardId);
    void PlayCardToBoard(int playerId, int cardId);
    List<CardDefinition> GetHand(int playerId);
    CardDefinition DrawRandomCard(int playerId);
}