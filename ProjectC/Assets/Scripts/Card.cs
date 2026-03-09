using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Cysharp.Threading.Tasks;

public class Card : MonoBehaviour
{
    [SerializeField]
    private int cardIndex;

    private readonly int maxCard = 8;

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


    private async void Start()
    {
        await UniTask.WaitUntil(() => GameManager.isReadyGameManager);
        await CardSetting();
    }

    public async UniTask CardSetting()
    {
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

}

