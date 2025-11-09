namespace Sudoku.Domain.Interfaces;

/// <summary>
/// Defines a method for creating a deep copy of an object.
/// </summary>
/// <typeparam name="T">The type of object to clone.</typeparam>
public interface ICloneable<out T>
{
    /// <summary>
    /// Creates a deep copy of the current object.
    /// </summary>
    /// <returns>A deep copy of the current object.</returns>
    T Clone();
}