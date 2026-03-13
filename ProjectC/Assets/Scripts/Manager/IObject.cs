using UnityEngine;

/// <summary>
/// 공통된 Object Interface
/// </summary>
public interface IObject
{
    public void OnSpawn();
    public void OnDespawn();
    
    // origin key값 세팅
    public int GetObjectKey();
    public void SetObjectKey(int _key); 

    public void SetTransform(Transform transform, Transform parent);
    public void SetParent(Transform parent);
    public Transform GetTransform();
}
