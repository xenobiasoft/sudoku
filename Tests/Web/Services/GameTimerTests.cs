using Sudoku.Web.Server.Services;

namespace UnitTests.Web.Services;

public class GameTimerTests
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
        _timer.Resume(pausedTimeTicks);

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
    public void Resume_WithInitialDuration_ShouldStartWithCorrectDuration()
    {
        // Arrange
        var initialDuration = TimeSpan.FromMinutes(5);
        _timer.Start();
        _timer.Pause();

        // Act
        _timer.Resume(initialDuration);

        // Assert
        _timer.ElapsedTime.Should().BeCloseTo(initialDuration, TimeSpan.FromMilliseconds(50));
    }

    [Fact]
    public void Resume_WithInitialDuration_ShouldContinueFromInitialDuration()
    {
        // Arrange
        var initialDuration = TimeSpan.FromMinutes(5);
        _timer.Start();
        _timer.Pause();
        _timer.Resume(initialDuration);

        // Act
        Thread.Sleep(1000); // Wait for 1 second

        // Assert
        Assert.True(_timer.ElapsedTime > initialDuration);
        Assert.True(_timer.ElapsedTime <= initialDuration.Add(TimeSpan.FromSeconds(2))); // Allow for some timing variance
    }

    [Fact]
    public void Resume_WithInitialDuration_ShouldRaiseTickEventWithCorrectDuration()
    {
        // Arrange
        var initialDuration = TimeSpan.FromMinutes(5);
        TimeSpan? tickedDuration = null;
        _timer.OnTick += (_, duration) => tickedDuration = duration;
        _timer.Start();
        _timer.Pause();
        _timer.Resume(initialDuration);

        // Act
        Thread.Sleep(1100); // Wait for first tick

        // Assert
        Assert.NotNull(tickedDuration);
        Assert.True(tickedDuration > initialDuration);
        Assert.True(tickedDuration <= initialDuration.Add(TimeSpan.FromSeconds(2))); // Allow for some timing variance
    }

    [Fact]
    public void Resume_WithInitialDuration_WhenAlreadyRunning_ShouldNotChangeDuration()
    {
        // Arrange
        var initialDuration = TimeSpan.FromMinutes(5);
        _timer.Start();
        Thread.Sleep(1000); // Let it run for a bit
        var beforeDuration = _timer.ElapsedTime;

        // Act
        _timer.Resume(initialDuration);

        // Assert
        _timer.ElapsedTime.Should().BeCloseTo(beforeDuration, TimeSpan.FromMilliseconds(50));
    }
}