using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;


public interface IFrontier<T>
{
    void Add(T item);
    T Remove();
    bool isEmpty();
}

public class QueueFrontier<T> : IFrontier<T>
{
    private readonly Queue<T> queue = new Queue<T>();
    public void Add(T item) => queue.Enqueue(item);
    public T Remove() => queue.Dequeue();
    public bool isEmpty() => queue.Count > 0;
}

public class StackFrontier<T> : IFrontier<T>
{
    private readonly Stack<T> stack = new Stack<T>();
    public void Add(T item) => stack.Push(item);
    public T Remove() => stack.Pop();
    public bool isEmpty() => stack.Count > 0;
}
