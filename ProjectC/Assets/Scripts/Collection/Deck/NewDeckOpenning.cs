using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Cysharp.Threading.Tasks;

public class NewDeckOpenning : MonoBehaviour
{
    public GameObject deck;
    public GameObject textObject;
    public Image deckImage;
    private float duration = 0.2f;

    private Vector3 initPosition;
    private float offset = 20.0f;

    // Deck Title¿¡ ´Þ±î?
    private async void OnEnable()
    {
        deck.SetActive(false);
        textObject.SetActive(true);
        var rectTransform = this.GetComponent<RectTransform>();
        initPosition = new Vector3(rectTransform.localPosition.x, rectTransform.localPosition.y, rectTransform.localPosition.z);

        await UniTask.WaitUntil(() => GameManager.isReadyGameManager);
        await Openning();
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
        sequence.Append(rectTransform.DORotate(new Vector3(0, 180, 0), duration / 2).SetEase(Ease.Linear));
        sequence.Append(rectTransform.DORotate(new Vector3(0, 360, 0), duration / 2).SetEase(Ease.Linear));
        sequence.AppendCallback(() => {
            textObject.SetActive(false);
            deck.SetActive(true);
        });

        sequence.OnComplete(() => { rectTransform.localPosition = initPosition; });
    }

}
