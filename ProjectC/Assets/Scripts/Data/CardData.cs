using UnityEngine;

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