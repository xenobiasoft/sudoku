namespace XenobiaSoft.Sudoku.GameState;

public class CircularStack<TStackItemType>
{
    private TStackItemType[] _buffer;
    private int _size;
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

    public int Count => _size;

    public int Capacity { get; }

    public void Push(TStackItemType item)
    {
        lock (_buffer)
        {
            if (_size == Capacity)
            {
                _top = (_top + 1) % Capacity;
            }
            else
            {
                _top = (_top + 1) % Capacity;
                _size++;
            }

            _buffer[_top] = item;
        }
    }

    public TStackItemType Pop()
    {
        if (_size == 0)
        {
            throw new InvalidOperationException("The stack is empty.");
        }

        var item = _buffer[_top];
        _top = (_top - 1 + Capacity) % Capacity;
        _size--;

        return item;
    }

    public TStackItemType Peek()
    {
        if (_size == 0)
        {
            throw new InvalidOperationException("The stack is empty.");
        }

        return _buffer[_top];
    }

    public bool IsEmpty()
    {
        return _size == 0;
    }

    public bool IsFull()
    {
        return _size == Capacity;
    }

    public void Clear()
    {
        Initialize();
    }

    private void Initialize()
    {
        _buffer = new TStackItemType[Capacity];
        _size = 0;
        _top = -1;
    }
}
