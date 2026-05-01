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

```csharp
[Test]
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
```

## Logging Assertions
Never mock `ILogger`. Use the `Logger` property from the base class:

```csharp
[Test]
public void MakeMove_ValidMove_LogsInformation()
{
    var sut = ResolveSut();

    sut.MakeMove(0, 0, 5);

    Logger.InformationLogs().ContainsMessage("Player made a move");
}
```

Available log-level methods: `InformationLogs()`, `WarningLogs()`, `ErrorLogs()`, `DebugLogs()`, `CriticalLogs()`

## Integration Testing
- Test complete workflows
- Use in-memory databases for testing
- Test API endpoints with real HTTP requests
- CI enforces **80% line coverage**
