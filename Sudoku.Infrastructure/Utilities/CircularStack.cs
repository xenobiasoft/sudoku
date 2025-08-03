namespace Sudoku.Infrastructure.Utilities;

public class CircularStack<TStackItemType>
{
    private TStackItemType[] _buffer;
    private int _top;

    public CircularStack(int capacity)
    {
        if (capacity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(capacity), "Capacity must be greater than zero.");
        }

        Capacity = capacity;
        Initialize();
    }

    public int Count { get; private set; }

    public int Capacity { get; }

    public void Push(TStackItemType item)
    {
        lock (_buffer)
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

            _buffer[_top] = item;
        }
    }

    public TStackItemType Pop()
    {
        lock (_buffer)
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
        lock (_buffer)
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
}
