using MediatR;
using Microsoft.AspNetCore.Mvc;
using Sudoku.Api.Models;
using Sudoku.Application.Commands;

namespace Sudoku.Api.Controllers
{
    [Route("api/players/{alias}/games/{gameId}/possible-values")]
    [ApiController]
    public class PossibleValuesController(IMediator mediator) : BaseGameController(mediator)
    {
        /// <summary>
        /// Adds a possible value to a cell
        /// </summary>
        [HttpPost("")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> AddPossibleValueAsync(string alias, string gameId, [FromBody] PossibleValueRequest request)
        {
            var (_, error) = await GetAuthorizedGameAsync(alias, gameId);
            if (error != null) return error;

            var result = await Mediator.Send(new AddPossibleValueCommand(gameId, request.Row, request.Column, request.Value));
            return HandleUnitResult(result);
        }

        /// <summary>
        /// Clears all possible values from a cell
        /// </summary>
        [HttpDelete("clear")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> ClearPossibleValuesAsync(string alias, string gameId, [FromBody] CellRequest request)
        {
            var (_, error) = await GetAuthorizedGameAsync(alias, gameId);
            if (error != null) return error;

            var result = await Mediator.Send(new ClearPossibleValuesCommand(gameId, request.Row, request.Column));
            return HandleUnitResult(result);
        }

        /// <summary>
        /// Removes a possible value from a cell
        /// </summary>
        [HttpDelete("")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> RemovePossibleValueAsync(string alias, string gameId, [FromBody] PossibleValueRequest request)
        {
            var (_, error) = await GetAuthorizedGameAsync(alias, gameId);
            if (error != null) return error;

            var result = await Mediator.Send(new RemovePossibleValueCommand(gameId, request.Row, request.Column, request.Value));
            return HandleUnitResult(result);
        }
    }
}
