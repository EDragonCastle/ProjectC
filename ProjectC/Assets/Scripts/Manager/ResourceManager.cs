using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;

/// <summary>
/// 실제 현업에서는 서버가 있어서 해당 서버에 Addressable Asset을 넣고 불러오거나 불러오지 않거나 할 것인데 
/// 지금은 Addressable Asset의 Local로 관리해서 진행하자.
/// </summary>
public class ResourceManager
{
    private Dictionary<string, AsyncOperationHandle> handle;

    public void Initalize()
    {
        handle = new Dictionary<string, AsyncOperationHandle>();
    }

    public void Release(string assetName)
    {
        if(handle.TryGetValue(assetName, out var _handle))
        {
            Addressables.Release(_handle);
            handle.Remove(assetName);
        }
    }


    public async UniTask<T> Get<T>(string assetName) where T : Object
    {
        if(handle.TryGetValue(assetName, out var _handle))
        {
            if (_handle.IsDone)
                return (T)_handle.Result;

            await _handle.Convert<T>().ToUniTask();
            return (T)_handle.Result;
        }

        var loadHandle = Addressables.LoadAssetAsync<T>(assetName);
        handle[assetName] = loadHandle;

        try
        {
            return await loadHandle.ToUniTask();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Resource Manager에서 {assetName}를 실패했습니다. : {e.Message}");
            handle.Remove(assetName);
            return null;
        }
    }
}
