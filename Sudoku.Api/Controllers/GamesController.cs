using Microsoft.AspNetCore.Mvc;
using XenobiaSoft.Sudoku.GameState;
using XenobiaSoft.Sudoku.Services;

namespace Sudoku.Api.Controllers;

[Route("api/players/{alias}/[controller]")]
[ApiController]
public class GamesController(IGameService gameService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<GameStateMemory>>> GetAsync(string alias)
    {
        var games = await gameService.GetGamesForPlayerAsync(alias);

        return Ok(games);
    }
}