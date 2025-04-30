using Bunit.TestDoubles;
using Microsoft.Extensions.DependencyInjection;
using Sudoku.Web.Server.Pages;
using UnitTests.Helpers.Mocks;
using XenobiaSoft.Sudoku;

namespace UnitTests.Web.Pages;

public class NewPageTests : TestContext
{
    private readonly Mock<ISudokuGame> _mockSudokuGame;

    public NewPageTests()
    {
        _mockSudokuGame = new Mock<ISudokuGame>();

        Services.AddSingleton(_mockSudokuGame.Object);
    }

    [Fact]
    public void OnInitializedAsync_GeneratesNewPuzzle()
    {
        // Arrange
        var puzzleId = "12345";
        _mockSudokuGame.SetupPuzzleId(puzzleId);

        // Act
        RenderComponent<New>(parameters => parameters.Add(p => p.Difficulty, "Medium"));

        // Assert
        _mockSudokuGame.VerifyGeneratesNewPuzzle(Times.Once);
    }

    [Fact]
    public void OnInitializedAsync_NavigatesToGamePageWithPuzzleId()
    {
        // Arrange
        var navMan = Services.GetRequiredService<FakeNavigationManager>();
        var puzzleId = "12345";
        var expectedUri = $"http://localhost/game/{puzzleId}";
        _mockSudokuGame.SetupPuzzleId(puzzleId);

        // Act
        RenderComponent<New>(parameters => parameters.Add(p => p.Difficulty, "Medium"));

        // Assert
        navMan.Uri.Should().Be(expectedUri);
    }

    [Fact]
    public void RendersLoader()
    {
        // Arrange

        // Act
        var sut = RenderComponent<New>(parameters => parameters.Add(p => p.Difficulty, "Medium"));

        // Assert
        var loader = sut.Find("div.loader");
        loader.Should().NotBeNull();
    }
}