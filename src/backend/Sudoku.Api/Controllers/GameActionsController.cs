using MediatR;
using Microsoft.AspNetCore.Mvc;
using Sudoku.Api.Models;
using Sudoku.Application.Commands;

namespace Sudoku.Api.Controllers
{
    [Route("api/players/{profileId}/games/{gameId}/actions")]
    [ApiController]
    public class GameActionsController(IMediator mediator) : BaseGameController(mediator)
    {
        /// <summary>
        /// Updates a game (makes a move)
        /// </summary>
        /// <param name="profileId">The player's profile ID</param>
        /// <param name="gameId">The game id</param>
        /// <param name="move">The move to make</param>
        /// <returns>Success or failure result</returns>
        [HttpPut("")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> MakeMoveAsync(string profileId, string gameId, [FromBody] MoveRequest move)
        {
            var (_, error) = await GetAuthorizedGameAsync(profileId, gameId);
            if (error != null) return error;

            var result = await Mediator.Send(new MakeMoveCommand(gameId, move.Row, move.Column, move.Value, move.PlayDuration));
            return HandleUnitResult(result);
        }

        /// <summary>
        /// Resets a game to its initial state
        /// </summary>
        /// <param name="profileId">The player's profile ID</param>
        /// <param name="gameId">The game id</param>
        /// <returns>No content if successful</returns>
        [HttpPost("reset")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> ResetGameAsync(string profileId, string gameId)
        {
            var (_, error) = await GetAuthorizedGameAsync(profileId, gameId);
            if (error != null) return error;

            var result = await Mediator.Send(new ResetGameCommand(gameId));
            return HandleUnitResult(result);
        }

        /// <summary>
        /// Undoes the last move in a game
        /// </summary>
        /// <param name="profileId">The player's profile ID</param>
        /// <param name="gameId">The game id</param>
        /// <returns>No content if successful</returns>
        [HttpPost("undo")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> UndoMoveAsync(string profileId, string gameId)
        {
            var (_, error) = await GetAuthorizedGameAsync(profileId, gameId);
            if (error != null) return error;

            var result = await Mediator.Send(new UndoLastMoveCommand(gameId));
            return HandleUnitResult(result);
        }
    }
}
