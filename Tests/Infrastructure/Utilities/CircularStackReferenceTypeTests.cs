using Sudoku.Infrastructure.Utilities;

namespace UnitTests.Infrastructure.Utilities;

// Test helper classes for reference type testing
public class TestPerson : ICloneable
{
    public string Name { get; set; } = string.Empty;
    public int Age { get; set; }

    public object Clone()
    {
        return new TestPerson { Name = Name, Age = Age };
    }

    public override bool Equals(object? obj)
    {
        return obj is TestPerson person && Name == person.Name && Age == person.Age;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Name, Age);
    }
}

public class TestGameState
{
    public List<int> Board { get; set; } = [];
    public string PlayerName { get; set; } = string.Empty;
    public int Score { get; set; }

    public override bool Equals(object? obj)
    {
        return obj is TestGameState state &&
               Board.SequenceEqual(state.Board) &&
               PlayerName == state.PlayerName &&
               Score == state.Score;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Board, PlayerName, Score);
    }
}

public class CircularStackReferenceTypeTests
{
    [Fact]
    public void Push_WithReferenceTypeWithoutCloning_ModificationDoesNotAffectStoredItem()
    {
        // Arrange - demonstrates the solution to the original problem
        var sut = new CircularStack<TestGameState>(3);
        var gameState = new TestGameState 
        { 
            Board = [1, 2, 3], 
            PlayerName = "Alice", 
            Score = 100 
        };

        // Act
        sut.Push(gameState);
        var originalStoredState = sut.Peek();
        
        // Modify the original object
        gameState.Score = 200;
        gameState.Board.Add(4);

        var modifiedStoredState = sut.Peek();

        // Assert - With our new implementation using JSON serialization, stored items are independent
        modifiedStoredState.Should().NotBe(gameState); // Different reference (content-wise)
        modifiedStoredState.Score.Should().Be(100); // Original value preserved
        modifiedStoredState.Board.Should().Equal([1, 2, 3]); // Original board preserved
        
        // Verify original object was indeed modified
        gameState.Score.Should().Be(200);
        gameState.Board.Should().Equal([1, 2, 3, 4]);
    }

    [Fact]
    public void Push_WithICloneableType_CreatesIndependentCopy()
    {
        // Arrange
        var sut = new CircularStack<TestPerson>(3);
        var person = new TestPerson { Name = "John", Age = 25 };

        // Act
        sut.Push(person);
        var storedPerson = sut.Peek();
        
        // Modify the original
        person.Age = 30;
        person.Name = "Jane";

        var retrievedPerson = sut.Peek();

        // Assert
        retrievedPerson.Should().NotBeSameAs(person); // Different reference
        retrievedPerson.Age.Should().Be(25); // Original value preserved
        retrievedPerson.Name.Should().Be("John"); // Original name preserved
        
        // Verify original was modified
        person.Age.Should().Be(30);
        person.Name.Should().Be("Jane");
    }

    [Fact]
    public void Push_WithCustomCloneFunction_UsesProvidedCloneLogic()
    {
        // Arrange
        var customCloneFunc = (TestGameState original) => new TestGameState
        {
            Board = [..original.Board], // Deep copy the list
            PlayerName = original.PlayerName + "_cloned",
            Score = original.Score
        };
        
        var sut = new CircularStack<TestGameState>(3, customCloneFunc);
        var gameState = new TestGameState 
        { 
            Board = [1, 2, 3], 
            PlayerName = "Alice", 
            Score = 100 
        };

        // Act
        sut.Push(gameState);
        var storedState = sut.Peek();

        // Modify original
        gameState.Board.Add(4);
        gameState.Score = 200;

        var retrievedState = sut.Peek();

        // Assert
        retrievedState.Should().NotBeSameAs(gameState);
        retrievedState.PlayerName.Should().Be("Alice_cloned"); // Custom clone logic applied
        retrievedState.Score.Should().Be(100); // Original value preserved
        retrievedState.Board.Should().Equal([1, 2, 3]); // Original board preserved
    }

    [Fact]
    public void Push_WithValueType_WorksAsExpected()
    {
        // Arrange
        var sut = new CircularStack<int>(3);
        
        // Act & Assert - Value types don't have reference issues
        sut.Push(10);
        var stored = sut.Peek();
        
        stored.Should().Be(10);
    }

    [Fact]
    public void Push_WithString_WorksAsExpected()
    {
        // Arrange
        var sut = new CircularStack<string>(3);
        
        // Act
        sut.Push("test");
        var stored = sut.Peek();
        
        // Assert - Strings are immutable, so they don't have reference modification issues
        stored.Should().Be("test");
    }

