using Microsoft.AspNetCore.Mvc;
using Sudoku.Api.Models;
using Sudoku.Application.DTOs;
using Sudoku.Application.Interfaces;

namespace Sudoku.Api.Controllers;

[Route("api/players/{alias}/games")]
[ApiController]
public class GamesController(IGameApplicationService gameService) : BaseGameController(gameService)
{
    /// <summary>
    /// Creates a new game for the specified player with the given difficulty.
    /// </summary>
    /// <param name="alias">The alias of the player</param>
    /// <param name="difficulty">The difficulty level of the game</param>
    /// <returns>The created game</returns>
    [HttpPost("{difficulty}")]
    [ProducesResponseType(typeof(GameDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<GameDto>> CreateGameAsync(string alias, string difficulty)
    {
        if (string.IsNullOrWhiteSpace(alias) || string.IsNullOrWhiteSpace(difficulty))
        {
            return BadRequest("Player alias and difficulty cannot be null or empty.");
        }

        var result = await GameService.CreateGameAsync(alias, difficulty);

        if (!result.IsSuccess)
        {
            return BadRequest(result.Error);
        }

        return Created($"/api/players/{alias}/games/{result.Value.Id}", result.Value);
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

        var result = await GameService.DeletePlayerGamesAsync(alias);

        if (!result.IsSuccess)
        {
            return BadRequest(result.Error);
        }

        return NoContent();
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
        var (game, error) = await GetAuthorizedGameAsync(alias, gameId);
        if (error != null) return error;

        var result = await GameService.DeleteGameAsync(gameId);
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

        var result = await GameService.GetPlayerGamesAsync(alias);

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

        // We already fetched the game in GetAuthorizedGameAsync, so return it directly.
        return Ok(game);
    }
}
