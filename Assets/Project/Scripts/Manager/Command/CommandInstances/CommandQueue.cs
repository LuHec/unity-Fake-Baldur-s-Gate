using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;

public class CommandQueue
{
    private Queue<CommandInstance> _cmdQueue;
    private int _maxSize = 5;

    public CommandQueue()
    {
        _cmdQueue = new Queue<CommandInstance>();
    }
    
    public CommandQueue(int maxSize)
    {
        _maxSize = maxSize;
        _cmdQueue = new Queue<CommandInstance>();
    }

    public bool Add(CommandInstance cmdInstance)
    {
        _cmdQueue.Enqueue(cmdInstance);
        if (Size() > MaxSize) _cmdQueue.Dequeue();
        return true;
    }

    public CommandInstance Back()
    {
        if (Empty()) return null;
        return _cmdQueue.Last();
    }

    public CommandInstance Front()
    {
        if (Empty()) return null;
        return _cmdQueue.Peek();
    }

    public CommandInstance PopFront()
    {
        if (Empty()) return _cmdQueue.Dequeue();
        else return null;
    }

    public bool Empty() => _cmdQueue.Count == 0;
    public int Size() => _cmdQueue.Count;
    public int MaxSize => _maxSize;
}