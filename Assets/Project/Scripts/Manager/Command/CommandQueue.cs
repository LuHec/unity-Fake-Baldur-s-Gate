using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class CommandQueue
{
    public CommandInstance CommandCache = null;
    private List<CommandInstance> cmdQueue;
    private int maxSize = 5;
    private GameActor actor;

    public CommandQueue(GameActor actor, int maxSize = 5)
    {
        this.maxSize = maxSize;
        cmdQueue = new List<CommandInstance>();
        this.actor = actor;
    }

    public void Clear()
    {
        cmdQueue.Clear();
    }

    public bool Add(CommandInstance cmdInstance)
    {
        CommandCache = cmdInstance;
        cmdQueue.Add(cmdInstance);
        if (Size() > MaxSize)
        {
            PopFront();
        }

        return true;
    }

    public CommandInstance Back()
    {
        if (Empty()) return null;
        return cmdQueue.Last();
    }

    public CommandInstance Front()
    {
        if (Empty()) return null;
        return cmdQueue[0];
    }

    public CommandInstance PopFront()
    {
        if (!Empty())
        {
            var res = cmdQueue[0];
            cmdQueue.RemoveAt(0);
            return res;
        }
        else return null;
    }

    public CommandInstance PopBack()
    {
        if (!Empty())
        {
            var res = cmdQueue[Size() - 1];
            cmdQueue.RemoveAt(Size() - 1);
            return res;
        }
        else return null;
    }

    public void Undo()
    {
        var cmd = PopBack();
        if (cmd != null)
        {
            cmd.Undo(actor);
        }
    }

    public bool Empty() => cmdQueue.Count == 0;
    public int Size() => cmdQueue.Count;
    public int MaxSize => maxSize;
}