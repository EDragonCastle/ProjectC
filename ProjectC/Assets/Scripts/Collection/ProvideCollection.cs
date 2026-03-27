using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;

public class ProvideCollection : MonoBehaviour
{
    public GameObject backGround;

    private readonly float originScale = 0.7f;
    private float duration = 0.7f;

    private async void Start()
    {
        await UniTask.WaitUntil(() => GameManager.isReadyGameManager);

        var uiManager = Locator<UIManager>.Get();
        uiManager.SetCollectionCanvas(this.gameObject);
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

    public void ExitCollection()
    {
        var backGroundTransform = backGround.GetComponent<RectTransform>();

        DG.Tweening.Sequence sequence = DOTween.Sequence();
        sequence.Append(backGroundTransform.DOScale(originScale, duration));
        sequence.OnComplete(() => { this.gameObject.SetActive(false); });
    }
}
