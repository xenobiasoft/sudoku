using MediatR;
using Microsoft.AspNetCore.Mvc;
using Sudoku.Application.Commands;
using Sudoku.Application.DTOs;
using Sudoku.Application.Queries;

namespace Sudoku.Api.Controllers
{
    [Route("api/players/{alias}/games/{gameId}/status")]
    [ApiController]
    public class GameStatusController(IMediator mediator) : BaseGameController(mediator)
    {
        /// <summary>
        /// Pauses an in-progress game.
        /// </summary>
        [HttpPost("pause")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> PauseGameAsync(string alias, string gameId)
        {
            var (_, error) = await GetAuthorizedGameAsync(alias, gameId);
            if (error != null) return error;

            return HandleUnitResult(await Mediator.Send(new PauseGameCommand(gameId)));
        }

        /// <summary>
        /// Resumes a paused game.
        /// </summary>
        [HttpPost("resume")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> ResumeGameAsync(string alias, string gameId)
        {
            var (_, error) = await GetAuthorizedGameAsync(alias, gameId);
            if (error != null) return error;

            return HandleUnitResult(await Mediator.Send(new ResumeGameCommand(gameId)));
        }

        /// <summary>
        /// Abandons a game.
        /// </summary>
        [HttpPost("abandon")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> AbandonGameAsync(string alias, string gameId)
        {
            var (_, error) = await GetAuthorizedGameAsync(alias, gameId);
            if (error != null) return error;

            return HandleUnitResult(await Mediator.Send(new AbandonGameCommand(gameId)));
        }

        /// <summary>
        /// Marks a game as complete.
        /// </summary>
        [HttpPost("complete")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> CompleteGameAsync(string alias, string gameId)
        {
            var (_, error) = await GetAuthorizedGameAsync(alias, gameId);
            if (error != null) return error;

            return HandleUnitResult(await Mediator.Send(new CompleteGameCommand(gameId)));
        }

        /// <summary>
        /// Validates a game to check if it is completed correctly.
        /// </summary>
        [HttpPost("validate")]
        [ProducesResponseType(typeof(ValidationResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ValidationResultDto>> ValidateGameAsync(string alias, string gameId)
        {
            var (_, error) = await GetAuthorizedGameAsync(alias, gameId);
            if (error != null) return error;

            var result = await Mediator.Send(new ValidateGameQuery(gameId));

            if (!result.IsSuccess)
            {
                return BadRequest(result.Error);
            }

            return Ok(result.Value);
        }
    }
}
