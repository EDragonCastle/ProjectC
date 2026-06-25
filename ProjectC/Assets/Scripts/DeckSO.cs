using UnityEngine;
using System.Collections.Generic;

public class DeckSO : ScriptableObject
{
    [System.Serializable]
    public struct DeckEntity
    {
        public CardSO card;
        public int count;
    }

    public List<DeckEntity> cards = new List<DeckEntity>();
}
