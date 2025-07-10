using Microsoft.AspNetCore.Mvc;
using XenobiaSoft.Sudoku.Abstractions;
using XenobiaSoft.Sudoku.GameState;

namespace Sudoku.Api.Controllers;

[Route("api/players/{alias}/[controller]")]
[ApiController]
public class GamesController(IGameService gameService) : ControllerBase
{
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
        var games = await gameService.LoadGamesAsync(alias);

        return Ok(games);
    }
}