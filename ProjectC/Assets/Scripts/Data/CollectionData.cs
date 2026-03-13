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
    public string deckName;
    public int currentCard;
    public int maxCard;
}