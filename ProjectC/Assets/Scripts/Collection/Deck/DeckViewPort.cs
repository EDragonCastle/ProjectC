using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class DeckViewPort : MonoBehaviour, IChannel
{
    public GameObject content;
    private int maxCard = 30;
    private int currentCard = 0;
    private readonly int deckCardMaxCount = 2;
    public TextMeshProUGUI cardNumber;

    private List<CardData> deckListCards = new List<CardData>();

    private void OnEnable()
    {
        cardNumber.text = $"{currentCard}/{maxCard}\n장";
        var eventManager = Locator<EventManager>.Get();
        eventManager.Subscription(ChannelInfo.InputDeck, HandleEvent);
        eventManager.Subscription(ChannelInfo.OutputDeck, HandleEvent);
    }

    private void OnDisable()
    {
        var eventManager = Locator<EventManager>.Get();
        eventManager.Unsubscription(ChannelInfo.InputDeck, HandleEvent);
        eventManager.Unsubscription(ChannelInfo.OutputDeck, HandleEvent);
        deckListCards.Clear();
    }

    public void HandleEvent(ChannelInfo channel, object information = null)
    {
        switch(channel)
        {
            case ChannelInfo.InputDeck:
                if (currentCard >= maxCard) {
                    Debug.Log("카드가 가득 차서 넣을 수 없다.");
                    return;
                }

                // information -> DeckObject 바꾸고
                GameObject cardDeck = information as GameObject;
                if(cardDeck == null) {
                    Debug.Log($"해당 Object는 GameObject로 변환할 수 없습니다.");
                    return;
                }

                InputDeckCard(cardDeck);
                break;
            case ChannelInfo.OutputDeck:
                if (currentCard < 0) {
                    Debug.Log("카드를 더 뺄 수 없다.");
                    return;
                }

                GameObject destoryObject = information as GameObject;
                if(destoryObject == null) {
                    Debug.Log($"해당 Object는 GameObject로 변환할 수 없습니다.");
                    return;
                }

                OutputDeckCard(destoryObject);
                break;
        }
    }

    private void InputDeckCard(GameObject cardDeck)
    {
        Card card = cardDeck.GetComponent<Card>();
        if (card != null) {
            if (card.cardData == null) {
                Debug.LogError($"{cardDeck.name}에는 cardData가 존재하지 않는다.");
                return;
            }
        }
        else
        {
            Debug.LogError($"{cardDeck.name}에는 Card Component가 존재하지 않는다.");
            return;
        }

        CardData newCardData = card.cardData;

        // Content 자식을 돌면서 확인해야 한다.
        foreach (Transform child in content.transform)
        {
            Card cardComponent = child.GetComponent<Card>();

            if (cardComponent == null)
                continue;

            CardData cardData = cardComponent.cardData;
            if (cardData != null && cardData.cardId == newCardData.cardId)
            {
                Debug.Log("중복된 카드가 존재한다.");
                Destroy(cardDeck);

                // 오래 걸리는 작업이다.
                var deckCardComponent = child.GetComponentInChildren<DeckCard>(true);

                if (deckCardComponent == null)
                {
                    Debug.Log($"{child.gameObject.name}의 자식에는 DeckCard Component가 없다.");
                    return;
                }

                if (deckCardComponent.deckCount >= deckCardMaxCount)
                {
                    Debug.Log("Deck에 해당 카드를 더 넣을 수 없다");
                    return;
                }

                deckCardComponent.deckCount++;
                deckCardComponent.CurrentDeckCount();
                break;
            }
        }

        cardDeck.transform.SetParent(content.transform);
        currentCard++;

        // Card Sorting Select Ver 1 O(N), Ver 2 O(log(N))
        // int targetIndex = SortIndexVer1(newCardData);
        int targetIndex = SortIndexVer2(newCardData);
        cardDeck.transform.SetSiblingIndex(targetIndex);
        //LayoutRebuilder.ForceRebuildLayoutImmediate(content.GetComponent<RectTransform>());

        cardNumber.text = $"{currentCard}/{maxCard}\n장";
    }

    private int SortIndexVer1(CardData cardData)
    {
        int index = 0;

        int newCost = cardData.cost;
        string cardName = cardData.cardName;

        foreach(Transform child in content.transform)
        {
            Card cardComponent = child.GetComponent<Card>();
            if(cardComponent != null)
            {
                CardData childCardData = cardComponent.cardData;

                if (childCardData.cost > newCost)
                    return index;
                else if(childCardData.cost == newCost)
                {
                    if (string.Compare(childCardData.cardName, cardName, System.StringComparison.Ordinal) > 0)
                        return index;
                }
            }
            index++;
        }

        return index;
    }
    
    private int SortIndexVer2(CardData cardData)
    {
        int index = deckListCards.Count;

        if(index == 0)
        {
            deckListCards.Add(cardData);
            return index;
        }

        int left = 0;
        int right = index - 1;

        while(left <= right)
        {
            int mid = (left + right) / 2;
            int compare = CompareCard(cardData, deckListCards[mid]);

            if (compare < 0)
                right = mid - 1;
            else
                left = mid + 1;
        }

        index = left;
        Debug.Log($"DeckList {index}");
        deckListCards.Insert(index, cardData);
        return index;
    }

    private int CompareCard(CardData newCard, CardData oldCard)
    {
        if (newCard.cost != oldCard.cost)
            return newCard.cost.CompareTo(oldCard.cost);

        return string.Compare(newCard.cardName, oldCard.cardName, System.StringComparison.Ordinal); 
    }


    private void OutputDeckCard(GameObject cardDeck)
    {
        // Output Deck에서 삭제할 때
        var card = cardDeck.GetComponent<Card>();
        var deckCardComponent = cardDeck.GetComponentInChildren<DeckCard>(true);

        if (card == null || deckCardComponent == null)
            return;

        if(deckCardComponent.deckCount == 1)
        {
            // DeckList에서 빼야한다.
            RemoveDeckList(card.cardData);
            Destroy(cardDeck);
        }

        deckCardComponent.deckCount--;
        deckCardComponent.CurrentDeckCount();

        currentCard--;
        cardNumber.text = $"{currentCard}/{maxCard}\n장";
    }

    private void RemoveDeckList(CardData cardData)
    {
        int left = 0;
        int right = deckListCards.Count - 1;

        while(left <= right)
        {
            int mid = (left + right) / 2;
            int compare = CompareCard(cardData, deckListCards[mid]);

            if(compare == 0)
            {
                deckListCards.RemoveAt(mid);
                return;
            }

            if (compare < 0) 
                right = mid - 1;
            else 
                left = mid + 1;
        }
    }
}
