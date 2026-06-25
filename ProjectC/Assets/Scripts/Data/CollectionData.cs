using UnityEngine;
using System;
using UnityEngine.UI;
using System.Collections.Generic;

[System.Serializable]
public class CardData
{
    public uint cardId;
    public string cardName;
    public int cost;
    public int attack;
    public int health;
    public string description;
    public string spriteName;
    public string gem;
    public bool isMinion; // 삭제 예정
    public string cardCategory;
    public string jobType;
    public string packgeType;
    public string cardType; // 변경 예정
    public bool isCollector;
    public string[] cardTypes; 
    public uint[] spawn;
    public float posX;
    public float posY;
    public float rotation;
}

[CreateAssetMenu(fileName = "New Hero", menuName = "HeroName")]
public class HeroData : ScriptableObject
{
    public uint heroId;
    public string heroName;
    public string heroSprite;
    public string heroPowerName;
    public string heroPowerSprite;
    public string heroPowerExplanation;
    public string heroPowerIconSprite;
    public string heroDeckName;
}

[System.Serializable]
public class AbilityData
{
    public uint cardId;
    public string actionTrigger;
    public string action;
    public string target;
    public int value;
    public uint spawnID;
    public string condition;
    public string conditionStat;
    public int conditionValue;
    public string conditionMinionType;
    public bool isTargetting;
    public bool isTempory;
}

public class DeckData
{
    public CardData cardData;
    public int count;
}

public class DeckInformation
{
    public List<DeckData> deckData;
    public Sprite deckImage;
    public uint heroIndex;
    public string deckName;
    public int currentCard;
    public int maxCard;
}

public struct FilterInformation
{
    public string[] job;
    public int? mana;
    public string keyword;
    public void Clear()
    {
        job = null;
        mana = null;
        keyword = string.Empty;
    }
}

[System.Serializable]
public struct CardTransform
{
    public Vector2 position;
    public Vector2 ratio;
    public Vector3 scale;
}
