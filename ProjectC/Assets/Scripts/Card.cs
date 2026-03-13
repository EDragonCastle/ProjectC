using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Cysharp.Threading.Tasks;

public class Card : MonoBehaviour, IObject
{
    [SerializeField]
    private int cardIndex;

    public DeckCard deckCardScript;
    private readonly int maxCard = 8;
    private int key;

    [Header("Page")]
    public GameObject page;
    public CardData cardData;

    public GameObject cardObject;
    public GameObject deckObject;
    
    [Header("Card Data")]
    public Image cardImage;
    public TextMeshProUGUI cardExplanation;
    public Image gem;
    public GameObject legandPortrait;
    public TextMeshProUGUI cost;
    public TextMeshProUGUI cardName;
    public TextMeshProUGUI attack;
    public TextMeshProUGUI health;

    [Header("Deck Data")]
    public Image deckImage;
    public TextMeshProUGUI deckManaCost;
    public TextMeshProUGUI deckName;


    #region IObject Interface
    public void OnSpawn()
    {

    }
    
    public void OnDespawn()
    {

    }

    // origin key값 세팅
    public int GetObjectKey() => key;

    public void SetObjectKey(int _key) => key = _key;

    public void SetParent(Transform parent)
    {
        transform.SetParent(parent);
        transform.localPosition = Vector3.zero;
        transform.localScale = Vector3.one;
        transform.localRotation = Quaternion.identity;
    }

    public void SetTransform(Transform transform, Transform parent)
    {
        transform.SetParent(parent);

        this.transform.position = transform.position;
        this.transform.rotation = transform.rotation;
        this.transform.localScale = Vector3.one;
    }
    public Transform GetTransform() => transform;
    #endregion

    private async void Start()
    {
        await UniTask.WaitUntil(() => GameManager.isReadyGameManager);
        await CardSetting();
    }

    public async UniTask CardSetting()
    {
        if (page == null)
            return;

        var pageInfoComponent = page.GetComponent<PageInformation>();
        int currentPage = pageInfoComponent.page;

        DataManager dataManager = Locator<DataManager>.Get();
        ResourceManager resourceManager = Locator<ResourceManager>.Get();
        var sortCardList = dataManager.GetSortCardData();

        // outofIndex를 대비해야 한다.
        if(sortCardList.Count <= currentPage * maxCard + cardIndex || currentPage * maxCard + cardIndex < 0)
        {
            Debug.Log("Out of Index다.");
            cardObject.SetActive(false);
            deckObject.SetActive(false);
            return;
        }

        CardData _cardData = sortCardList[currentPage * maxCard + cardIndex];
        cardData = _cardData;
        // Resource를 Addressable에서 다운을 받아야 한다.
        var cardSpriteTask = resourceManager.Get<Sprite>(cardData.spriteName);
        var gemSpriteTask = resourceManager.Get<Sprite>(cardData.gem);

        cardName.text = cardData.cardName;
        cost.text = cardData.cost.ToString();
        attack.text = cardData.attack.ToString();
        health.text = cardData.health.ToString();
        cardExplanation.text = cardData.description;
        deckManaCost.text = cost.text;
        deckName.text = cardName.text;

        var (cardSprite, gemSprite) = await UniTask.WhenAll(cardSpriteTask, gemSpriteTask);

        cardImage.sprite = cardSprite;
        deckImage.sprite = cardSprite;
        gem.sprite = gemSprite;
    }

    public async UniTask CardSetting(int index)
    {
        DataManager dataManager = Locator<DataManager>.Get();
        ResourceManager resourceManager = Locator<ResourceManager>.Get();
        var sortCardList = dataManager.GetSortCardData();

        // outofIndex를 대비해야 한다.
        if (sortCardList.Count <= index * maxCard + cardIndex || index * maxCard + cardIndex < 0)
        {
            Debug.Log("Out of Index다.");
            cardObject.SetActive(false);
            deckObject.SetActive(false);
            return;
        }

        cardObject.SetActive(true);
        deckObject.SetActive(false);

        CardData _cardData = sortCardList[index * maxCard + cardIndex];
        cardData = _cardData;
        // Resource를 Addressable에서 다운을 받아야 한다.
        var cardSpriteTask = resourceManager.Get<Sprite>(cardData.spriteName);
        var gemSpriteTask = resourceManager.Get<Sprite>(cardData.gem);

        cardName.text = _cardData.cardName;
        cost.text = _cardData.cost.ToString();
        attack.text = _cardData.attack.ToString();
        health.text = _cardData.health.ToString();
        cardExplanation.text = _cardData.description;
        deckManaCost.text = cost.text;
        deckName.text = cardName.text;

        var (cardSprite, gemSprite) = await UniTask.WhenAll(cardSpriteTask, gemSpriteTask);

        cardImage.sprite = cardSprite;
        deckImage.sprite = cardSprite;
        gem.sprite = gemSprite;
    }

    public async UniTask CardSetting(CardData _cardData)
    {
        ResourceManager resourceManager = Locator<ResourceManager>.Get();

        var cardSpriteTask = resourceManager.Get<Sprite>(_cardData.spriteName);
        var gemSpriteTask = resourceManager.Get<Sprite>(_cardData.gem);

        cardData = _cardData;

        cardName.text = _cardData.cardName;
        cost.text = _cardData.cost.ToString();
        attack.text = _cardData.attack.ToString();
        health.text = _cardData.health.ToString();
        cardExplanation.text = _cardData.description;
        deckManaCost.text = cost.text;
        deckName.text = cardName.text;

        var (cardSprite, gemSprite) = await UniTask.WhenAll(cardSpriteTask, gemSpriteTask);

        cardImage.sprite = cardSprite;
        deckImage.sprite = cardSprite;
        gem.sprite = gemSprite;

        cardObject.SetActive(false);
        deckObject.SetActive(true);

        // 이거 설정을 해줘야 버그가 없어져서 해야 한다.
        var uiManager = Locator<UIManager>.Get();
        deckCardScript.canvasParent = uiManager.GetCollectionCanvas().GetComponent<RectTransform>();
    }

    public void ReleaseCard(int index)
    {
        DataManager dataManager = Locator<DataManager>.Get();
        ResourceManager resourceManager = Locator<ResourceManager>.Get();
        var sortCardList = dataManager.GetSortCardData();

        if (sortCardList.Count <= index * maxCard + cardIndex || index * maxCard + cardIndex < 0) {
            return;
        }

        CardData _cardData = sortCardList[index * maxCard + cardIndex];
        resourceManager.Release(_cardData.spriteName);
        resourceManager.Release(_cardData.gem);
    }

    public void ActiveCards(bool isCardActive)
    {
        cardObject.SetActive(isCardActive);
        deckObject.SetActive(!isCardActive);
    }

    public void ActiveCards(bool cardActive, bool deckActive)
    {
        cardObject.SetActive(cardActive);
        deckObject.SetActive(deckActive);
    }

}


