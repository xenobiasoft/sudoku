namespace Sudoku.Domain.Helpers;

public static class HashSetExtensions
{
    public static void AddRange<TType>(this HashSet<TType> set, IEnumerable<TType> values)
    {
        foreach (var type in values.ToList())
        {
            set.Add(type);
        }
    }
}