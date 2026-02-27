using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Object를 생성하고 반납하는 곳을 담당하는 Class다.
/// </summary>
public class Factory
{
    private Dictionary<int, object> poolDictionary = new Dictionary<int, object>();

    public T Create<T>(T prefab, Transform transform, Transform parent = null) where T : Component, IObject
    {
        // 생성할 때는 무조건 원본 Prefab이 들어올 것이기 때문에 id를 가져와도 된다.
        int key = prefab.gameObject.GetInstanceID();

        if(!poolDictionary.ContainsKey(key)) {
            poolDictionary.Add(key, new ObjectPool<T>(prefab, 10));
        }

        var pool = (ObjectPool<T>)poolDictionary[key];
        T poolingObject = pool.Get();

        poolingObject.SetObjectKey(key);

        poolingObject.gameObject.SetActive(true);
        poolingObject.SetTransform(transform, parent);

        poolingObject.OnSpawn();

        return poolingObject;
    }

    public void Release<T>(T instance) where T : Component, IObject
    {
        int key = instance.GetObjectKey();

        if (poolDictionary.ContainsKey(key))
        {
            var pool = (ObjectPool<T>)poolDictionary[key];
            pool.Return(instance);
        }
        else
            GameObject.Destroy(instance.gameObject);
    }
}
