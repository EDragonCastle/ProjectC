using UnityEngine;
using System.Collections.Generic;
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

    public GameObject minion;
    public GameObject magic;
   
    // Minion CardData
    [Header("Minion Card Data")]
    public Image cardImage;
    public Image cardBackGround;
    public TextMeshProUGUI cardExplanation;
    public Image gem;
    public GameObject legandPortrait;
    public TextMeshProUGUI cost;
    public TextMeshProUGUI cardName;
    public GameObject type;
    public TextMeshProUGUI typeName;

    public TextMeshProUGUI attack;
    public TextMeshProUGUI health;
    

    [Header("Magic Card Data")]
    public Image magicCardImage;
    public Image magicCardBackGround;
    public TextMeshProUGUI magicCardExplanation;
    public Image magicGem;
    public GameObject magicLegandPortrait;
    public TextMeshProUGUI magicCost;
    public TextMeshProUGUI magicCardName;
    public GameObject magicType;
    public TextMeshProUGUI magicTypeName;

    [Header("Deck Data")]
    public Image deckImage;
    public TextMeshProUGUI deckManaCost;
    public TextMeshProUGUI deckName;

    private CollectionCardData collectionCardData;
    public CollectionCardData GetCollectionCardData() => collectionCardData;

    // Weapon card나 영웅 카드 기타 카드들이 추가될떄마다 위와 같이 Card들을 세팅할 수는 없다.
    // 왜냐하면 weapon card나 hero card들이 추가될때마다 그에 맞게 설정해줘야 하기 때문이다.
    // 추가로 넣고 코드도 길어지고 유지보수가 힘들어진다. -> 수정 필요.


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
        collectionCardData = new CollectionCardData();
        await CardSetting();
    }

    public async UniTask CardSetting()
    {
        if (page == null)
            return;

        var pageInfoComponent = page.GetComponent<PageInformation>();
        int currentPage = pageInfoComponent.page;

        DataManager dataManager = Locator<DataManager>.Get();
        var pageData = dataManager.GetPageData(currentPage);

        // outofIndex를 대비해야 한다.
        if (pageData == null || cardIndex >= pageData.Count)
        {
            cardObject.SetActive(false);
            deckObject.SetActive(false);
            return;
        }

        CardData _cardData = pageData[cardIndex];
        cardData = _cardData;

        if (cardData.isMinion)
            await MinionCardSetting(cardData);
        else
            await MagicCardSetting(cardData);

        SettingCollectionCardData();
    }

    public async UniTask CardSetting(int pageIndex)
    {
        DataManager dataManager = Locator<DataManager>.Get();
        var pageData = dataManager.GetPageData(pageIndex);

        // outofIndex를 대비해야 한다.
        if (pageData == null || cardIndex >= pageData.Count)
        {
            cardObject.SetActive(false);
            deckObject.SetActive(false);
            return;
        }

        cardObject.SetActive(true);
        deckObject.SetActive(false);

        CardData _cardData = pageData[cardIndex];
        cardData = _cardData;

        if (cardData.isMinion)
            await MinionCardSetting(cardData);
        else
            await MagicCardSetting(cardData);

        SettingCollectionCardData();
    }

    public async UniTask CardSetting(CardData _cardData)
    {
        cardData = _cardData;

        cardObject.SetActive(false);
        deckObject.SetActive(true);

        var uiManager = Locator<UIManager>.Get();
        deckCardScript.canvasParent = uiManager.GetCollectionCanvas().GetComponent<RectTransform>();

        if (cardData.isMinion)
            await MinionCardSetting(cardData);
        else
            await MagicCardSetting(cardData);

        SettingCollectionCardData();
    }

    public void ReleaseCard(int page)
    {
        DataManager dataManager = Locator<DataManager>.Get();
        ResourceManager resourceManager = Locator<ResourceManager>.Get();
        var pageData = dataManager.GetPageData(page);

        if (pageData == null || cardIndex >= pageData.Count) {
            return;
        }

        CardData _cardData = pageData[cardIndex];
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

    private void SettingCollectionCardData()
    {
        if (collectionCardData == null)
            collectionCardData = new CollectionCardData();

        collectionCardData.spawnID = cardData.spawn;

        if (cardData.isMinion)
        {
            collectionCardData.isMinion = cardData.isMinion;
            collectionCardData.cardImage = cardImage;
            collectionCardData.cardBackGround = cardBackGround;
            collectionCardData.cardExplanation = cardExplanation;

            collectionCardData.isActiveGem = gem.gameObject.activeSelf;
            collectionCardData.gem = gem;
            collectionCardData.legandPortrait = legandPortrait;

            collectionCardData.cost = cost;
            collectionCardData.cardName = cardName;

            collectionCardData.isActiveType = type.activeSelf;
            collectionCardData.cardTypeText = typeName;
            collectionCardData.spawnID = cardData.spawn;

            collectionCardData.attack = attack;
            collectionCardData.health = health;
        }
        else
        {
            collectionCardData.isMinion = cardData.isMinion;
            collectionCardData.cardImage = magicCardImage;
            collectionCardData.cardBackGround = magicCardBackGround;
            collectionCardData.cardExplanation = magicCardExplanation;

            collectionCardData.isActiveGem = magicGem.gameObject.activeSelf;
            collectionCardData.gem = magicGem;
            collectionCardData.legandPortrait = magicLegandPortrait;
            collectionCardData.cost = magicCost;
            
            collectionCardData.cardName = magicCardName;
            collectionCardData.isActiveType = magicType.activeSelf;
            collectionCardData.cardTypeText = magicTypeName;
            collectionCardData.spawnID = cardData.spawn;
        }
    }

    private string GemToResourceName(bool isMinion, string gemType)
    {
        string gemName = null;
        if(isMinion) {
            gem.gameObject.SetActive(true);
            legandPortrait.SetActive(false);
        }
        else {
            magicGem.gameObject.SetActive(true);
            magicLegandPortrait.SetActive(false);
        }

        switch (gemType)
        {
            case "None":
                gemName = null;
                if (isMinion)
                    gem.gameObject.SetActive(false);
                else
                    magicGem.gameObject.SetActive(false);
                break;
            case "Gem_Common":
                gemName = isMinion ? "Gem_Common" : "Spell_Gem_Common";
                break;
            case "Gem_Rare":
                gemName = isMinion ? "Gem_Rare" : "Spell_Gem_Rare";
                break;
            case "Gem_Epic":
                gemName = isMinion ? "Gem_Epic" : "Spell_Gem_Epic";
                break;
            case "Gem_Legendary":
                gemName = isMinion ? "Gem_Legendary" : "Spell_Gem_Legendary";
                if (isMinion)
                    legandPortrait.SetActive(true);
                else
                    magicLegandPortrait.SetActive(true);
                break;
        }

        return gemName;
    }


    // 바꿔야 한다.
    private string JobTypeToResourceName(bool isMinion, string jobType)
    {
        string result = null;

        switch (jobType)
        {
            case "중립":
                result = isMinion ? "Neutral" : "Neutral";
                break;
            case "사냥꾼":
                result = isMinion ? "Hunter" : "Magic_Hunter";
                break;
            case "마법사":
                result = isMinion ? "Mage" : "Magic_Mage";
                break;
            case "드루이드":
                result = isMinion ? "Druid" : "Magic_Druid";
                break;
            case "성기사":
                result = isMinion ? "Paladin" : "Magic_Paladin";
                break;
            case "도적":
                result = isMinion ? "Rogue" : "Magic_Paladin";
                break;
            case "주술사":
                result = isMinion ? "Shaman" : "Magic_Paladin";
                break;
            case "흑마법사":
                result = isMinion ? "Warrior" : "Magic_Paladin";
                break;
            case "사제":
                result = isMinion ? "Priest" : "Magic_Paladin";
                break;
        }

        return result;
    }

    private void MinionCardType(string cardType)
    {
        type.SetActive(true);

        if(cardType == "None")
        {
            type.SetActive(false);
        }
    }

    private void MagicCardType(string cardType)
    {
        magicType.SetActive(true);

        if (cardType == "None")
        {
            magicType.SetActive(false);
        }

    }

    private async UniTask MinionCardSetting(CardData cardData)
    {
        var resourceManager = Locator<ResourceManager>.Get();
        string gemResourceName = GemToResourceName(cardData.isMinion, cardData.gem);

        string jobCardSprite = JobTypeToResourceName(cardData.isMinion, cardData.jobType);
        MinionCardType(cardData.cardType);

        var gemTask = !string.IsNullOrWhiteSpace(gemResourceName) ? resourceManager.Get<Sprite>(gemResourceName) : UniTask.FromResult<Sprite>(null);
        var cardSpriteTask = resourceManager.Get<Sprite>(cardData.spriteName);
        var jobCardSpriteTask = resourceManager.Get<Sprite>(jobCardSprite);

        // card setting
        cardName.text = cardData.cardName;
        cardExplanation.text = cardData.description;
        typeName.text = cardData.cardType;
        cost.text = cardData.cost.ToString();
        attack.text = cardData.attack.ToString();
        health.text = cardData.health.ToString();

        // deck setting
        deckName.text = cardName.text;
        deckManaCost.text = cost.text;

        // Resource Load 중
        var spriteResult = await UniTask.WhenAll(gemTask, jobCardSpriteTask, cardSpriteTask);

        if(spriteResult.Item1 != null)
            gem.sprite = spriteResult.Item1;

        cardImage.sprite = spriteResult.Item3;
        cardBackGround.sprite = spriteResult.Item2;
        deckImage.sprite = spriteResult.Item2;

        minion.SetActive(true);
        magic.SetActive(false);

        // setting을 해야해
    }

    private async UniTask MagicCardSetting(CardData cardData)
    {
        var resourceManager = Locator<ResourceManager>.Get();
        string gemResourceName = GemToResourceName(cardData.isMinion, cardData.gem);

        string jobCardSprite = JobTypeToResourceName(cardData.isMinion, cardData.jobType);
        MagicCardType(cardData.cardType);

        var gemTask = !string.IsNullOrWhiteSpace(gemResourceName) ? resourceManager.Get<Sprite>(gemResourceName) : UniTask.FromResult<Sprite>(null);
        var cardSpriteTask = resourceManager.Get<Sprite>(cardData.spriteName);
        var jobCardSpriteTask = resourceManager.Get<Sprite>(jobCardSprite);

        // card setting
        magicCardName.text = cardData.cardName;
        magicCardExplanation.text = cardData.description;
        magicCost.text = cardData.cost.ToString();
        magicTypeName.text = cardData.cardType;

        // deck setting
        deckName.text = magicCardName.text;
        deckManaCost.text = magicCost.text;

        // Resource Load 중
        var spriteResult = await UniTask.WhenAll(gemTask, jobCardSpriteTask, cardSpriteTask);

        if (spriteResult.Item1 != null)
            magicGem.sprite = spriteResult.Item1;

        magicCardImage.sprite = spriteResult.Item3;
        magicCardBackGround.sprite = spriteResult.Item2;
        deckImage.sprite = spriteResult.Item3;

        magic.SetActive(true);
        minion.SetActive(false);
    }
}

