using System;
using System.Collections.Generic;

using UnityEngine;

public class GenericPool<T> where T : Component
{
    private readonly Func<T> createFunc;
    private readonly Action<T> onGet;
    private readonly Action<T> onRelease;
    private readonly Action<T> onDestroy;

    private readonly Stack<T> objects;
    private readonly int maxSize;

    public GenericPool(Func<T> createFunc, Action<T> onGet = null, Action<T> onRelease = null, Action<T> onDestroy = null, int initialCapacity = 10, int maxSize = 50)
    {
        if (createFunc == null) throw new ArgumentNullException(nameof(createFunc));

        this.createFunc = createFunc;
        this.onGet = onGet;
        this.onRelease = onRelease;
        this.onDestroy = onDestroy;
        this.maxSize = maxSize;

        objects = new Stack<T>(initialCapacity);

        for (int i = 0; i < initialCapacity; i++)
        {
            objects.Push(createFunc());
        }
    }

    public T Get()
    {
        T obj = objects.Count > 0 ? objects.Pop() : createFunc();
        onGet?.Invoke(obj);
        return obj;
    }

    public void Release(T obj)
    {
        if (objects.Count < maxSize)
        {
            onRelease?.Invoke(obj);
            objects.Push(obj);
        }
        else
        {
            onDestroy?.Invoke(obj);
        }
    }
}
