using Microsoft.AspNetCore.Mvc;
using Sudoku.Api.Models;
using Sudoku.Application.Interfaces;

namespace Sudoku.Api.Controllers
{
    [Route("api/players")]
    [ApiController]
    public class PlayersController(IPlayerApplicationService playerService) : ControllerBase
    {
        private readonly IPlayerApplicationService _playerService = playerService;

        /// <summary>
        /// Creates a new player alias
        /// </summary>
        /// <param name="request">Optional request with a custom alias</param>
        /// <returns>The newly created player's alias</returns>
        [HttpPost]
        [ProducesResponseType(typeof(string), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<string>> CreatePlayerAsync([FromBody] CreatePlayerRequest? request = null)
        {
            var result = await _playerService.CreatePlayerAsync(request?.Alias);
            
            if (!result.IsSuccess)
            {
                return BadRequest(result.Error);
            }
            
            return CreatedAtAction(nameof(PlayerExistsAsync), new { alias = result.Value }, result.Value);
        }
        
        /// <summary>
        /// Checks if a player with the given alias exists
        /// </summary>
        /// <param name="alias">The player's alias</param>
        /// <returns>True if the player exists, false otherwise</returns>
        [HttpGet("{alias}/exists")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<bool>> PlayerExistsAsync(string alias)
        {
            if (string.IsNullOrWhiteSpace(alias))
            {
                return BadRequest("Player alias cannot be null or empty.");
            }
            
            var result = await _playerService.PlayerExistsAsync(alias);
            
            if (!result.IsSuccess)
            {
                return BadRequest(result.Error);
            }
            
            return Ok(result.Value);
        }
    }
}
