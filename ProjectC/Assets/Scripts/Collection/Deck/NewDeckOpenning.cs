using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using TMPro;

public class NewDeckOpenning : MonoBehaviour
{
    public GameObject deck;
    public GameObject textObject;
    public Image deckImage;
    public TextMeshProUGUI deckNameText;
    private float duration = 0.2f;
    public bool isOpenning = true;

    private Vector3 initPosition;
    private float offset = 20.0f;

    // Deck Title에 달까?
    private async void OnEnable()
    {
        // 이 기능을 사용하고 싶지 않으면
        // 쉬운 방법은 DataManager에 어떤 Data를 넣고 Load를 막는다.
        // 아니면 다른 방식이 필요한데 어떻게 해야할까?
        deck.SetActive(false);
        textObject.SetActive(true);

        var rectTransform = this.GetComponent<RectTransform>();
        initPosition = new Vector3(rectTransform.localPosition.x, rectTransform.localPosition.y, rectTransform.localPosition.z);

        await UniTask.WaitUntil(() => GameManager.isReadyGameManager);

        if (isOpenning)
            await Openning();
        else
            ReconstractDeck();
    }

    private async UniTask Openning()
    {
        var dataManager = Locator<DataManager>.Get();
        var resourceManager = Locator<ResourceManager>.Get();

        uint heroIndex = dataManager.GetHeroIndex();
        if(heroIndex != 0)
        {
            var heroData = dataManager.GetHeroData();
            var heroSprite = await resourceManager.Get<Sprite>(heroData[heroIndex].heroSprite);
            if(heroSprite != null)
                deckImage.sprite = heroSprite;
        }
        
        DeckInitalize();
    }

    private void DeckInitalize()
    {
        var rectTransform = this.GetComponent<RectTransform>();
        rectTransform.localPosition = new Vector3(initPosition.x, initPosition.y - offset, initPosition.z);
        rectTransform.localRotation = Quaternion.identity;
        rectTransform.DOKill();

        DG.Tweening.Sequence sequence = DOTween.Sequence();
        sequence.Join(rectTransform.DOLocalMove(initPosition, duration / 2).SetEase(Ease.InQuad));
        sequence.Append(rectTransform.DORotate(new Vector3(180, 0, 0), duration / 2).SetEase(Ease.Linear));
        sequence.AppendCallback(() => {
            textObject.SetActive(false);
            deck.SetActive(true);
        });
        sequence.Append(rectTransform.DORotate(new Vector3(360, 0, 0), duration / 2).SetEase(Ease.Linear));
        sequence.OnComplete(() => { rectTransform.localPosition = initPosition; });
    }

    private void ReconstractDeck()
    {
        // 재 조립
        // Openning 결과와 달리 실행되는 함수 일단은 지금은 그냥 가져왔다.
        Debug.Log("Setup");

        var rectTransform = this.GetComponent<RectTransform>();
        rectTransform.localPosition = new Vector3(initPosition.x, initPosition.y - offset, initPosition.z);
        rectTransform.localRotation = Quaternion.identity;
        rectTransform.DOKill();

        textObject.SetActive(false);
        deck.SetActive(true);

        DG.Tweening.Sequence sequence = DOTween.Sequence();
        sequence.Join(rectTransform.DOLocalMove(initPosition, duration / 2).SetEase(Ease.InQuad));
        sequence.Append(rectTransform.DORotate(new Vector3(180, 0, 0), duration / 2).SetEase(Ease.Linear));
        sequence.Append(rectTransform.DORotate(new Vector3(360, 0, 0), duration / 2).SetEase(Ease.Linear));

        sequence.OnComplete(() => { rectTransform.localPosition = initPosition; });

        isOpenning = true;
    }

}
