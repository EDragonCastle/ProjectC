using UnityEngine;

public class CardSO : ScriptableObject
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
