using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Card", menuName = "CardData")]
public class CardData : ScriptableObject
{
    public uint cardId;
    public string cardName;
    public int cost;
    public int attack;
    public int health;
    public string description;
    public string spriteName;
    public string gem;
    public bool isMinion;
    public string jobType;
    public string packgeType;
    public string cardType;
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
    public string job;
    public int? mana;
    public string keyword;

    public void Clear()
    {
        job = null;
        mana = null;
        keyword = "";
    }
}
