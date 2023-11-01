using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class CommandQueue
{
    private List<CommandInstance> _cmdQueue;
    private int _maxSize = 5;
    private GameActor _actor;

    public CommandQueue(GameActor actor, int maxSize = 5)
    {
        _maxSize = maxSize;
        _cmdQueue = new List<CommandInstance>();
        _actor = actor;
    }

    public void Clear()
    {
        _cmdQueue.Clear();
    }

    public bool Add(CommandInstance cmdInstance)
    {
        _cmdQueue.Add(cmdInstance);
        if (Size() > MaxSize)
        {
            PopFront();
        }

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
        return _cmdQueue[0];
    }

    public CommandInstance PopFront()
    {
        if (!Empty())
        {
            var res = _cmdQueue[0];
            _cmdQueue.RemoveAt(0);
            return res;
        }
        else return null;
    }

    public CommandInstance PopBack()
    {
        if (!Empty())
        {
            var res = _cmdQueue[Size() - 1];
            _cmdQueue.RemoveAt(Size() - 1);
            return res;
        }
        else return null;
    }

    public void Undo()
    {
        var cmd = PopBack();
        if (cmd != null)
        {
            cmd.Undo(_actor);
        }
    }

    public bool Empty() => _cmdQueue.Count == 0;
    public int Size() => _cmdQueue.Count;
    public int MaxSize => _maxSize;
}