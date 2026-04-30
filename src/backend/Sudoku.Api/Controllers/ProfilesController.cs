using MediatR;
using Microsoft.AspNetCore.Mvc;
using Sudoku.Api.Models;
using Sudoku.Application.Commands;
using Sudoku.Application.DTOs;
using Sudoku.Application.Queries;

namespace Sudoku.Api.Controllers;

[Route("api/profiles")]
[ApiController]
public class ProfilesController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(ProfileDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ProfileDto>> CreateProfileAsync([FromBody] CreateProfileRequest request)
    {
        var result = await mediator.Send(new CreateProfileCommand(request.Alias));

        if (!result.IsSuccess)
        {
            if (result.Error!.Contains("already taken", StringComparison.OrdinalIgnoreCase))
                return Conflict(result.Error);
            return BadRequest(result.Error);
        }

        return StatusCode(StatusCodes.Status201Created, result.Value);
    }

    [HttpGet("{alias}")]
    [ProducesResponseType(typeof(ProfileDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProfileDto>> GetProfileAsync(string alias)
    {
        var result = await mediator.Send(new GetProfileByAliasQuery(alias));

        if (!result.IsSuccess)
            return BadRequest(result.Error);

        if (result.Value == null)
            return NotFound();

        return Ok(result.Value);
    }

    [HttpPatch("{alias}")]
    [ProducesResponseType(typeof(ProfileDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ProfileDto>> UpdateProfileAliasAsync(string alias, [FromBody] UpdateProfileAliasRequest request)
    {
        // First resolve the profile by alias to get profileId
        var getResult = await mediator.Send(new GetProfileByAliasQuery(alias));
        if (!getResult.IsSuccess)
            return BadRequest(getResult.Error);
        if (getResult.Value == null)
            return NotFound();

        var updateResult = await mediator.Send(new UpdateProfileAliasCommand(getResult.Value.ProfileId, request.NewAlias));

        if (!updateResult.IsSuccess)
        {
            if (updateResult.Error!.Contains("already taken", StringComparison.OrdinalIgnoreCase))
                return Conflict(updateResult.Error);
            if (updateResult.Error.Contains("not found", StringComparison.OrdinalIgnoreCase))
                return NotFound();
            return BadRequest(updateResult.Error);
        }

        return Ok(updateResult.Value);
    }
}