    [Fact]
    public void Push_WithNullReferenceType_HandlesNullCorrectly()
    {
        // Arrange
        var sut = new CircularStack<TestGameState?>(3);
        
        // Act
        sut.Push(null);
        var stored = sut.Peek();
        
        // Assert
        stored.Should().BeNull();
    }

    [Fact]
    public void Push_MultipleReferenceTypesWithModifications_MaintainsIndependentCopies()
    {
        // Arrange
        var sut = new CircularStack<TestGameState>(3);
        var gameState1 = new TestGameState { Board = [1], PlayerName = "Alice", Score = 100 };
        var gameState2 = new TestGameState { Board = [2], PlayerName = "Bob", Score = 200 };
        var gameState3 = new TestGameState { Board = [3], PlayerName = "Charlie", Score = 300 };

        // Act
        sut.Push(gameState1);
        sut.Push(gameState2);
        sut.Push(gameState3);

        // Modify all original objects
        gameState1.Score = 999;
        gameState1.Board.Add(10);
        gameState2.Score = 888;
        gameState2.Board.Add(20);
        gameState3.Score = 777;
        gameState3.Board.Add(30);

        // Assert - Pop them in reverse order and verify they maintain original values
        var retrieved3 = sut.Pop();
        var retrieved2 = sut.Pop();
        var retrieved1 = sut.Pop();

        retrieved3.Score.Should().Be(300);
        retrieved3.Board.Should().Equal([3]);
        retrieved3.PlayerName.Should().Be("Charlie");

        retrieved2.Score.Should().Be(200);
        retrieved2.Board.Should().Equal([2]);
        retrieved2.PlayerName.Should().Be("Bob");

        retrieved1.Score.Should().Be(100);
        retrieved1.Board.Should().Equal([1]);
        retrieved1.PlayerName.Should().Be("Alice");
    }

    [Fact]
    public void Push_CircularOverwrite_MaintainsIndependentCopiesWhenOverwriting()
    {
        // Arrange - Test the specific scenario where circular overwriting occurs
        var sut = new CircularStack<TestGameState>(2); // Small capacity to force overwriting
        var gameState1 = new TestGameState { Board = [1], PlayerName = "Alice", Score = 100 };
        var gameState2 = new TestGameState { Board = [2], PlayerName = "Bob", Score = 200 };
        var gameState3 = new TestGameState { Board = [3], PlayerName = "Charlie", Score = 300 };

        // Act
        sut.Push(gameState1);
        sut.Push(gameState2);
        
        // Modify the original objects before pushing the third (which will overwrite the first)
        gameState1.Score = 999;
        gameState2.Score = 888;
        
        sut.Push(gameState3); // This should overwrite gameState1's slot

        // Assert
        sut.Count.Should().Be(2); // Should still be at capacity
        
        // The stack should contain gameState2 and gameState3 (original values)
        var top = sut.Pop(); // Should be gameState3
        var second = sut.Pop(); // Should be gameState2

        top.Score.Should().Be(300);
        top.PlayerName.Should().Be("Charlie");
        
        second.Score.Should().Be(200); // Should be original value, not 888
        second.PlayerName.Should().Be("Bob");
    }

    [Fact]
    public void Constructor_WithNullCloneFunction_UsesFallbackCloning()
    {
        // Arrange & Act
        var sut = new CircularStack<TestGameState>(3, null);
        var gameState = new TestGameState { Board = [1, 2], PlayerName = "Test", Score = 50 };
        
        sut.Push(gameState);
        gameState.Score = 999; // Modify original
        
        var stored = sut.Peek();
        
        // Assert - Should still use default cloning (JSON serialization)
        stored.Score.Should().Be(50); // Original value preserved
    }

    [Fact]
    public void Push_WithCustomCloneFunctionThatReturnsNull_HandlesGracefully()
    {
        // Arrange
        var sut = new CircularStack<TestGameState>(3, _ => null!);
        var gameState = new TestGameState { Board = [1], PlayerName = "Alice", Score = 100 };

        // Act
        sut.Push(gameState);
        var stored = sut.Peek();

        // Assert
        stored.Should().BeNull();
    }

    [Fact]
    public void CloneFunction_ReceivesCorrectItemToClone()
    {
        // Arrange
        TestGameState? receivedItem = null;
        var cloneFunction = (TestGameState item) => {
            receivedItem = item;
            return new TestGameState { Board = [..item.Board], PlayerName = item.PlayerName, Score = item.Score };
        };
        
        var sut = new CircularStack<TestGameState>(3, cloneFunction);
        var gameState = new TestGameState { Board = [5, 6], PlayerName = "Test", Score = 42 };

        // Act
        sut.Push(gameState);

        // Assert
        receivedItem.Should().NotBeNull();
        receivedItem!.Board.Should().Equal([5, 6]);
        receivedItem.PlayerName.Should().Be("Test");
        receivedItem.Score.Should().Be(42);
    }
}