using Microsoft.AspNetCore.Mvc;
using Sudoku.Application.DTOs;
using Sudoku.Application.Interfaces;

namespace Sudoku.Api.Controllers;

[Route("api/players/{alias}/games")]
[ApiController]
public class GamesController(IGameApplicationService gameService) : ControllerBase
{
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
        if (string.IsNullOrWhiteSpace(alias) || string.IsNullOrWhiteSpace(gameId))
        {
            return BadRequest("Player alias and game id cannot be null or empty.");
        }

        var result = await gameService.GetGameAsync(gameId);
        
        if (!result.IsSuccess)
        {
            return BadRequest(result.Error);
        }

        // Verify the game belongs to the player
        if (result.Value.PlayerAlias != alias)
        {
            return NotFound();
        }
        
        return Ok(result.Value);
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
        
        return CreatedAtAction(nameof(GetGameAsync), new { alias, gameId = result.Value.Id }, result.Value);
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
        if (string.IsNullOrWhiteSpace(alias) || string.IsNullOrWhiteSpace(gameId))
        {
            return BadRequest("Player alias and game id cannot be null or empty.");
        }

        // Verify the game exists and belongs to the player
        var gameResult = await gameService.GetGameAsync(gameId);
        if (!gameResult.IsSuccess)
        {
            return BadRequest(gameResult.Error);
        }
        
        if (gameResult.Value.PlayerAlias != alias)
        {
            return NotFound();
        }

        var result = await gameService.MakeMoveAsync(gameId, move.Row, move.Column, move.Value);
        
        if (!result.IsSuccess)
        {
            return BadRequest(result.Error);
        }
        
