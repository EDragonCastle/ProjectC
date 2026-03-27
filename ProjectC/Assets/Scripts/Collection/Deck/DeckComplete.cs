using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;

using TMPro;

public class DeckComplete : MonoBehaviour
{
    // Complete Button을 눌렀을 때 Deck Scroll 안에 들어가야 한다.
    public bool isResetting = false;

    public GameObject prefab;
    public GameObject origin;

    public DeckViewPort deckViewPort;
    public NewDeckOpening deckOpening;
    public TextMeshProUGUI originDeckName;

    public GameObject newDeck;
    public GameObject deckList;

    public RectTransform title;
    public Transform collection;
    public Transform deckListViewPort;

    public Vector3 initPosition;
    public float duration = 0.2f;
    public float offset = 0.4f;


    public async void Complete()
    {
        if(!isResetting)
        {
            var dataManager = Locator<DataManager>.Get();
            var resourceManager = Locator<ResourceManager>.Get();

            var cardList = dataManager.GetHeroData();
            var cardIndex = dataManager.GetHeroIndex();
            
            var heroSprite = await resourceManager.Get<Sprite>(cardList[cardIndex].heroSprite);
            SettingNewDeck(heroSprite, cardIndex);
        }
        else
        {
            ResettingNewDeck();
        }

        var eventManager = Locator<EventManager>.Get();
        string[] none = { "All" };
        FilterParameter parameter = new FilterParameter(FilterType.Search, _job: none);
        eventManager.Notify(ChannelInfo.Filter, parameter);
    }

    private void SettingNewDeck(Sprite heroSprite, uint heroIndex)
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
            var deckComponent = deckObject.GetComponent<Deck>();

            if (deckComponent != null && deckOpening != null) {
                DeckInformation deckInformation = new DeckInformation();
                deckInformation.deckImage = heroSprite;
                deckInformation.heroIndex = heroIndex;
                deckInformation.deckName = originDeckName.text;
                deckInformation.deckData = deckViewPort.GetDeckData();
                deckInformation.currentCard = deckViewPort.GetCurrentCard();
                deckInformation.maxCard = deckViewPort.GetMaxCard();

                deckComponent.SettingDeck(deckInformation);
            }

            newDeck.SetActive(false);
            deckList.SetActive(true);

            // 추가를 해도 괜찮다. deck button이 사라져서 괜찮다.
            deckObject.transform.SetParent(deckListViewPort);
            var eventManager = Locator<EventManager>.Get();
            eventManager.Notify(ChannelInfo.InputDeckList);
            eventManager.Notify(ChannelInfo.SelectingDeck, false);
        });
    }

    public void ResetSetup(GameObject deck, Vector3 _initPosition)
    {
        origin = deck;
        initPosition = _initPosition;
        isResetting = true;
    }

    // 필요한 걸 적어보자.
    // Deck이 원래 있었던 Position 
    // Complete를 눌렀을 때 Deck Object -> collection parent를 가져야 한다.
    private void ResettingNewDeck()
    {
        int myindex = origin.transform.GetSiblingIndex();
        if (myindex <= 0)
            myindex = 0;

        // deck position
        origin.transform.SetParent(collection);
        var rect = origin.GetComponent<RectTransform>();
        rect.position = title.position;
        rect.localScale = new Vector3(1.24f, 1.4f, 1f);

        // 음 Prefab을 만드는 데 width랑 height를 조절해야 할 것 같은데
        var rectTransform = origin.GetComponent<RectTransform>();

        // Scale을 1로 되돌리고 새로운 덱 위치는 고정인것 같다.
        DG.Tweening.Sequence sequence = DOTween.Sequence();

        sequence.Append(rect.DOScale(Vector3.one, duration).SetEase(Ease.InBack));
        sequence.Join(rect.DOMove(new Vector3(initPosition.x, initPosition.y, initPosition.z), duration).SetEase(Ease.InBack));

        var deckComponent = origin.GetComponent<Deck>();
        deckComponent.deckName.text = originDeckName.text;

        // 여기인데 
        sequence.OnComplete(() => {
            var deckComponent = origin.GetComponent<Deck>();

            if (deckComponent != null && deckOpening != null)
            {
                DeckInformation deckInformation = new DeckInformation();
                deckInformation.deckImage = deckComponent.deckImage.sprite;
                deckInformation.deckName = originDeckName.text;
                deckInformation.deckData = deckViewPort.GetDeckData();
                deckInformation.currentCard = deckViewPort.GetCurrentCard();
                deckInformation.maxCard = deckViewPort.GetMaxCard();
                deckInformation.heroIndex = deckOpening.deckHover.heroIndex;

                deckComponent.SettingDeck(deckInformation);
            }

            newDeck.SetActive(false);
            deckList.SetActive(true);

            // 추가를 해도 괜찮다. deck button이 사라져서 괜찮다.
            origin.transform.SetParent(deckListViewPort);
            origin.transform.SetSiblingIndex(myindex);
            var eventManager = Locator<EventManager>.Get();
            eventManager.Notify(ChannelInfo.SelectingDeck, false);
            isResetting = false;
        });
    }
}

