using Cysharp.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using UnityEngine;

public class CardVisualHelper
{
    private readonly ResourceManager resourceManager;
    private CardData cardData;
    private ActiveCard activeCard;

    private const string minion = "Minion";
    private const string spell = "Spell";
    private const string weapon = "Weapon";
    private const string hero = "Hero";

    public CardVisualHelper()
    {
        resourceManager = Locator<ResourceManager>.Get();
        activeCard = new ActiveCard();
    }

    // 최종 출력할 것들을 적어보자.
    // Active 유무, CardTransform, Sprite[] 정도?
    public async UniTask<CardVisualData> CardCategorySetting(CardData _cardData)
    {
        cardData = _cardData;

        var indexList = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8 };

        // resource 이름을 받아온다.
        var resourceNames = CardCategoryToResourceNames(indexList);

        // 여기서 cardCategory를 가지고 CardSettingTransform을 받아와야 해
        string[] cardSettingObjectName = CreateCardTransformSettingName();
        var cardSettingObject = resourceManager.Get<CardTransformSetting>(cardSettingObjectName[0]);

        // 빈 task 생성
        var taskList = new List<UniTask<Sprite>>();

        // indexList를 돌면서 resourceName value task를 받아온다.
        foreach (var value in indexList) {
            var task = resourceManager.Get<Sprite>(resourceNames[value]);
            taskList.Add(task);
        }

        // 병렬로 값을 기다린다.
        Sprite[] spriteResult = await UniTask.WhenAll(taskList);
        CardTransformSetting cardTransformSetting = await cardSettingObject;

        // Card Visual Data를 생성해서 값을 넣는다.
        CardVisualData visualData = new CardVisualData();
        visualData.activeCard = activeCard;
        visualData.cardType = NameToCardType(cardSettingObjectName[1]);
        visualData.cardTransformSetting = cardTransformSetting;
        visualData.sprites = spriteResult;
        visualData.indexList = indexList;

        return visualData;
    }

    public async UniTask<CardVisualData> CardCategorySetting(CardData _cardData, CancellationToken token)
    {
        cardData = _cardData;

        var indexList = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8 };

        // resource 이름을 받아온다.
        var resourceNames = CardCategoryToResourceNames(indexList);

        // 여기서 cardCategory를 가지고 CardSettingTransform을 받아와야 해
        string[] cardSettingObjectName = CreateCardTransformSettingName();
        var cardSettingObject = resourceManager.Get<CardTransformSetting>(cardSettingObjectName[0]);

        // 빈 task 생성
        var taskList = new List<UniTask<Sprite>>();

        // indexList를 돌면서 resourceName value task를 받아온다.
        foreach (var value in indexList)
        {
            var task = resourceManager.Get<Sprite>(resourceNames[value]);
            taskList.Add(task);
        }

        // 병렬로 값을 기다린다.
        Sprite[] spriteResult = await UniTask.WhenAll(taskList);
        CardTransformSetting cardTransformSetting = await cardSettingObject;

        // Card Visual Data를 생성해서 값을 넣는다.
        CardVisualData visualData = new CardVisualData();
        visualData.cardType = NameToCardType(cardSettingObjectName[1]);
        visualData.activeCard = activeCard;
        visualData.cardTransformSetting = cardTransformSetting;
        visualData.sprites = spriteResult;
        visualData.indexList = indexList;

        return visualData;
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

        // Active Setting은 굳이 필요없긴 해.
        CardInitActiveSetting();

        List<string> resourceNames = new List<string>(9);
        for (int i = 0; i < 9; i++)
        {
            resourceNames.Add("");
        }

        // 공통 부분은 따로 빼두자.
        if (cardData.gem == "None")
        {
            activeCard.gem = false;
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

    private string[] CreateCardTransformSettingName()
    {
        string[] objName = new string[] { "", "" };

        switch (cardData.cardCategory)
        {
            case minion:
                objName[0] = "MinionSetting";
                objName[1] = minion;
                break;
            case "Magic":
                objName[0] = "SpellSetting";
                objName[1] = spell;
                break;
            case weapon:
                objName[0] = "WeaponSetting";
                objName[1] = weapon;
                break;
            case hero:
                objName[0] = "HeroSetting";
                objName[1] = hero;
                break;
        }

        return objName;
    }

    private void CardInitActiveSetting()
    {
        activeCard.legandPortrait = false;
        activeCard.specialCardExplanation = false;
        activeCard.cardExplanation = true;
        activeCard.gem = true;
        activeCard.type = true;
        activeCard.attack = true;
        activeCard.health = true;
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
                activeCard.legandPortrait = true;
                resourceNames[4] = "Gem_Legendary";
                break;
        }

        // card Name BackGround
        resourceNames[5] = "Minion_Title";

        // card Type
        if (cardData.cardTypes[0] == "None")
        {
            activeCard.type = false;
            indexList.Remove(6);
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
                activeCard.legandPortrait = true;
                resourceNames[4] = "Spell_Gem_Legendary";
                break;
        }

        // card Name BackGround
        resourceNames[5] = "Spell_Title";

        // card Type
        if (cardData.cardTypes[0] == "None")
        {
            activeCard.type = false;
            indexList.Remove(6);
        }
        else
            resourceNames[6] = "Spell_Type";

        indexList.Remove(7);
        indexList.Remove(8);
        activeCard.attack = false;
        activeCard.health = false;
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
        activeCard.specialCardExplanation = true;

        indexList.Remove(3);
        activeCard.cardExplanation = false;

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
                activeCard.legandPortrait = true;
                resourceNames[4] = "Weapon_Gem_Legendary";
                break;
        }

        // card Name BackGround
        resourceNames[5] = "Weapon_Title";

        // card Type
        indexList.Remove(6);
        activeCard.type = false;

        resourceNames[7] = "Weapon_Attack";
        resourceNames[8] = "Weapon_Health";
    }

    private void HeroSetting(List<string> resourceNames, List<int> indexList)
    {
        // card Mask
        resourceNames[0] = "white_arch";

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
        activeCard.specialCardExplanation = true;

        indexList.Remove(3);
        activeCard.cardExplanation = false;

        // Gem
        switch (cardData.gem)
        {
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
                activeCard.legandPortrait = true;
                resourceNames[4] = "HeroCard_Gem_Legendary";
                break;
        }

        // card Name BackGround
        resourceNames[5] = "HeroCard_Title";

        // card Type
        indexList.Remove(6);
        activeCard.type = false;

        indexList.Remove(7);
        activeCard.attack = false;

        resourceNames[8] = "Shield_Health";
    }

    private CardType NameToCardType(string type)
    {
        CardType cardType = CardType.None;
        switch(type)
        {
            case minion:
                cardType = CardType.Minion;
                break;
            case spell:
                cardType = CardType.Spell;
                break;
            case weapon: 
                cardType = CardType.Weapon;
                break;
            case hero:
                cardType = CardType.Hero;
                break;
        }
        return cardType;
    }
}

public struct ActiveCard
{
    public bool legandPortrait;
    public bool specialCardExplanation;
    public bool cardExplanation;
    public bool gem;
    public bool type;
    public bool attack;
    public bool health;
}

public struct CardVisualData
{
    public ActiveCard activeCard;
    public CardType cardType;
    public CardTransformSetting cardTransformSetting;
    public Sprite[] sprites;
    public List<int> indexList;
}

public enum CardType
{
    None, Minion, Spell, Weapon, Hero,
}