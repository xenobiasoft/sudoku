using Microsoft.AspNetCore.Mvc;
using XenobiaSoft.Sudoku.Services;

namespace Sudoku.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlayersController(IPlayerService playerService) : ControllerBase
    {
        [HttpPost]
        public async Task<ActionResult<string>> PostAsync()
        {
            var alias = await playerService.CreateNewAsync();

            return CreatedAtAction(nameof(PostAsync), new { alias });
        }
    }
}
