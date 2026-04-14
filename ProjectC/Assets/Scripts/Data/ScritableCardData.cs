using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewScritableCardData", menuName = "Data/ScritalbeCardData")]
public class ScritableCardData : ScriptableObject
{
    public List<CardData> cardDatas = new List<CardData>();
}