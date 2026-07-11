using MediatR;
using Microsoft.AspNetCore.Mvc;
using Sudoku.Application.DTOs;
using Sudoku.Application.Queries;

namespace Sudoku.Api.Controllers;

[Route("api/players/{profileId}/stats")]
[ApiController]
public class StatsController(IMediator mediator) : BaseGameController(mediator)
{
    /// <summary>
    /// Gets aggregated game statistics for the specified player profile.
    /// </summary>
    /// <param name="profileId">The profile ID of the player</param>
    /// <returns>The player's games played, games won, win rate, and per-difficulty breakdown</returns>
    /// <remarks>A player with no games is a valid result: zeroed stats, not a 404.</remarks>
    [HttpGet]
    [ProducesResponseType(typeof(PlayerStatsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PlayerStatsDto>> GetPlayerStatsAsync(string profileId)
    {
        if (string.IsNullOrWhiteSpace(profileId))
        {
            return BadRequest("Profile ID cannot be null or empty.");
        }

        var result = await Mediator.Send(new GetPlayerStatsQuery(profileId));

        if (!result.IsSuccess)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }
}
