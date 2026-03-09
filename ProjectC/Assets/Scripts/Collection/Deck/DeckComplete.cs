using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;

using TMPro;

public class DeckComplete : MonoBehaviour
{
    // Complete Button을 눌렀을 때 Deck Scroll 안에 들어가야 한다.
    public GameObject prefab;

    // 일단 text만 deck information은 이미지 parser로 받아오고 한 다음에 하자.
    //public GameObject copyOrigin;
    public TextMeshProUGUI originDeckName;

    public GameObject newDeck;
    public GameObject deckList;

    public RectTransform title;
    public Transform collection;
    public Transform deckListViewPort;

    private Vector3 initPosition;
    public float duration = 0.2f;
    public float offset = 0.4f;

    public async void Complete()
    {
        var dataManager = Locator<DataManager>.Get();
        var resourceManager = Locator<ResourceManager>.Get();
        var cardList = dataManager.GetHeroData();
        var cardIndex = dataManager.GetHeroIndex();

        var heroSprite = await resourceManager.Get<Sprite>(cardList[cardIndex].heroSprite);

        SettingNewDeck(heroSprite);
    }

    private void SettingNewDeck(Sprite heroSprite)
    {
        // 음 Prefab을 만드는 데 width랑 height를 조절해야 할 것 같은데
        var deckObject = Instantiate(prefab, collection);
        var rectTransform = deckObject.GetComponent<RectTransform>();

        rectTransform.position = title.position;
        rectTransform.localScale = new Vector3(1.24f, 1.4f, 1f);

        // Scale을 1로 되돌리고 새로운 덱 위치는 고정인것 같다.
        DG.Tweening.Sequence sequence = DOTween.Sequence();

        sequence.Append(rectTransform.DOScale(Vector3.one, duration).SetEase(Ease.InBack));
        sequence.Join(rectTransform.DOMove(new Vector3(title.position.x, title.position.y-offset, title.position.z), duration).SetEase(Ease.InBack));

        var deckComponent = deckObject.GetComponent<Deck>();
        deckComponent.deckImage.sprite = heroSprite;
        deckComponent.deckName.text = originDeckName.text;

        // 여기인데 
        sequence.OnComplete(() => {
            newDeck.SetActive(false);
            deckList.SetActive(true);
            // 추가를 해도 괜찮다. deck button이 사라져서 괜찮다.
            deckObject.transform.SetParent(deckListViewPort);
            var eventManager = Locator<EventManager>.Get();
            eventManager.Notify(ChannelInfo.InputDeckList);
            eventManager.Notify(ChannelInfo.SelectingDeck, false);
        });
        // Input
    }

}

