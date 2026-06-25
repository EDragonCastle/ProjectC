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
    private int key;

    [Header("Page")]
    public GameObject page;
    public CardData cardData;

    public GameObject cardObject;
    public GameObject deckObject;

    [Header("New Card Data")]
    public Image cardMask;
    public Image cardImage;
    public Image cardBackGround;
    
    public GameObject legandPortrait;
    
    public TextMeshProUGUI specialCardExplanation;
    public Image cardExplanationObject;
    public TextMeshProUGUI cardExplanation;

    public Image gem;
    public TextMeshProUGUI cost;
    
    public Image cardNameImage;
    public TextMeshProUGUI cardName;

    public Image typeImage;
    public TextMeshProUGUI typeImageName;

    public Image attackImage;
    public TextMeshProUGUI attack;

    public Image healthImage;
    public TextMeshProUGUI health;
    
    [Header("Deck Data")]
    public Image deckImage;
    public TextMeshProUGUI deckManaCost;
    public TextMeshProUGUI deckName;

    private CollectionCardData collectionCardData;
    private CardTransformSetting cardTransformSetting;
    public CollectionCardData GetCollectionCardData() => collectionCardData;


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

        CardVisualHelper helper = new CardVisualHelper();
        var result = await helper.CardCategorySetting(cardData);

        CardVisualSetting(result);
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

        CardVisualHelper helper = new CardVisualHelper();
        var result = await helper.CardCategorySetting(cardData);

        CardVisualSetting(result);
    }

    private void CardVisualSetting(CardVisualData visualData)
    {
        InitActiveSetting(visualData.activeCard);

        // Card Position Setting
        CardPositionSetting(visualData.cardTransformSetting, visualData.activeCard.specialCardExplanation);
        
        // Sprite Setting
        for(int i = 0; i < visualData.sprites.Length; i++)
        {
            Sprite sprite = visualData.sprites[i];
            int index = visualData.indexList[i];
            SpriteCardSetting(sprite, index);
        }

        cost.text = cardData.cost.ToString();

        deckImage.sprite = cardImage.sprite;
        deckManaCost.text = cost.text;
        deckName.text = cardName.text;

        SettingCollectionCardData(visualData.activeCard, visualData.cardTransformSetting);
    }

    private void InitActiveSetting(ActiveCard activeCard)
    {
        legandPortrait.SetActive(activeCard.legandPortrait);
        specialCardExplanation.gameObject.SetActive(activeCard.specialCardExplanation);
        cardExplanationObject.gameObject.SetActive(activeCard.cardExplanation);
        gem.gameObject.SetActive(activeCard.gem);
        typeImage.gameObject.SetActive(activeCard.type);
        attackImage.gameObject.SetActive(activeCard.attack);
        healthImage.gameObject.SetActive(activeCard.health);
    }

    public async UniTask CardSetting(CardData _cardData)
    {
        cardData = _cardData;

        cardObject.SetActive(false);
        deckObject.SetActive(true);

        CardVisualHelper helper = new CardVisualHelper();
        var result = await helper.CardCategorySetting(cardData);

        CardVisualSetting(result);
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

    private void SpriteCardSetting(Sprite sprite, int index)
    {
        switch (index)
        {
            case 0:
                cardMask.sprite = sprite;
                break;
            case 1:
                cardImage.sprite = sprite;
                break;
            case 2:
                cardBackGround.sprite = sprite;
                break;
            case 3:
                cardExplanationObject.sprite = sprite;
                break;
            case 4:
                gem.sprite = sprite;
                break;
            case 5:
                cardNameImage.sprite = sprite;
                break;
            case 6:
                typeImage.sprite = sprite;
                break;
            case 7:
                attackImage.sprite = sprite;
                break;
            case 8:
                healthImage.sprite = sprite;
                break;
        }
    }

    private void SettingCollectionCardData(ActiveCard activeCard, CardTransformSetting _cardTransformSetting)
    {
        if (collectionCardData == null)
            collectionCardData = new CollectionCardData();

        var uiManager = Locator<UIManager>.Get();
        deckCardScript.canvasParent = uiManager.GetCollectionCanvas().GetComponent<RectTransform>();

        collectionCardData.maskImage = cardMask;
        collectionCardData.cardImage = cardImage;
        collectionCardData.cardBackGround = cardBackGround;
        collectionCardData.isSpecial = activeCard.specialCardExplanation;

        if (collectionCardData.isSpecial)
        {
            collectionCardData.cardExplanation = specialCardExplanation;
        }
        else
        {
            collectionCardData.cardExplanationImage = cardExplanationObject;
            collectionCardData.cardExplanation = cardExplanation;
        }

        collectionCardData.legandPortrait = legandPortrait;
        collectionCardData.isActiveGem = activeCard.gem;
        collectionCardData.gem = gem;

        collectionCardData.cost = cost;
        collectionCardData.cardNameImage = cardNameImage;
        collectionCardData.cardName = cardName;

        collectionCardData.isActiveType = activeCard.type;
        collectionCardData.typeImage = typeImage;
        collectionCardData.cardTypeText = typeImageName;

        collectionCardData.spawnID = cardData.spawn;

        collectionCardData.isAttack = activeCard.attack;
        collectionCardData.attackImage = attackImage;
        collectionCardData.attack = attack;

        collectionCardData.isHealth = activeCard.health;
        collectionCardData.healthImage = healthImage;
        collectionCardData.health = health;

        collectionCardData.cardTransformSetting = _cardTransformSetting;
    }



    private void CardPositionSetting(CardTransformSetting setting, bool isSpecial)
    {
        if (!setting.isVoidValue(setting.mask))
        {
            var maskRect = cardMask.GetComponent<RectTransform>();
            maskRect.anchoredPosition = setting.mask.position;
            maskRect.sizeDelta = setting.mask.ratio;
            maskRect.localScale = setting.mask.scale;
        }

        if(!setting.isVoidValue(setting.cardMainImage))
        {
            var mainImageRect = cardImage.GetComponent<RectTransform>();
            mainImageRect.anchoredPosition = setting.cardMainImage.position;
            mainImageRect.sizeDelta = setting.cardMainImage.ratio;
            mainImageRect.localScale = setting.cardMainImage.scale;
        }

        if (!setting.isVoidValue(setting.legandPortrait))
        {
            var legandRect = legandPortrait.GetComponent<RectTransform>();
            legandRect.anchoredPosition = setting.legandPortrait.position;
            legandRect.sizeDelta = setting.legandPortrait.ratio;
            legandRect.localScale = setting.legandPortrait.scale;
        }

        if (!setting.isVoidValue(setting.cardExplanation))
        {
            if (isSpecial)
            {
                var explanationRect = specialCardExplanation.GetComponent<RectTransform>();
                explanationRect.anchoredPosition = setting.cardExplanation.position;
                explanationRect.sizeDelta = setting.cardExplanation.ratio;
                explanationRect.localScale = setting.cardExplanation.scale;
                specialCardExplanation.text = cardData.description;
            }
            else
            {
                var explanationRect = cardExplanationObject.GetComponent<RectTransform>();
                explanationRect.anchoredPosition = setting.cardExplanation.position;
                explanationRect.sizeDelta = setting.cardExplanation.ratio;
                explanationRect.localScale = setting.cardExplanation.scale;
                cardExplanation.text = cardData.description;
            }
        }

        if (!setting.isVoidValue(setting.gem))
        {
            var gemRect = gem.GetComponent<RectTransform>();
            gemRect.anchoredPosition = setting.gem.position;
            gemRect.sizeDelta = setting.gem.ratio;
            gemRect.localScale = setting.gem.scale;
        }

        if (!setting.isVoidValue(setting.cardName))
        {
            var cardNameRect = cardNameImage.GetComponent<RectTransform>();
            cardNameRect.anchoredPosition = setting.cardName.position;
            cardNameRect.sizeDelta = setting.cardName.ratio;
            cardNameRect.localScale = setting.cardName.scale;
            cardName.text = cardData.cardName;
        }

        if (!setting.isVoidValue(setting.cardNameText))
        {
            var cardNameTextRect = cardName.GetComponent<RectTransform>();
            cardNameTextRect.anchoredPosition = setting.cardNameText.position;
            cardNameTextRect.sizeDelta = setting.cardNameText.ratio;
            cardNameTextRect.localScale = setting.cardNameText.scale;
        }

        if (!setting.isVoidValue(setting.cardType))
        {
            var cardTypeRect = typeImage.GetComponent<RectTransform>();
            cardTypeRect.anchoredPosition = setting.cardType.position;
            cardTypeRect.sizeDelta = setting.cardType.ratio;
            cardTypeRect.localScale = setting.cardType.scale;

            var cardTypeTextRect = typeImageName.GetComponent<RectTransform>();
            cardTypeTextRect.anchoredPosition = setting.cardTypeText.position;
            cardTypeTextRect.sizeDelta = setting.cardTypeText.ratio;
            cardTypeTextRect.localScale = setting.cardTypeText.scale;

            string typeName = cardData.cardTypes[0];
            for (int i = 1; i < cardData.cardTypes.Length; i++)
            {
                typeName += $"\n{cardData.cardTypes[i]}";
            }
            typeImageName.text = typeName;
        }

        if (!setting.isVoidValue(setting.attack))
        {
            var attackRect = attackImage.GetComponent<RectTransform>();
            attackRect.anchoredPosition = setting.attack.position;
            attackRect.sizeDelta = setting.attack.ratio;
            attackRect.localScale = setting.attack.scale;
            attack.text = cardData.attack.ToString();
        }

        if (!setting.isVoidValue(setting.attackText))
        {
            var attackRect = attack.GetComponent<RectTransform>();
            attackRect.anchoredPosition = setting.attackText.position;
            attackRect.sizeDelta = setting.attackText.ratio;
            attackRect.localScale = setting.attackText.scale;
        }

        if (!setting.isVoidValue(setting.health))
        {
            var healthRect = healthImage.GetComponent<RectTransform>();
            healthRect.anchoredPosition = setting.health.position;
            healthRect.sizeDelta = setting.health.ratio;
            healthRect.localScale = setting.health.scale;
            health.text = cardData.health.ToString();
        }

        if (!setting.isVoidValue(setting.healthText))
        {
            var healthRect = health.GetComponent<RectTransform>();
            healthRect.anchoredPosition = setting.healthText.position;
            healthRect.sizeDelta = setting.healthText.ratio;
            healthRect.localScale = setting.healthText.scale;
        }
    }
}

