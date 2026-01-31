using Microsoft.AspNetCore.Mvc;
using Sudoku.Application.DTOs;
using Sudoku.Application.Interfaces;

namespace Sudoku.Api.Controllers
{
    [Route("api/players/{alias}/games/{gameId}/status")]
    [ApiController]
    public class GameStatusController(IGameApplicationService gameService) : BaseGameController(gameService)
    {
        /// <summary>
        /// Updates the status of a game for the specified player.
        /// </summary>
        /// <param name="alias">The alias of the player associated with the game. Cannot be null or empty.</param>
        /// <param name="gameId">The unique identifier of the game to update. Cannot be null or empty.</param>
        /// <param name="gameStatus">The new status to set for the game.</param>
        /// <returns>An <see cref="ActionResult"/> indicating the result of the operation. Returns 204 No Content if the update is
        /// successful, 400 Bad Request if the input is invalid or the update fails, or 404 Not Found if the game does not
        /// exist or does not belong to the specified player.</returns>
        [HttpPatch("{gameStatus}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> UpdateGameStatusAsync(string alias, string gameId, string gameStatus)
        {
            var (game, error) = await GetAuthorizedGameAsync(alias, gameId);
            if (error != null) return error;

            var result = await GameService.UpdateGameStatusAsync(gameId, gameStatus);
            return HandleUnitResult(result);
        }

        /// <summary>
        /// Validates a game to check if it's completed correctly
        /// </summary>
        /// <param name="alias">The player's alias</param>
        /// <param name="gameId">The game id</param>
        /// <returns>Result of the validation</returns>
        [HttpPost("validate")]
        [ProducesResponseType(typeof(ValidationResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ValidationResultDto>> ValidateGameAsync(string alias, string gameId)
        {
            var (_, error) = await GetAuthorizedGameAsync(alias, gameId);
            if (error != null) return error;

            var result = await GameService.ValidateGameAsync(gameId);

            if (!result.IsSuccess)
            {
                return BadRequest(result.Error);
            }

            return Ok(result.Value);
        }
    }
}
