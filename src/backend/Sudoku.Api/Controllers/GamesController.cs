using MediatR;
using Microsoft.AspNetCore.Mvc;
using Sudoku.Api.Models;
using Sudoku.Application.Commands;
using Sudoku.Application.DTOs;
using Sudoku.Application.Queries;

namespace Sudoku.Api.Controllers;

[Route("api/players/{profileId}/games")]
[ApiController]
public class GamesController(IMediator mediator) : BaseGameController(mediator)
{
    /// <summary>
    /// Creates a new game for the specified player profile with the given difficulty.
    /// </summary>
    /// <param name="profileId">The profile ID of the player</param>
    /// <param name="difficulty">The difficulty level of the game</param>
    /// <returns>Location of the created game</returns>
    [HttpPost("{difficulty}")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> CreateGameAsync(string profileId, string difficulty)
    {
        if (string.IsNullOrWhiteSpace(profileId) || string.IsNullOrWhiteSpace(difficulty))
        {
            return BadRequest("Profile ID and difficulty cannot be null or empty.");
        }

        var profileResult = await Mediator.Send(new GetProfileByIdQuery(profileId));
        if (!profileResult.IsSuccess || profileResult.Value == null)
        {
            return NotFound($"Profile '{profileId}' not found.");
        }

        var result = await Mediator.Send(new CreateGameCommand(profileId, profileResult.Value.Alias, difficulty));

        if (!result.IsSuccess)
        {
            return BadRequest(result.Error);
        }

        return Created($"/api/players/{profileId}/games/{result.Value}", null);
    }

    /// <summary>
    /// Deletes all games for a player
    /// </summary>
    /// <param name="profileId">The player's profile ID</param>
    /// <returns>No content if successful</returns>
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> DeleteAllGamesAsync(string profileId)
    {
        if (string.IsNullOrWhiteSpace(profileId))
        {
            return BadRequest("Profile ID cannot be null or empty.");
        }

        var result = await Mediator.Send(new DeletePlayerGamesCommand(profileId));
        return HandleUnitResult(result);
    }

    /// <summary>
    /// Deletes a specific game for a player
    /// </summary>
    /// <param name="profileId">The player's profile ID</param>
    /// <param name="gameId">The game id to delete</param>
    /// <returns>No content if successful</returns>
    [HttpDelete("{gameId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteGameAsync(string profileId, string gameId)
    {
        var (_, error) = await GetAuthorizedGameAsync(profileId, gameId);
        if (error != null) return error;

        var result = await Mediator.Send(new DeleteGameCommand(gameId));
        return HandleUnitResult(result);
    }

    /// <summary>
    /// Gets all games for a specific player
    /// </summary>
    /// <param name="profileId">The player's profile ID</param>
    /// <returns>A list of games for the player</returns>
    [HttpGet]
    [ProducesResponseType(typeof(List<GameDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<GameDto>>> GetAllGamesAsync(string profileId)
    {
        if (string.IsNullOrWhiteSpace(profileId))
        {
            return BadRequest("Profile ID cannot be null or empty.");
        }

        var result = await Mediator.Send(new GetPlayerGamesQuery(profileId));

        if (!result.IsSuccess)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Gets a specific game by id for a player
    /// </summary>
    /// <param name="profileId">The player's profile ID</param>
    /// <param name="gameId">The game id</param>
    /// <returns>The game if found</returns>
    [HttpGet("{gameId}")]
    [ProducesResponseType(typeof(GameDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GameDto>> GetGameAsync(string profileId, string gameId)
    {
        var (game, error) = await GetAuthorizedGameAsync(profileId, gameId);
        if (error != null) return error;

        return Ok(game);
    }
}
