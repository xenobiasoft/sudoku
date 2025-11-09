using System.Text.Json;

namespace Sudoku.Infrastructure.Utilities;

public class CircularStack<TStackItemType>
{
    private readonly Lock _syncRoot = new();
    private readonly Func<TStackItemType, TStackItemType>? _cloneFunction;
    private TStackItemType[] _buffer;
    private int _top;

    public CircularStack(int capacity, Func<TStackItemType, TStackItemType>? cloneFunction = null)
    {
        if (capacity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(capacity), "Capacity must be greater than zero.");
        }

        Capacity = capacity;
        _cloneFunction = cloneFunction;
        Initialize();
    }

    public int Count { get; private set; }

    public int Capacity { get; }

    public void Push(TStackItemType item)
    {
        lock (_syncRoot)
        {
            if (Count == Capacity)
            {
                _top = (_top + 1) % Capacity;
            }
            else
            {
                _top = (_top + 1) % Capacity;
                Count++;
            }

            _buffer[_top] = CloneItem(item);
        }
    }

    public TStackItemType Pop()
    {
        lock (_syncRoot)
        {
            if (Count == 0)
            {
                throw new InvalidOperationException("The stack is empty.");
            }

            var item = _buffer[_top];
            _top = (_top - 1 + Capacity) % Capacity;
            Count--;

            return item;
        }
    }

    public TStackItemType Peek()
    {
        lock (_syncRoot)
        {
            if (Count == 0)
            {
                throw new InvalidOperationException("The stack is empty.");
            }

            return _buffer[_top];
        }
    }

    public bool IsEmpty()
    {
        return Count == 0;
    }

    public bool IsFull()
    {
        return Count == Capacity;
    }

    public void Clear()
    {
        Initialize();
    }

    private void Initialize()
    {
        _buffer = new TStackItemType[Capacity];
        Count = 0;
        _top = -1;
    }

    private TStackItemType CloneItem(TStackItemType item)
    {
        // If a custom clone function is provided, use it
        if (_cloneFunction != null)
        {
            return _cloneFunction(item);
        }

        // If the item is null, return as-is
        if (item == null)
        {
            return item;
        }

        // If the item implements ICloneable, use it
        if (item is ICloneable cloneable)
        {
            return (TStackItemType)cloneable.Clone();
        }

        // For value types, return as-is (they are copied by value anyway)
        var itemType = typeof(TStackItemType);
        if (itemType.IsValueType || itemType == typeof(string))
        {
            return item;
        }

        // For reference types, try JSON serialization as a fallback
        try
        {
            var json = JsonSerializer.Serialize(item);
            return JsonSerializer.Deserialize<TStackItemType>(json)!;
        }
        catch (NotSupportedException ex)
        {
            // If JSON serialization fails, return the original item
            // This maintains backward compatibility but the caller should provide a clone function
            return item;
        }
    }
}
