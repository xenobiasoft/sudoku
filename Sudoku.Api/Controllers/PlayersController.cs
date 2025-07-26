using Microsoft.AspNetCore.Mvc;
using Sudoku.Application.Interfaces;

namespace Sudoku.Api.Controllers
{
    [Route("api/players")]
    [ApiController]
    public class PlayersController(IPlayerApplicationService playerService) : ControllerBase
    {
        // Since we don't have an IPlayerApplicationService yet, we'll need to add one
        // This is a placeholder for now
        private readonly IPlayerApplicationService _playerService = playerService;

        /// <summary>
        /// Creates a new player alias
        /// </summary>
        /// <returns>The newly created player's alias</returns>
        [HttpPost]
        [ProducesResponseType(typeof(string), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<string>> PostAsync()
        {
            // This will need to be implemented once we have the PlayerApplicationService
            // For now, we'll just return a not implemented result
            return StatusCode(StatusCodes.Status501NotImplemented, "This endpoint needs to be implemented");
        }
    }
}
