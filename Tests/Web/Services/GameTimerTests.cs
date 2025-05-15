using Sudoku.Web.Server.Services;

namespace UnitTests.Web.Services;

public class GameTimerTests : IDisposable
{
    private readonly TimeSpan _interval = TimeSpan.FromMilliseconds(10);
    private readonly GameTimer _timer;

    public GameTimerTests()
    {
        _timer = new GameTimer(_interval);
    }

    [Fact]
    public void Start_ShouldSetIsRunning()
    {
        // Arrange
        _timer.IsRunning.Should().BeFalse();

        // Act
        _timer.Start();

        // Assert
        _timer.IsRunning.Should().BeTrue();
    }

    [Fact]
    public async Task Start_ShouldIncreaseElapsedTime()
    {
        // Arrange
        _timer.ElapsedTime.Should().Be(TimeSpan.Zero);

        // Act
        _timer.Start();

        // Assert
        await Task.Delay(_interval * 2);
        _timer.ElapsedTime.Should().BeGreaterThan(TimeSpan.Zero);
    }

    [Fact]
    public async Task Pause_ShouldStopElapsedTime()
    {
        // Arrange
        _timer.Start();
        await Task.Delay(_interval);

        // Act
        _timer.Pause();

        // Assert
        var pausedTime = _timer.ElapsedTime;
        await Task.Delay(_interval);
        _timer.IsRunning.Should().BeFalse();
    }

    [Fact]
    public async Task Resume_ShouldContinueElapsedTime()
    {
        // Arrange
        _timer.Start();
        await Task.Delay(_interval * 2);
        _timer.Pause();
        var pausedTimeTicks = _timer.ElapsedTime;
        await Task.Delay(_interval * 2);

        // Act
        _timer.Resume();

        // Assert
        _timer.IsRunning.Should().BeTrue();
        await Task.Delay(_interval * 2);
        var totalElapsed = _timer.ElapsedTime;
        totalElapsed.Should().BeGreaterThan(pausedTimeTicks);
    }

    [Fact]
    public async Task Reset_ShouldClearElapsedTime()
    {
        // Arrange
        _timer.Start();
        await Task.Delay(_interval * 2);

        // Act
        _timer.Reset();

        // Assert
        _timer.ElapsedTime.Should().Be(TimeSpan.Zero);
    }

    [Fact]
    public async Task Reset_ShouldStopTimer()
    {
        // Arrange
        _timer.Start();
        await Task.Delay(_interval * 2);

        // Act
        _timer.Reset();

        // Assert
        _timer.IsRunning.Should().BeFalse();
    }

    [Fact]
    public async Task OnTick_ShouldFireAtInterval()
    {
        // Arrange
        var tickCount = 0;
        _timer.OnTick += (s, t) => tickCount++;

        // Act
        _timer.Start();
        await Task.Delay(_interval * 5);
        _timer.Pause();

        // Assert
        tickCount.Should().BeGreaterThanOrEqualTo(3);
    }

    [Fact]
    public void Dispose_ShouldNotThrow()
    {
        // Arrange
        _timer.Start();

        // Act
        var dispose = () => _timer.Dispose();

        // Assert
        dispose.Should().NotThrow();
    }

    [Fact]
    public async Task Dispose_ShouldStopTimer()
    {
        // Arrange
        _timer.Start();

        // Act
        _timer.Dispose();

        // Assert
        await Task.Delay(_interval * 2);
        _timer.IsRunning.Should().BeFalse();
    }

    public void Dispose()
    {
        _timer.Dispose();
    }
}