        return NoContent();
    }

    /// <summary>
    /// Adds a possible value to a cell
    /// </summary>
    /// <param name="alias">The player's alias</param>
    /// <param name="gameId">The game id</param>
    /// <param name="request">The possible value request</param>
    /// <returns>Success or failure result</returns>
    [HttpPost("{gameId}/possiblevalues")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> AddPossibleValueAsync(string alias, string gameId, [FromBody] PossibleValueRequest request)
    {
        if (string.IsNullOrWhiteSpace(alias) || string.IsNullOrWhiteSpace(gameId))
        {
            return BadRequest("Player alias and game id cannot be null or empty.");
        }

        // Verify the game exists and belongs to the player
        var gameResult = await gameService.GetGameAsync(gameId);
        if (!gameResult.IsSuccess)
        {
            return BadRequest(gameResult.Error);
        }
        
        if (gameResult.Value.PlayerAlias != alias)
        {
            return NotFound();
        }

        var result = await gameService.AddPossibleValueAsync(gameId, request.Row, request.Column, request.Value);
        
        if (!result.IsSuccess)
        {
            return BadRequest(result.Error);
        }
        
        return NoContent();
    }

    /// <summary>
    /// Removes a possible value from a cell
    /// </summary>
    /// <param name="alias">The player's alias</param>
    /// <param name="gameId">The game id</param>
    /// <param name="request">The possible value request</param>
    /// <returns>Success or failure result</returns>
    [HttpDelete("{gameId}/possiblevalues")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> RemovePossibleValueAsync(string alias, string gameId, [FromBody] PossibleValueRequest request)
    {
        if (string.IsNullOrWhiteSpace(alias) || string.IsNullOrWhiteSpace(gameId))
        {
            return BadRequest("Player alias and game id cannot be null or empty.");
        }

        // Verify the game exists and belongs to the player
        var gameResult = await gameService.GetGameAsync(gameId);
        if (!gameResult.IsSuccess)
        {
            return BadRequest(gameResult.Error);
        }
        
        if (gameResult.Value.PlayerAlias != alias)
        {
            return NotFound();
        }

        var result = await gameService.RemovePossibleValueAsync(gameId, request.Row, request.Column, request.Value);
        
        if (!result.IsSuccess)
        {
            return BadRequest(result.Error);
        }
        
        return NoContent();
    }

    /// <summary>
    /// Clears all possible values from a cell
    /// </summary>
    /// <param name="alias">The player's alias</param>
    /// <param name="gameId">The game id</param>
    /// <param name="request">The cell request</param>
    /// <returns>Success or failure result</returns>
    [HttpDelete("{gameId}/possiblevalues/clear")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> ClearPossibleValuesAsync(string alias, string gameId, [FromBody] CellRequest request)
    {
        if (string.IsNullOrWhiteSpace(alias) || string.IsNullOrWhiteSpace(gameId))
        {
            return BadRequest("Player alias and game id cannot be null or empty.");
        }

        // Verify the game exists and belongs to the player
        var gameResult = await gameService.GetGameAsync(gameId);
        if (!gameResult.IsSuccess)
        {
            return BadRequest(gameResult.Error);
        }
        
        if (gameResult.Value.PlayerAlias != alias)
        {
            return NotFound();
        }

        var result = await gameService.ClearPossibleValuesAsync(gameId, request.Row, request.Column);
        
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
        if (string.IsNullOrWhiteSpace(alias) || string.IsNullOrWhiteSpace(gameId))
        {
            return BadRequest("Player alias and game id cannot be null or empty.");
        }

        // Verify the game exists and belongs to the player
        var gameResult = await gameService.GetGameAsync(gameId);
        if (!gameResult.IsSuccess)
        {
            return BadRequest(gameResult.Error);
        }
        
        if (gameResult.Value.PlayerAlias != alias)
        {
            return NotFound();
        }

        var result = await gameService.DeleteGameAsync(gameId);
        
        if (!result.IsSuccess)
        {
            return BadRequest(result.Error);
        }
        
        return NoContent();
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
        if (string.IsNullOrWhiteSpace(alias) || string.IsNullOrWhiteSpace(gameId))
        {
            return BadRequest("Player alias and game id cannot be null or empty.");
        }

        // Verify the game exists and belongs to the player
        var gameResult = await gameService.GetGameAsync(gameId);
        if (!gameResult.IsSuccess)
        {
            return BadRequest(gameResult.Error);
        }
        
        if (gameResult.Value.PlayerAlias != alias)
        {
            return NotFound();
        }

        var result = await gameService.UndoLastMoveAsync(gameId);
        
        if (!result.IsSuccess)
        {
            return BadRequest(result.Error);
        }
        
        return NoContent();
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
        if (string.IsNullOrWhiteSpace(alias) || string.IsNullOrWhiteSpace(gameId))
        {
            return BadRequest("Player alias and game id cannot be null or empty.");
        }

        // Verify the game exists and belongs to the player
        var gameResult = await gameService.GetGameAsync(gameId);
        if (!gameResult.IsSuccess)
        {
            return BadRequest(gameResult.Error);
        }
        
        if (gameResult.Value.PlayerAlias != alias)
        {
            return NotFound();
        }

        var result = await gameService.ResetGameAsync(gameId);
        
        if (!result.IsSuccess)
        {
            return BadRequest(result.Error);
        }
        
        return NoContent();
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
        if (string.IsNullOrWhiteSpace(alias) || string.IsNullOrWhiteSpace(gameId))
        {
            return BadRequest("Player alias and game id cannot be null or empty.");
        }

        // Verify the game exists and belongs to the player
        var gameResult = await gameService.GetGameAsync(gameId);
        if (!gameResult.IsSuccess)
        {
            return BadRequest(gameResult.Error);
        }
        
        if (gameResult.Value.PlayerAlias != alias)
        {
            return NotFound();
        }

        var result = await gameService.ValidateGameAsync(gameId);
        
        if (!result.IsSuccess)
        {
            return BadRequest(result.Error);
        }
        
        return Ok(result.Value);
    }
}

// Request Models
public record MoveRequest(int Row, int Column, int? Value);
public record PossibleValueRequest(int Row, int Column, int Value);
public record CellRequest(int Row, int Column);