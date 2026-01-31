using Microsoft.AspNetCore.Mvc;
using Sudoku.Api.Models;
using Sudoku.Application.Interfaces;

namespace Sudoku.Api.Controllers
{
    [Route("api/players/{alias}/games/{gameId}/possible-values")]
    [ApiController]
    public class PossibleValuesController(IGameApplicationService gameService) : BaseGameController(gameService)
    {
        /// <summary>
        /// Adds a possible value to a cell
        /// </summary>
        /// <param name="alias">The player's alias</param>
        /// <param name="gameId">The game id</param>
        /// <param name="request">The possible value request</param>
        /// <returns>Success or failure result</returns>
        [HttpPost("")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> AddPossibleValueAsync(string alias, string gameId, [FromBody] PossibleValueRequest request)
        {
            var (game, error) = await GetAuthorizedGameAsync(alias, gameId);
            if (error != null) return error;

            var result = await GameService.AddPossibleValueAsync(gameId, request.Row, request.Column, request.Value);
            return HandleUnitResult(result);
        }

        /// <summary>
        /// Clears all possible values from a cell
        /// </summary>
        /// <param name="alias">The player's alias</param>
        /// <param name="gameId">The game id</param>
        /// <param name="request">The cell request</param>
        /// <returns>Success or failure result</returns>
        [HttpDelete("clear")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> ClearPossibleValuesAsync(string alias, string gameId, [FromBody] CellRequest request)
        {
            var (game, error) = await GetAuthorizedGameAsync(alias, gameId);
            if (error != null) return error;

            var result = await GameService.ClearPossibleValuesAsync(gameId, request.Row, request.Column);
            return HandleUnitResult(result);
        }

        /// <summary>
        /// Removes a possible value from a cell
        /// </summary>
        /// <param name="alias">The player's alias</param>
        /// <param name="gameId">The game id</param>
        /// <param name="request">The possible value request</param>
        /// <returns>Success or failure result</returns>
        [HttpDelete("")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> RemovePossibleValueAsync(string alias, string gameId, [FromBody] PossibleValueRequest request)
        {
            var (game, error) = await GetAuthorizedGameAsync(alias, gameId);
            if (error != null) return error;

            var result = await GameService.RemovePossibleValueAsync(gameId, request.Row, request.Column, request.Value);
            return HandleUnitResult(result);
        }
    }
}
