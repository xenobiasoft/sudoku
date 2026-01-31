using Microsoft.AspNetCore.Mvc;
using Sudoku.Api.Models;
using Sudoku.Application.Interfaces;

namespace Sudoku.Api.Controllers
{
    [Route("api/players/{alias}/games/{gameId}")]
    [ApiController]
    public class GameActionsController(IGameApplicationService gameService) : BaseGameController(gameService)
    {
        /// <summary>
        /// Updates a game (makes a move)
        /// </summary>
        /// <param name="alias">The player's alias</param>
        /// <param name="gameId">The game id</param>
        /// <param name="move">The move to make</param>
        /// <returns>Success or failure result</returns>
        [HttpPut("")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> MakeMoveAsync(string alias, string gameId, [FromBody] MoveRequest move)
        {
            var (_, error) = await GetAuthorizedGameAsync(alias, gameId);
            if (error != null) return error;

            var result = await GameService.MakeMoveAsync(gameId, move.Row, move.Column, move.Value, move.PlayDuration);
            return HandleUnitResult(result);
        }

        /// <summary>
        /// Resets a game to its initial state
        /// </summary>
        /// <param name="alias">The player's alias</param>
        /// <param name="gameId">The game id</param>
        /// <returns>No content if successful</returns>
        [HttpPost("reset")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> ResetGameAsync(string alias, string gameId)
        {
            var (game, error) = await GetAuthorizedGameAsync(alias, gameId);
            if (error != null) return error;

            var result = await GameService.ResetGameAsync(gameId);
            return HandleUnitResult(result);
        }

        /// <summary>
        /// Undoes the last move in a game
        /// </summary>
        /// <param name="alias">The player's alias</param>
        /// <param name="gameId">The game id</param>
        /// <returns>No content if successful</returns>
        [HttpPost("undo")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> UndoMoveAsync(string alias, string gameId)
        {
            var (game, error) = await GetAuthorizedGameAsync(alias, gameId);
            if (error != null) return error;

            var result = await GameService.UndoLastMoveAsync(gameId);
            return HandleUnitResult(result);
        }
    }
}
