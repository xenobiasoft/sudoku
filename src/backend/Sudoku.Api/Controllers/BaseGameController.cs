using MediatR;
using Microsoft.AspNetCore.Mvc;
using Sudoku.Application.Common;
using Sudoku.Application.DTOs;
using Sudoku.Application.Queries;

namespace Sudoku.Api.Controllers;

public abstract class BaseGameController(IMediator mediator) : ControllerBase
{
    protected IMediator Mediator => mediator;

    protected async Task<(GameDto? game, ActionResult? error)> GetAuthorizedGameAsync(string alias, string gameId)
    {
        if (string.IsNullOrWhiteSpace(alias) || string.IsNullOrWhiteSpace(gameId))
        {
            return (null, BadRequest("Player alias and game id cannot be null or empty."));
        }

        var gameResult = await mediator.Send(new GetGameQuery(gameId));
        if (!gameResult.IsSuccess)
        {
            return (null, NotFound(gameResult.Error));
        }

        if (gameResult.Value.PlayerAlias != alias)
        {
            return (null, NotFound());
        }

        return (gameResult.Value, null);
    }

    protected ActionResult HandleUnitResult(Result result)
    {
        if (!result.IsSuccess)
        {
            return BadRequest(result.Error);
        }

        return NoContent();
    }
}
