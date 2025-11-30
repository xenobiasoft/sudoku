using Microsoft.AspNetCore.Mvc;
using Sudoku.Api.Models;
using Sudoku.Application.DTOs;
using Sudoku.Application.Interfaces;

namespace Sudoku.Api.Controllers;

[Route("api/players/{alias}/games")]
[ApiController]
public class GamesController(IGameApplicationService gameService) : ControllerBase
{
    /// <summary>
    /// Adds a possible value to a cell
    /// </summary>
    /// <param name="alias">The player's alias</param>
    /// <param name="gameId">The game id</param>
    /// <param name="request">The possible value request</param>
    /// <returns>Success or failure result</returns>
    [HttpPost("{gameId}/possible-values")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> AddPossibleValueAsync(string alias, string gameId, [FromBody] PossibleValueRequest request)
    {
        var (game, error) = await GetAuthorizedGameAsync(alias, gameId);
        if (error != null) return error;

        var result = await gameService.AddPossibleValueAsync(gameId, request.Row, request.Column, request.Value);
        return HandleUnitResult(result);
    }

    /// <summary>
    /// Clears all possible values from a cell
    /// </summary>
    /// <param name="alias">The player's alias</param>
    /// <param name="gameId">The game id</param>
    /// <param name="request">The cell request</param>
    /// <returns>Success or failure result</returns>
    [HttpDelete("{gameId}/possible-values/clear")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> ClearPossibleValuesAsync(string alias, string gameId, [FromBody] CellRequest request)
    {
        var (game, error) = await GetAuthorizedGameAsync(alias, gameId);
        if (error != null) return error;

        var result = await gameService.ClearPossibleValuesAsync(gameId, request.Row, request.Column);
        return HandleUnitResult(result);
    }

    /// <summary>
    /// Creates a new game for the specified player with the given difficulty.
    /// </summary>
    /// <param name="alias">The alias of the player</param>
    /// <param name="difficulty">The difficulty level of the game</param>
    /// <returns>The created game</returns>
    [HttpPost("{difficulty}")]
    [ProducesResponseType(typeof(GameDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<GameDto>> CreateGameAsync(string alias, string difficulty)
    {
        if (string.IsNullOrWhiteSpace(alias) || string.IsNullOrWhiteSpace(difficulty))
        {
            return BadRequest("Player alias and difficulty cannot be null or empty.");
        }

        var result = await gameService.CreateGameAsync(alias, difficulty);

        if (!result.IsSuccess)
        {
            return BadRequest(result.Error);
        }

        return Created($"/api/players/{alias}/games/{result.Value.Id}", result.Value);
    }

    /// <summary>
    /// Deletes all games for a player
    /// </summary>
    /// <param name="alias">The player's alias</param>
    /// <returns>No content if successful</returns>
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> DeleteAllGamesAsync(string alias)
    {
        if (string.IsNullOrWhiteSpace(alias))
        {
            return BadRequest("Player alias cannot be null or empty.");
        }

        var result = await gameService.DeletePlayerGamesAsync(alias);

        if (!result.IsSuccess)
        {
            return BadRequest(result.Error);
        }

        return NoContent();
    }

    /// <summary>
    /// Deletes a specific game for a player
    /// </summary>
    /// <param name="alias">The player's alias</param>
    /// <param name="gameId">The game id to delete</param>
    /// <returns>No content if successful</returns>
    [HttpDelete("{gameId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteGameAsync(string alias, string gameId)
    {
        var (game, error) = await GetAuthorizedGameAsync(alias, gameId);
        if (error != null) return error;

        var result = await gameService.DeleteGameAsync(gameId);
        return HandleUnitResult(result);
    }

    /// <summary>
    /// Gets all games for a specific player
    /// </summary>
    /// <param name="alias">The player's alias</param>
    /// <returns>A list of games for the player</returns>
    [HttpGet]
    [ProducesResponseType(typeof(List<GameDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<GameDto>>> GetAllGamesAsync(string alias)
    {
        if (string.IsNullOrWhiteSpace(alias))
        {
            return BadRequest("Player alias cannot be null or empty.");
        }

        var result = await gameService.GetPlayerGamesAsync(alias);

        if (!result.IsSuccess)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Gets a specific game by id for a player
    /// </summary>
    /// <param name="alias">The player's alias</param>
    /// <param name="gameId">The game id</param>
    /// <returns>The game if found</returns>
    [HttpGet("{gameId}")]
    [ProducesResponseType(typeof(GameDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GameDto>> GetGameAsync(string alias, string gameId)
    {
        var (game, error) = await GetAuthorizedGameAsync(alias, gameId);
        if (error != null) return error;

        // We already fetched the game in GetAuthorizedGameAsync, so return it directly.
        return Ok(game);
    }

    /// <summary>
    /// Removes a possible value from a cell
    /// </summary>
    /// <param name="alias">The player's alias</param>
    /// <param name="gameId">The game id</param>
    /// <param name="request">The possible value request</param>
    /// <returns>Success or failure result</returns>
    [HttpDelete("{gameId}/possible-values")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> RemovePossibleValueAsync(string alias, string gameId, [FromBody] PossibleValueRequest request)
    {
        var (game, error) = await GetAuthorizedGameAsync(alias, gameId);
        if (error != null) return error;

        var result = await gameService.RemovePossibleValueAsync(gameId, request.Row, request.Column, request.Value);
        return HandleUnitResult(result);
    }

    /// <summary>
    /// Resets a game to its initial state
    /// </summary>
    /// <param name="alias">The player's alias</param>
    /// <param name="gameId">The game id</param>
    /// <returns>No content if successful</returns>
    [HttpPost("{gameId}/reset")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> ResetGameAsync(string alias, string gameId)
    {
        var (game, error) = await GetAuthorizedGameAsync(alias, gameId);
        if (error != null) return error;

        var result = await gameService.ResetGameAsync(gameId);
        return HandleUnitResult(result);
    }

    /// <summary>
    /// Undoes the last move in a game
    /// </summary>
    /// <param name="alias">The player's alias</param>
    /// <param name="gameId">The game id</param>
    /// <returns>No content if successful</returns>
    [HttpPost("{gameId}/undo")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> UndoMoveAsync(string alias, string gameId)
    {
        var (game, error) = await GetAuthorizedGameAsync(alias, gameId);
        if (error != null) return error;

        var result = await gameService.UndoLastMoveAsync(gameId);
        return HandleUnitResult(result);
    }

    /// <summary>
    /// Updates the status of a game for the specified player.
    /// </summary>
    /// <param name="alias">The alias of the player associated with the game. Cannot be null or empty.</param>
    /// <param name="gameId">The unique identifier of the game to update. Cannot be null or empty.</param>
    /// <param name="gameStatus">The new status to set for the game.</param>
    /// <returns>An <see cref="ActionResult"/> indicating the result of the operation. Returns 204 No Content if the update is
    /// successful, 400 Bad Request if the input is invalid or the update fails, or 404 Not Found if the game does not
    /// exist or does not belong to the specified player.</returns>
    [HttpPatch("{gameId}/status/{gameStatus}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> UpdateGameStatusAsync(string alias, string gameId, string gameStatus)
    {
        var (game, error) = await GetAuthorizedGameAsync(alias, gameId);
        if (error != null) return error;

        var result = await gameService.UpdateGameStatusAsync(gameId, gameStatus);
        return HandleUnitResult(result);
    }

    /// <summary>
    /// Updates a game (makes a move)
    /// </summary>
    /// <param name="alias">The player's alias</param>
    /// <param name="gameId">The game id</param>
    /// <param name="move">The move to make</param>
    /// <returns>Success or failure result</returns>
    [HttpPut("{gameId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> UpdateGameAsync(string alias, string gameId, [FromBody] MoveRequest move)
    {
        var (game, error) = await GetAuthorizedGameAsync(alias, gameId);
        if (error != null) return error;

        var result = await gameService.MakeMoveAsync(gameId, move.Row, move.Column, move.Value, move.PlayDuration);
        return HandleUnitResult(result);
    }

    /// <summary>
    /// Validates a game to check if it's completed correctly
    /// </summary>
    /// <param name="alias">The player's alias</param>
    /// <param name="gameId">The game id</param>
    /// <returns>Result of the validation</returns>
    [HttpPost("{gameId}/validate")]
    [ProducesResponseType(typeof(ValidationResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ValidationResultDto>> ValidateGameAsync(string alias, string gameId)
    {
        var (game, error) = await GetAuthorizedGameAsync(alias, gameId);
        if (error != null) return error;

        var result = await gameService.ValidateGameAsync(gameId);

        if (!result.IsSuccess)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }
    
    private async Task<(GameDto? game, ActionResult? error)> GetAuthorizedGameAsync(string alias, string gameId)
    {
        if (string.IsNullOrWhiteSpace(alias) || string.IsNullOrWhiteSpace(gameId))
        {
            return (null, BadRequest("Player alias and game id cannot be null or empty."));
        }

        var gameResult = await gameService.GetGameAsync(gameId);
        if (!gameResult.IsSuccess)
        {
            return (null, BadRequest(gameResult.Error));
        }

        if (gameResult.Value.PlayerAlias != alias)
        {
            return (null, NotFound());
        }

        return (gameResult.Value, null);
    }

    private ActionResult HandleUnitResult(dynamic result)
    {
        // result is expected to have IsSuccess and Error members.
        if (!result.IsSuccess)
        {
            return BadRequest(result.Error);
        }

        return NoContent();
    }
}