public class CollectionCardData
{
    public CardTransformSetting cardTransformSetting;
    public Image maskImage;
    public Image cardImage;
    public Image cardBackGround;

    public bool isSpecial;
    public Image cardExplanationImage;
    public TextMeshProUGUI cardExplanation;
    
    public GameObject legandPortrait;

    public bool isActiveGem;
    public Image gem;

    public TextMeshProUGUI cost;

    public Image cardNameImage;
    public TextMeshProUGUI cardName;

    public bool isActiveType;
    public Image typeImage;
    public TextMeshProUGUI cardTypeText;

    public uint[] spawnID;

    public bool isAttack;
    public Image attackImage;
    public TextMeshProUGUI attack;

    public bool isHealth;
    public Image healthImage;
    public TextMeshProUGUI health;
}

// 지금 여기서 바꾼거 
// CollectionCardData에 어떤 Data를 넣어야 할지 다시 고민해야 한다.

/*
    
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

        await CardCategorySetting();

        SettingCollectionCardData();
    }
 
    private void SettingCollectionCardData()
    {
        if (collectionCardData == null)
            collectionCardData = new CollectionCardData();

        var uiManager = Locator<UIManager>.Get();
        deckCardScript.canvasParent = uiManager.GetCollectionCanvas().GetComponent<RectTransform>();

        collectionCardData.maskImage = cardMask;
        collectionCardData.cardImage = cardImage;
        collectionCardData.cardBackGround = cardBackGround;
        collectionCardData.isSpecial = IsSpecialText();
        if(collectionCardData.isSpecial) {
            collectionCardData.cardExplanation = specialCardExplanation;
        }
        else {
            collectionCardData.cardExplanationImage = cardExplanationObject;
            collectionCardData.cardExplanation = cardExplanation;
        }

        collectionCardData.legandPortrait = legandPortrait;
        collectionCardData.isActiveGem = gem.gameObject.activeSelf;
        collectionCardData.gem = gem;

        collectionCardData.cost = cost;
        collectionCardData.cardNameImage = cardNameImage;
        collectionCardData.cardName = cardName;

        collectionCardData.isActiveType = typeImage.gameObject.activeSelf;
        collectionCardData.typeImage = typeImage;
        collectionCardData.cardTypeText = typeImageName;

        collectionCardData.spawnID = cardData.spawn;

        collectionCardData.isAttack = attackImage.gameObject.activeSelf;
        collectionCardData.attackImage = attackImage;
        collectionCardData.attack = attack;

        collectionCardData.isHealth = healthImage.gameObject.activeSelf;
        collectionCardData.healthImage = healthImage;
        collectionCardData.health = health;

        collectionCardData.cardTransformSetting = cardTransformSetting;
    }
    
    // Card Setting은 이미 있어서 CardData에 담긴 내용을 가지고 하는 것인데 이름을 무엇으로 지어야 할까?
    private async UniTask CardCategorySetting()
    {
        var resourceManager = Locator<ResourceManager>.Get();

        var indexList = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8 };

        // resource 이름을 받아온다.
        var resourceNames = CardCategoryToResourceNames(indexList);

        // 여기서 cardCategory를 가지고 CardSettingTransform을 받아와야 해
        string cardSettingObjectName = CreateCardTransformSettingName();
        var cardSettingObject = resourceManager.Get<CardTransformSetting>(cardSettingObjectName);

        // 빈 task 생성
        var taskList = new List<UniTask<Sprite>>();

        // indexList를 돌면서 resourceName value task를 받아온다.
        foreach(var value in indexList) {
            var task = resourceManager.Get<Sprite>(resourceNames[value]);
            taskList.Add(task);
        }

        // 다 될때까지 기다린다.
        Sprite[] spriteResult = await UniTask.WhenAll(taskList);

        cardTransformSetting = await cardSettingObject;


        bool isSepcial = IsSpecialText();
        CardPositionSetting(cardTransformSetting, isSepcial);

        // indexList와 spriteResult를 돌면서 sprite와 index를 일치시켜 실제 Card에 넣는다.
        // 여기서 Sprite Setting을 해준다.
        for (int i = 0; i < spriteResult.Length; i++)
        {
            Sprite sprite = spriteResult[i];
            int index = indexList[i];
            SpriteCardSetting(sprite, index);
        }
        cost.text = cardData.cost.ToString();

        deckImage.sprite = cardImage.sprite;
        deckManaCost.text = cost.text;
        deckName.text = cardName.text;
    }

    private string CreateCardTransformSettingName()
    {
        string objName = "";

        switch (cardData.cardCategory)
        {
            case "Minion":
                objName = "MinionSetting";
                break;
            case "Magic":
                objName = "SpellSetting";
                break;
            case "Weapon":
                objName = "WeaponSetting";
                break;
            case "Hero":
                objName = "HeroSetting";
                break;
        }

        return objName;
    }

    private bool IsSpecialText()
    {
        bool isSpeical = false;
        switch(cardData.cardCategory) {
            case "Minion":
            case "Magic":
                isSpeical = false;
                break;
            case "Weapon":
            case "Hero":
                isSpeical = true;
                break;
        }
        return isSpeical;
    }

   

    private List<string> CardCategoryToResourceNames(List<int> indexList)
    {
        // 0 CardMask
        // 1 CardImage
        // 2 CardBackGround
        // 3 Card Text BackGround
        // 4 gem
        // 5 CardName
        // 6 CardType
        // 7 Attack
        // 8 Health

        CardInitActiveSetting();

        List<string> resourceNames = new List<string>(9);
        for(int i = 0; i < 9; i++) {
            resourceNames.Add("");
        }
        
        // 공통 부분은 따로 빼두자.
        if (cardData.gem == "None") {
            gem.gameObject.SetActive(false);
            indexList.Remove(4);
        }

        // card Image
        resourceNames[1] = cardData.spriteName;
        
        // 아마 Minion Setting 별 위치 Setting을 해야 하는데
        switch (cardData.cardCategory)
        {
            case "Minion":
                MinionSetting(resourceNames, indexList);
                break;
            case "Magic":
                SpellSetting(resourceNames, indexList);
                break;
            case "Weapon":
                WeaponSetting(resourceNames, indexList);
                break;
            case "Hero":
                HeroSetting(resourceNames, indexList);
                break;
        }

        return resourceNames;
    }
    
    private void CardInitActiveSetting()
    {
        legandPortrait.SetActive(false);
        specialCardExplanation.gameObject.SetActive(false);
        cardExplanationObject.gameObject.SetActive(true);
        gem.gameObject.SetActive(true);
        typeImage.gameObject.SetActive(true);
        attackImage.gameObject.SetActive(true);
        healthImage.gameObject.SetActive(true);
    }

    private void MinionSetting(List<string> resourceNames, List<int> indexList)
    {
        // card Mask
        resourceNames[0] = "circle";

        // card BackGround
        switch (cardData.jobType)
        {
            case "드루이드":
                resourceNames[2] = "Druid";
                break;
            case "사냥꾼":
                resourceNames[2] = "Hunter";
                break;
            case "마법사":
                resourceNames[2] = "Mage";
                break;
            case "성기사":
                resourceNames[2] = "Paladin";
                break;
            case "도적":
                resourceNames[2] = "Rogue";
                break;
            case "주술사":
                resourceNames[2] = "Shaman";
                break;
            case "흑마법사":
                resourceNames[2] = "Warlock";
                break;
            case "사제":
                resourceNames[2] = "Priest";
                break;
            case "전사":
                resourceNames[2] = "Warrior";
                break;
            case "중립":
                resourceNames[2] = "Neutral";
                break;
        }

        // Card Text BackGround
        resourceNames[3] = "Minion_Text";

        // Gem
        switch (cardData.gem)
        {
            case "Gem_Common":
                resourceNames[4] = "Gem_Common";
                break;
            case "Gem_Rare":
                resourceNames[4] = "Gem_Rare";
                break;
            case "Gem_Epic":
                resourceNames[4] = "Gem_Epic";
                break;
            case "Gem_Legendary":
                legandPortrait.SetActive(true);
                resourceNames[4] = "Gem_Legendary";
                break;
        }

        // card Name BackGround
        resourceNames[5] = "Minion_Title";

        // card Type
        if (cardData.cardTypes[0] == "None")
        {
            indexList.Remove(6);
            typeImage.gameObject.SetActive(false);
        }
        else
            resourceNames[6] = "Minion_Type";

        resourceNames[7] = "Attack";
        resourceNames[8] = "Health";
    }

    private void SpellSetting(List<string> resourceNames, List<int> indexList)
    {
        // card Mask
        resourceNames[0] = "SpellBackGround";

        // card BackGround
        switch (cardData.jobType)
        {
            case "드루이드":
                resourceNames[2] = "Spell_Druid";
                break;
            case "사냥꾼":
                resourceNames[2] = "Spell_Hunter";
                break;
            case "마법사":
                resourceNames[2] = "Spell_Mage";
                break;
            case "성기사":
                resourceNames[2] = "Spell_Paladin";
                break;
            case "도적":
                resourceNames[2] = "Spell_Rogue";
                break;
            case "주술사":
                resourceNames[2] = "Spell_Shaman";
                break;
            case "흑마법사":
                resourceNames[2] = "Spell_Warlock";
                break;
            case "사제":
                resourceNames[2] = "Spell_Priest";
                break;
            case "전사":
                resourceNames[2] = "Spell_Warrior";
                break;
            case "중립":
                resourceNames[2] = "Spell_Neutral";
                break;
        }

        // Card Text BackGround
        resourceNames[3] = "Spell_Text";

        // Gem
        switch (cardData.gem)
        {
            case "Gem_Common":
                resourceNames[4] = "Spell_Gem_Common";
                break;
            case "Gem_Rare":
                resourceNames[4] = "Spell_Gem_Rare";
                break;
            case "Gem_Epic":
                resourceNames[4] = "Spell_Gem_Epic";
                break;
            case "Gem_Legendary":
                legandPortrait.SetActive(true);
                resourceNames[4] = "Spell_Gem_Legendary";
                break;
        }

        // card Name BackGround
        resourceNames[5] = "Spell_Title";

        // card Type
        if (cardData.cardTypes[0] == "None")
        {
            indexList.Remove(6);
            typeImage.gameObject.SetActive(false);
        }
        else
            resourceNames[6] = "Spell_Type";

        indexList.Remove(7);
        indexList.Remove(8);
        attackImage.gameObject.SetActive(false);
        healthImage.gameObject.SetActive(false);
    }

    private void WeaponSetting(List<string> resourceNames, List<int> indexList)
    {
        // card Mask
        resourceNames[0] = "circle";

        // card BackGround
        switch (cardData.jobType)
        {
            case "드루이드":
                resourceNames[2] = "Weapon_Druid";
                break;
            case "사냥꾼":
                resourceNames[2] = "Weapon_Hunter";
                break;
            case "마법사":
                resourceNames[2] = "Weapon_Mage";
                break;
            case "성기사":
                resourceNames[2] = "Weapon_Paladin";
                break;
            case "도적":
                resourceNames[2] = "Weapon_Rogue";
                break;
            case "주술사":
                resourceNames[2] = "Weapon_Shaman";
                break;
            case "흑마법사":
                resourceNames[2] = "Weapon_Warlock";
                break;
            case "사제":
                resourceNames[2] = "Weapon_Priest";
                break;
            case "전사":
                resourceNames[2] = "Weapon_Warrior";
                break;
            case "중립":
                resourceNames[2] = "Weapon_Neutral";
                break;
        }

        // Card Text BackGround
        specialCardExplanation.gameObject.SetActive(true);
        
        indexList.Remove(3);
        cardExplanationObject.gameObject.SetActive(false);

        // Gem
        switch (cardData.gem)
        {
            case "Gem_Common":
                resourceNames[4] = "Weapon_Gem_Common";
                break;
            case "Gem_Rare":
                resourceNames[4] = "Weapon_Gem_Rare";
                break;
            case "Gem_Epic":
                resourceNames[4] = "Weapon_Gem_Epic";
                break;
            case "Gem_Legendary":
                legandPortrait.SetActive(true);
                resourceNames[4] = "Weapon_Gem_Legendary";
                break;
        }

        // card Name BackGround
        resourceNames[5] = "Weapon_Title";

        // card Type
        indexList.Remove(6);
        typeImage.gameObject.SetActive(false);

        resourceNames[7] = "Weapon_Attack";
        resourceNames[8] = "Weapon_Health";
    }

    private void HeroSetting(List<string> resourceNames, List<int> indexList)
    {
        // card Mask
        resourceNames[0] = "white_arch";

        // card BackGround
        switch (cardData.jobType) {
            case "드루이드":
                resourceNames[2] = "Weapon_Druid";
                break;
            case "사냥꾼":
                resourceNames[2] = "Weapon_Hunter";
                break;
            case "마법사":
                resourceNames[2] = "Weapon_Mage";
                break;
            case "성기사":
                resourceNames[2] = "Weapon_Paladin";
                break;
            case "도적":
                resourceNames[2] = "Weapon_Rogue";
                break;
            case "주술사":
                resourceNames[2] = "Weapon_Shaman";
                break;
            case "흑마법사":
                resourceNames[2] = "Weapon_Warlock";
                break;
            case "사제":
                resourceNames[2] = "Weapon_Priest";
                break;
            case "전사":
                resourceNames[2] = "Weapon_Warrior";
                break;
            case "중립":
                resourceNames[2] = "Weapon_Neutral";
                break;
        }

        // Card Text BackGround
        specialCardExplanation.gameObject.SetActive(true);

        indexList.Remove(3);
        cardExplanationObject.gameObject.SetActive(false);

        // Gem
        switch (cardData.gem) {
            case "Gem_Common":
                resourceNames[4] = "HeroCard_Gem_Common";
                break;
            case "Gem_Rare":
                resourceNames[4] = "HeroCard_Gem_Rare";
                break;
            case "Gem_Epic":
                resourceNames[4] = "HeroCard_Gem_Epic";
                break;
            case "Gem_Legendary":
                legandPortrait.SetActive(true);
                resourceNames[4] = "HeroCard_Gem_Legendary";
                break;
        }

        // card Name BackGround
        resourceNames[5] = "HeroCard_Title";

        // card Type
        indexList.Remove(6);
        typeImage.gameObject.SetActive(false);

        indexList.Remove(7);
        attackImage.gameObject.SetActive(false);

        resourceNames[8] = "Shield_Health";
    }


 
 
 
 */