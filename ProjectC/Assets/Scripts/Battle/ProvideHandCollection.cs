using UnityEngine;

public class ProvideHandCollection : MonoBehaviour
{
    private async void Start()
    {
        await Cysharp.Threading.Tasks.UniTask.WaitUntil(() => GameManager.isReadyGameManager);

        var battleManager = Locator<BattleManager>.Get();
        battleManager.SetHandPanel(this.gameObject);
        this.gameObject.SetActive(false);
    }
}