public class CollectionCardData
{
    public bool isMinion;
    public Image cardImage;
    public Image cardBackGround;
    public TextMeshProUGUI cardExplanation;

    public bool isActiveGem;
    public Image gem;
    public GameObject legandPortrait;
    public TextMeshProUGUI cost;

    public TextMeshProUGUI cardName;

    public bool isActiveType;
    public TextMeshProUGUI cardTypeText;

    public uint[] spawnID;

    public TextMeshProUGUI attack;
    public TextMeshProUGUI health;
}

/*
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

    GemToLegandPortrait(cardData.gem);
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
    SetCollectionCardData();

}
 */

/// Only once Card Version 1
/*
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

     GemToLegandPortrait(cardData.gem);
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
     SetCollectionCardData();
 }

 public async UniTask CardSetting(CardData _cardData)
 {
     ResourceManager resourceManager = Locator<ResourceManager>.Get();

     var cardSpriteTask = resourceManager.Get<Sprite>(_cardData.spriteName);
     var gemSpriteTask = resourceManager.Get<Sprite>(_cardData.gem);

     cardData = _cardData;
     GemToLegandPortrait(cardData.gem);
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
     SetCollectionCardData();
 }
  */




/// Magic Minion Card Setting Ver 2
/*
    public async UniTask CardSetting()
    {
        if (page == null)
            return;

        var pageInfoComponent = page.GetComponent<PageInformation>();
        int currentPage = pageInfoComponent.page;

        DataManager dataManager = Locator<DataManager>.Get();
        var sortCardList = dataManager.GetSortCardData();

        // outofIndex를 대비해야 한다.
        if (sortCardList.Count <= currentPage * maxCard + cardIndex || currentPage * maxCard + cardIndex < 0)
        {
            cardObject.SetActive(false);
            deckObject.SetActive(false);
            return;
        }

        CardData _cardData = sortCardList[currentPage * maxCard + cardIndex];
        cardData = _cardData;

        if (cardData.isMinion)
            await MinionCardSetting(cardData);
        else
            await MagicCardSetting(cardData);

        SettingCollectionCardData();
    }

    
    public async UniTask CardSetting(int index)
    {
        DataManager dataManager = Locator<DataManager>.Get();
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

        if (cardData.isMinion)
            await MinionCardSetting(cardData);
        else
            await MagicCardSetting(cardData);

        SettingCollectionCardData();
    }

 */