using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TCGGAPI.Models;
using TCGGAPI.Services;

namespace TCGGAPI;

public class GameManager : IGameManager
{
    private readonly IMatchService _matchService;

    public GameManager(IMatchService matchService)
    {
        _matchService = matchService;
    }

    public Match GetMatch()
    {
        return _matchService.GetMatch();
    }

    public Board GetBoard()
    {
        return _matchService.GetBoard();
    }

    public void StartTurn(int playerId)
    {
        _matchService.EnsureValidTurn(playerId);
        _matchService.StartTurn(playerId);
    }

    private bool IsCoinTossResult(int result)
    {
        return result == 1 || result == 0;
    }
    
    public string StartMatch(int coinToss)
    {
        var result = IsCoinTossResult(coinToss);
        if (!result)
        {
            return "Invalid coin toss result. Must be 0 or 1.";
        }

        _matchService.StartMatch(coinToss);
        return "Match started successfully.";
    }

    public void RestartMatch(int coinToss)
    {
        _matchService.RestartMatch(coinToss);
    }
    
    public void EndTurn(int playerId)
    {
        _matchService.EnsureValidTurn(playerId);
        _matchService.EndTurn(playerId);
    }

    public CardDefinition DrawCard(int playerId)
    {
        _matchService.EnsureValidTurn(playerId);
        return _matchService.DrawCard(playerId);
    }

    public CardDefinition DrawRandomCard(int playerId)
    {
        _matchService.EnsureValidTurn(playerId);
        return _matchService.DrawRandomCard(playerId);
    }

    public List<CardDefinition> DrawMultipleCards(int playerId, int amount)
    {
        _matchService.EnsureValidTurn(playerId);
        return _matchService.DrawMultipleCards(playerId, amount);
    }
    
    public List<CardDefinition> GetHand(int playerId) => _matchService.GetPlayerHand(playerId);

    public void AttackCard(int attackCardId, int defenseCardId, int playerId)
    {
        _matchService.EnsureValidTurn(playerId);
        _matchService.AttackCard(attackCardId, defenseCardId, playerId);
    }


    public Player AttackPlayer(int playerId, int cardId)
    {
        _matchService.EnsureValidTurn(playerId);
        return _matchService.AttackPlayer(playerId, cardId);
    }

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