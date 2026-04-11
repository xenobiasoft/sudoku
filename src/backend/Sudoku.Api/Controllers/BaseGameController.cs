using Microsoft.AspNetCore.Mvc;
using Sudoku.Application.DTOs;
using Sudoku.Application.Interfaces;

namespace Sudoku.Api.Controllers;

public abstract class BaseGameController(IGameApplicationService gameService) : ControllerBase
{
    protected IGameApplicationService GameService => gameService;

    protected async Task<(GameDto? game, ActionResult? error)> GetAuthorizedGameAsync(string alias, string gameId)
    {
        if (string.IsNullOrWhiteSpace(alias) || string.IsNullOrWhiteSpace(gameId))
        {
            return (null, BadRequest("Player alias and game id cannot be null or empty."));
        }

        var gameResult = await GameService.GetGameAsync(gameId);
        if (!gameResult.IsSuccess)
        {
            return (null, BadRequest(gameResult.Error));
        }

        if (gameResult.Value.PlayerAlias != alias)
        {
            return (null, NotFound());
        }

        return (gameResult.Value, null);
    }

    protected ActionResult HandleUnitResult(dynamic result)
    {
        // result is expected to have IsSuccess and Error members.
        if (!result.IsSuccess)
        {
            return BadRequest(result.Error);
        }

        return NoContent();
    }
}