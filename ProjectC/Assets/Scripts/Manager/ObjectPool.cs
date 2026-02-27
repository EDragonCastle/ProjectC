using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Object를 관리하는 공간
/// SetActive True, False로 관리한다.
/// </summary>
/// <typeparam name="T">Object Type</typeparam>
public class ObjectPool<T> where T : Component, IObject
{
    private Stack<T> pool = new Stack<T>();
    private T prefab;
    private readonly int initalizeLength;
    private Transform parent;

    // Object Pool 생성자
    #region AddressObject Object Pool Consgtrcut
    /// <summary>
    /// Object Pool 생성자를 담당한다.
    /// </summary>
    public ObjectPool(T _prefab, int length = 5, Transform _parent = null)
    {
        prefab = _prefab;
        initalizeLength = length;
        parent = _parent;
        Initialize();
    }
    #endregion

    // Type에 받는 Object를 가져온다.
    public T Get()
    {
        T poolObject = (pool.Count > 0) ? pool.Pop() : CreateNewObject();
        return poolObject;
    }

    /// <summary>
    /// Object를 Pool로 반납한다.
    /// </summary>
    /// <param name="destoryObject">반납할 object</param>
    public void Return(T destoryObject)
    {
        destoryObject.OnDespawn();
        destoryObject.gameObject.SetActive(false);
        pool.Push(destoryObject);
    }

    // 생성자에서 실행할 함수
    private void Initialize()
    {
        // 초기 setting이 있으니까 미리 생성하고 active false를 해야 하지 않나?
        for(int i = 0; i < initalizeLength; i++)
        {
            T newObject = CreateNewObject();
            pool.Push(newObject);
        }
    }

    private T CreateNewObject()
    {
        T newObject = GameObject.Instantiate(prefab, parent);
        newObject.gameObject.SetActive(false);
        return newObject;
    }
}
