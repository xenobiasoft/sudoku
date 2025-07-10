using Microsoft.AspNetCore.Mvc;
using XenobiaSoft.Sudoku.Abstractions;

namespace Sudoku.Api.Controllers
{
    [Route("api/players")]
    [ApiController]
    public class PlayersController(IPlayerService playerService) : ControllerBase
    {
        /// <summary>
        /// Creates a new player
        /// </summary>
        /// <returns>The newly created player's alias</returns>
        [HttpPost]
        [ProducesResponseType(typeof(string), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<string>> PostAsync()
        {
            var alias = await playerService.CreateNewAsync();

            return CreatedAtAction(nameof(PostAsync), new { alias });
        }
    }
}
