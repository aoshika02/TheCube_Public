using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GenericObjectPool<T> where T : MonoBehaviour, IPool
{
    protected readonly GameObject _instance;
    protected readonly List<T> _instances = new List<T>();
    protected readonly Queue<T> _available = new Queue<T>();
    protected readonly Transform _parent;

    public GenericObjectPool(GameObject instance, Transform parent)
    {
        _instance = instance;
        _parent = parent;
    }

    public T Get()
    {
        T instance;
        if (_available.Count > 0)
        {
            instance = _available.Dequeue();
        }
        else
        {
            instance = Create();
        }
        instance.IsGenericUse = true;
        instance.OnReuse();
        return instance;
    }

    private T Create()
    {
        T instance = Object.Instantiate(_instance, _parent).GetComponent<T>();
        _instances.Add(instance);
        return instance;
    }
    public void Release(T instance)
    {
        instance.OnRelease();
        instance.IsGenericUse = false;
        _available.Enqueue(instance);
    }
    public void ReleaseAll()
    {
        foreach (var instance in _instances)
        {
            Release(instance);
        }
    }
}
