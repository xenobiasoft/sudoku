---
paths:
  - "src/backend/Tests/**/*.cs"
---

# Testing Guidelines

## Test Naming

Use the pattern: `[MethodName]_[Scenario]_[ExpectedResult]`

## Test Structure

- **Arrange-Act-Assert**: clear separation of test phases
- **Test Isolation**: each test must be independent
- **Single Assert**: one logical assertion per test

## DepenMock Framework

- Inherit from `BaseTestByAbstraction` when the SUT implements an interface or inherits a base class
- Inherit from `BaseTestByType` otherwise
- Resolve SUT via `ResolveSut()`
- Resolve mocks in the constructor via `Container.ResolveMock<T>()` and store as private fields
- Every test class must carry `[LogOutput(LogOutputTiming.Always)]` above the class declaration, with `using DepenMock.Attributes;` in the using block, so captured log output is always emitted for diagnostics. This attribute is inherited (`AttributeUsage(Inherited = true)`), so a shared abstract test base (e.g. `BaseGameControllerTests<T>`) only needs it once — concrete subclasses pick it up automatically and should not redeclare it

```csharp
using DepenMock.Attributes;

[LogOutput(LogOutputTiming.Always)]
public class SudokuGameTests : MoqBaseTestByType<SudokuGame>
{
    [Fact]
    public void MakeMove_ValidMove_RaisesEvent()
    {
        // Arrange
        var sut = ResolveSut();
        var initialEventCount = sut.DomainEvents.Count;

        // Act
        sut.MakeMove(0, 0, 5);

        // Assert
        sut.DomainEvents.Count.Should().Be(initialEventCount + 1);
    }
}
```

## Logging Assertions

Never mock `ILogger`. Use the `Logger` property from the base class:

```csharp
[Fact]
public void MakeMove_ValidMove_LogsInformation()
{
    var sut = ResolveSut();

    sut.MakeMove(0, 0, 5);

    Logger.InformationLogs().ContainsMessage("Player made a move");
}
```

## Mock Setup & Verification

### DSL Helper Extensions

When setting up or verifying mocks, check the `Mocks/` folder in the root of the test project for existing **DSL extensions**. Those extensions provide a fluent API for common mock setups and verifications, improving readability and maintainability.

- **Rule**: Always prefer existing DSL helpers over raw `.Setup()` and `Verify()` calls. If no helper exists for your scenario, create one and add it to the `Mocks/` folder for future reuse.
- **Pattern**: Follow the naming convention `Mock[ClassName]Extensions` for the helper class and use descriptive method names that clearly indicate the behavior being set up or verified. Use `Setup[Behavior]` for setup methods and `Verify[Behavior]` for verification methods.
- **Special Cases**: When setting up a mock to throw an exception, use the `SetupThrows[Behavior]` pattern for clarity. For example, `SetupThrowsWhenInvalidInput()` clearly indicates that the setup is configuring the mock to throw an exception when invalid input is provided.

Available log-level methods: `InformationLogs()`, `WarningLogs()`, `ErrorLogs()`, `DebugLogs()`, `CriticalLogs()`

## Integration Testing

- Test complete workflows
- Use in-memory databases for testing
- Test API endpoints with real HTTP requests
- CI enforces **80% line coverage**
