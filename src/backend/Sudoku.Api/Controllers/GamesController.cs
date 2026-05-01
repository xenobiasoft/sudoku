using MediatR;
using Microsoft.AspNetCore.Mvc;
using Sudoku.Api.Models;
using Sudoku.Application.Commands;
using Sudoku.Application.DTOs;
using Sudoku.Application.Queries;

namespace Sudoku.Api.Controllers;

[Route("api/players/{alias}/games")]
[ApiController]
public class GamesController(IMediator mediator) : BaseGameController(mediator)
{
    /// <summary>
    /// Creates a new game for the specified player with the given difficulty.
    /// </summary>
    /// <param name="alias">The alias of the player</param>
    /// <param name="difficulty">The difficulty level of the game</param>
    /// <returns>Location of the created game</returns>
    [HttpPost("{difficulty}")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> CreateGameAsync(string alias, string difficulty)
    {
        if (string.IsNullOrWhiteSpace(alias) || string.IsNullOrWhiteSpace(difficulty))
        {
            return BadRequest("Player alias and difficulty cannot be null or empty.");
        }

        var result = await Mediator.Send(new CreateGameCommand(alias, difficulty));

        if (!result.IsSuccess)
        {
            return BadRequest(result.Error);
        }

        return Created($"/api/players/{alias}/games/{result.Value}", null);
    }

    /// <summary>
    /// Deletes all games for a player
    /// </summary>
    /// <param name="alias">The player's alias</param>
    /// <returns>No content if successful</returns>
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> DeleteAllGamesAsync(string alias)
    {
        if (string.IsNullOrWhiteSpace(alias))
        {
            return BadRequest("Player alias cannot be null or empty.");
        }

        var result = await Mediator.Send(new DeletePlayerGamesCommand(alias));
        return HandleUnitResult(result);
    }

    /// <summary>
    /// Deletes a specific game for a player
    /// </summary>
    /// <param name="alias">The player's alias</param>
    /// <param name="gameId">The game id to delete</param>
    /// <returns>No content if successful</returns>
    [HttpDelete("{gameId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteGameAsync(string alias, string gameId)
    {
        var (_, error) = await GetAuthorizedGameAsync(alias, gameId);
        if (error != null) return error;

        var result = await Mediator.Send(new DeleteGameCommand(gameId));
        return HandleUnitResult(result);
    }

    /// <summary>
    /// Gets all games for a specific player
    /// </summary>
    /// <param name="alias">The player's alias</param>
    /// <returns>A list of games for the player</returns>
    [HttpGet]
    [ProducesResponseType(typeof(List<GameDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<GameDto>>> GetAllGamesAsync(string alias)
    {
        if (string.IsNullOrWhiteSpace(alias))
        {
            return BadRequest("Player alias cannot be null or empty.");
        }

        var result = await Mediator.Send(new GetPlayerGamesQuery(alias));

        if (!result.IsSuccess)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Gets a specific game by id for a player
    /// </summary>
    /// <param name="alias">The player's alias</param>
    /// <param name="gameId">The game id</param>
    /// <returns>The game if found</returns>
    [HttpGet("{gameId}")]
    [ProducesResponseType(typeof(GameDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GameDto>> GetGameAsync(string alias, string gameId)
    {
        var (game, error) = await GetAuthorizedGameAsync(alias, gameId);
        if (error != null) return error;

        return Ok(game);
    }
}
