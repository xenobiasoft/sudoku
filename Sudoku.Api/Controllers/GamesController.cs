using Microsoft.AspNetCore.Mvc;
using XenobiaSoft.Sudoku.Abstractions;
using XenobiaSoft.Sudoku.GameState;

namespace Sudoku.Api.Controllers;

[Route("api/players/{alias}/[controller]")]
[ApiController]
public class GamesController(IGameService gameService) : ControllerBase
{
    /// <summary>
    /// Deletes the games associated with the specified player alias.
    /// </summary>
    /// <param name="alias">The alias of the player whose games are to be deleted. Cannot be null or empty.</param>
    /// <param name="gameId">The identifier of the game to be deleted.</param>
    /// <returns>An <see cref="IActionResult"/> indicating the result of the operation. Returns <see
    /// cref="StatusCodes.Status204NoContent"/> if the deletion is successful, or <see
    /// cref="StatusCodes.Status400BadRequest"/> if the alias, or the game id is null or empty.</returns>
    [HttpDelete("{gameId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> DeleteAsync(string alias, string gameId)
    {
        if (string.IsNullOrWhiteSpace(alias) || string.IsNullOrWhiteSpace(gameId))
        {
            return BadRequest("Player alias and game Id cannot be null or empty.");
        }

        await gameService.DeleteGameAsync(alias, gameId);
        return NoContent();
    }

    /// <summary>
    /// Gets all games for a specific player
    /// </summary>
    /// <param name="alias">The player's alias</param>
    /// <returns>A list of game states for the player</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<GameStateMemory>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IEnumerable<GameStateMemory>>> GetAsync(string alias)
    {
        if (string.IsNullOrWhiteSpace(alias))
        {
            return BadRequest("Player alias cannot be null or empty.");
        }

        var games = await gameService.LoadGamesAsync(alias);

        return Ok(games);
    }

    /// <summary>
    /// Retrieves the current game state for a specified player and game.
    /// </summary>
    /// <param name="alias">The alias of the player. Cannot be null or empty.</param>
    /// <param name="gameId">The unique identifier of the game. Cannot be null or empty.</param>
    /// <returns>An <see cref="ActionResult{T}"/> containing the <see cref="GameStateMemory"/> if the request is successful.
    /// Returns a 200 OK status with the game state, or a 400 Bad Request status if the alias or gameId is invalid.</returns>
    [HttpGet("{gameId}")]
    [ProducesResponseType(typeof(GameStateMemory), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<GameStateMemory>> GetAsync(string alias, string gameId)
    {
        if (string.IsNullOrWhiteSpace(alias) || string.IsNullOrWhiteSpace(gameId))
        {
            return BadRequest("Player alias and game Id cannot be null or empty.");
        }
        var game = await gameService.LoadGameAsync(alias, gameId);

        if (game == null)
        {
            return NotFound($"Game with ID '{gameId}' for player '{alias}' not found.");
        }
        return Ok(game);
    }
}