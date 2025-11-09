using UnityEngine;
using UnityEngine.Pool;

/// <summary>
/// Abstract generic pool for MonoBehaviour objects.
/// Handles lazy loading and object reuse.
/// </summary>
public abstract class GenericPool<T> where T : MonoBehaviour
{
    private readonly IObjectPool<T> pool;

    protected GenericPool(int initialCapacity = 0, int maxSize = 50)
    {
        pool = new ObjectPool<T>(
            createFunc: Create,
            actionOnGet: OnGet,
            actionOnRelease: OnRelease,
            actionOnDestroy: OnDestroy,
            collectionCheck: true,
            defaultCapacity: initialCapacity,
            maxSize: maxSize
        );
    }

    /// <summary> Called when a new object is needed. </summary>
    protected abstract T Create();

    /// <summary> Called when an object is retrieved from the pool. </summary>
    protected abstract void OnGet(T item);

    /// <summary> Called when an object is returned to the pool. </summary>
    protected abstract void OnRelease(T item);

    /// <summary> Called when an object is destroyed by the pool. </summary>
    protected abstract void OnDestroy(T item);

    public T Get() => pool.Get();
    public void Release(T item) => pool.Release(item);
}
