using UnityEngine;
using DG.Tweening;

public class Battle : MonoBehaviour
{
    public GameObject backGround;

    private float originScale = 0.7f;
    private float duration = 1.0f;

    private async void Start()
    {
        await Cysharp.Threading.Tasks.UniTask.WaitUntil(() => GameManager.isReadyGameManager);

        var uiManager = Locator<UIManager>.Get();
        uiManager.SetBattle(this.gameObject);
    }

    private void OnEnable()
    {
        var backGroundTransform = backGround.GetComponent<RectTransform>();
        backGroundTransform.DOScale(1.0f, duration);
    }

    private void OnDisable()
    {
        var backGroundTransform = backGround.GetComponent<RectTransform>();
        backGroundTransform.localScale = new Vector3(originScale, originScale, originScale);
    }

    public void ExitButton()
    {
        var uiManager = Locator<UIManager>.Get();
        var lobby = uiManager.GetLobby();
        lobby.SetActive(true);

        var backGroundTransform = backGround.GetComponent<RectTransform>();
        backGroundTransform.DOScale(originScale, duration * 1.5f).OnComplete(() => { this.gameObject.SetActive(false); });
    }

    public void CollectionButton()
    {
        var uiManager = Locator<UIManager>.Get();
        var collection = uiManager.GetCollectionCanvas();

        collection.SetActive(true);


        // 수집품 눌렀을 때 어떻게 이동하는지 봐야한다.
        // 문을 닫고 다시 여는 것을 볼 수 있다. 구현해야 한다.

    }

}
