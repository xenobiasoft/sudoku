using MediatR;
using Microsoft.AspNetCore.Mvc;
using Sudoku.Api.Controllers;
using Sudoku.Application.Common;
using Sudoku.Application.DTOs;
using Sudoku.Application.Queries;

namespace UnitTests.API;

public class StatsControllerTests : BaseGameControllerTests<StatsController>
{
    [Fact]
    public async Task GetPlayerStatsAsync_WithValidProfileId_ReturnsOkWithStats()
    {
        // Arrange
        var profileId = Guid.NewGuid().ToString();
        var stats = CreateTestStats();
        SetupStatsQuery(Result<PlayerStatsDto>.Success(stats));

        // Act
        var result = await Sut.GetPlayerStatsAsync(profileId);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeAssignableTo<PlayerStatsDto>().Subject.Should().BeEquivalentTo(stats);
    }

    [Fact]
    public async Task GetPlayerStatsAsync_WithValidProfileId_SendsQueryWithThatProfileId()
    {
        // Arrange
        var profileId = Guid.NewGuid().ToString();
        SetupStatsQuery(Result<PlayerStatsDto>.Success(CreateTestStats()));

        // Act
        await Sut.GetPlayerStatsAsync(profileId);

        // Assert
        MockMediator.Verify(
            x => x.Send(It.Is<GetPlayerStatsQuery>(q => q.ProfileId == profileId), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public async Task GetPlayerStatsAsync_WithEmptyProfileId_ReturnsBadRequest(string profileId)
    {
        // Act
        var result = await Sut.GetPlayerStatsAsync(profileId);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task GetPlayerStatsAsync_WhenResultIsFailure_ReturnsBadRequest()
    {
        // Arrange
        SetupStatsQuery(Result<PlayerStatsDto>.Failure("Something went wrong"));

        // Act
        var result = await Sut.GetPlayerStatsAsync(Guid.NewGuid().ToString());

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    private void SetupStatsQuery(Result<PlayerStatsDto> result)
    {
        MockMediator
            .Setup(x => x.Send(It.IsAny<GetPlayerStatsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);
    }

    private static PlayerStatsDto CreateTestStats() => new(
        GamesPlayed: 3,
        GamesWon: 2,
        WinRate: 2d / 3d,
        ByDifficulty:
        [
            new DifficultyStatsDto("Easy", 2, 2, TimeSpan.FromMinutes(6), TimeSpan.FromMinutes(4)),
            new DifficultyStatsDto("Medium", 1, 0, null, null),
            new DifficultyStatsDto("Hard", 0, 0, null, null),
            new DifficultyStatsDto("Expert", 0, 0, null, null)
        ]);
}
