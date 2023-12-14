using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

public class TListQueue<T>: IEnumerable<T>
{
    private List<T> queue;
    public int Count => queue.Count;

    public T this[int i]
    {
        get => queue[i];
        set => queue[i] = value;
    }

    public TListQueue()
    {
        queue = new List<T>();
    }

    public void Enqueue(T item)
    {
        queue.Add(item);
    }

    public T Dequeue()
    {
        T front = queue[0];
        queue.RemoveAt(0);
        return front;
    }

    public bool Remove(T item)
    {
        return queue.Remove(item);
    }
    
    public void Sort(System.Comparison<T> comparison)
    {
        queue.Sort(comparison);
    }

    public bool Contain(T item)
    {
        return queue.Contains(item);
    }

    public IEnumerator<T> GetEnumerator()
    {
        return queue.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public List<T> ToList()
    {
        return queue.ToList();
    }
}