using UnityEngine;
using Cysharp.Threading.Tasks;

public class ProvideCollection : MonoBehaviour
{
    private async void Start()
    {
        await UniTask.WaitUntil(() => GameManager.isReadyGameManager);

        var uiManager = Locator<UIManager>.Get();
        uiManager.SetCollectionCanvas(this.gameObject);
    }
}
