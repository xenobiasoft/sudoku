using Sudoku.Infrastructure.Utilities;

namespace UnitTests.Infrastructure.Utilities;

public class CircularStackTests
{
	[Theory]
	[InlineData(0)]
	[InlineData(-1)]
	public void ConstructorTest_WhenCreatingWithCapacityLessOrEqualToZero_ThrowsArgumentOutOfRangeException(int capacity)
	{
		// Arrange
		
		// Act
		CircularStack<int> ConstructorTest() => new(capacity);

		// Assert
		Assert.Throws<ArgumentOutOfRangeException>(ConstructorTest);
	}

	[Fact]
	public void Clear_EmptiesEntireStack()
	{
		// Arrange
		var sut = new CircularStack<int>(5);
		for (var i = 0; i < 5; i++)
		{
			sut.Push(i);
		}

		// Act
		sut.Clear();

		// Assert
		Assert.Multiple(() =>
		{
			sut.Count.Should().Be(0);
			sut.IsFull().Should().BeFalse();
			sut.IsEmpty().Should().BeTrue();
		});
	}

	[Fact]
	public void IsEmpty_AfterInitialCreation_ReturnsTrue()
	{
		// Arrange

		// Act
		var sut = new CircularStack<int>(5);

		// Assert
		sut.IsEmpty().Should().BeTrue();
	}

	[Fact]
	public void IsFull_AfterInitialCreation_ReturnsFalse()
	{
		// Arrange

		// Act
		var sut = new CircularStack<int>(5);

		// Assert
		sut.IsFull().Should().BeFalse();
	}

	[Fact]
	public void Capacity_ReturnsTheCapacityValueGivenDuringCreation()
	{
		// Arrange

		// Act
		var sut = new CircularStack<int>(10);

		// Assert
		sut.Capacity.Should().Be(10);
	}

	[Fact]
	public void Push_AfterAddingItem_ReturnsCountOfOne()
	{
		// Arrange
		var sut = new CircularStack<int>(5);

		// Act
		sut.Push(6);

		// Assert
		sut.Count.Should().Be(1);
	}

	[Fact]
	public void Push_WhenAddingItemsToCapacity_FillsUpStack()
	{
		// Arrange
		var sut = new CircularStack<int>(5);

		// Act
		for (var i = 0; i < 5; i++)
		{
			sut.Push(i);
		}

		// Assert
		Assert.Multiple(() =>
		{
			sut.Count.Should().Be(5);
			sut.IsFull().Should().BeTrue();
			sut.IsEmpty().Should().BeFalse();
		});
	}

	[Fact]
	public void Push_AfterAddingMoreItemsThanCapacity_StillAcceptsItem()
	{
		// Arrange
		var sut = new CircularStack<int>(5);

		// Act
		for (var i = 0; i < 10; i++)
		{
			sut.Push(i);
		}

		// Assert
		Assert.Multiple(() =>
		{
			sut.Count.Should().Be(5);
			sut.IsFull().Should().BeTrue();
			sut.IsEmpty().Should().BeFalse();
		});
	}

	[Fact]
	public void Pop_ReturnsLastItemAdded()
	{
		// Arrange
		var sut = new CircularStack<int>(5);
		for (var i = 0; i < 15; i++)
		{
			sut.Push(i);
		}

		// Act
		var itemPopped = sut.Pop();

		// Assert
		itemPopped.Should().Be(14);
	}

	[Fact]
	public void Peek_ReturnsLastItem_ButDoesNotReduceCount()
	{
		// Arrange
		var sut = new CircularStack<int>(5);
		for (var i = 0; i < 15; i++)
		{
			sut.Push(i);
		}
		
		// Act
		var lastItem = sut.Peek();

		// Assert
		Assert.Multiple(() =>
		{
			sut.Count.Should().Be(5);
			lastItem.Should().Be(14);
		});
	}
}