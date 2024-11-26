using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TCGGAPI.Models;
using TCGGAPI.Services;

namespace TCGGAPI;

public class GameManager
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
    
    public void StartMatch(int coinToss)
    {
        var match = _matchService.StartMatch(coinToss);
    }

    public void RestartMatch(int coinToss)
    {
        _matchService.RestartMatch(coinToss);
    }
    
    public void EndTurn(int playerId)
    {
        _matchService.EndTurn(playerId);
    }

    public CardDefintion DrawCard(int playerId)
    {
        return _matchService.DrawCard(playerId);
    }

    public void AttackCard(int attackCardId, int defenseCardId, int playerId)
    {
        _matchService.AttackCard(attackCardId, defenseCardId, playerId);
    }


    public Player AttackPlayer(int playerId, int cardId)
    {
        return _matchService.AttackPlayer(playerId, cardId);
    }

    public void PlayCardToBoard(int playerId, int cardId)
    {
        _matchService.PlayCardToBoard(playerId, cardId);
    }
}