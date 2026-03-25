using UnityEngine;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;
using TMPro;
using System;

public class DeckViewPort : MonoBehaviour, IChannel
{
    public GameObject content;
    private int maxCard = 30;
    private int currentCard = 0;
    private readonly int deckCardMaxCount = 2;
    public TextMeshProUGUI cardNumber;

    // 여기가 실제 Deck List인데 Card 개수는 모르지만, 어떤 Card들이 들어있는지는 알 수 있어.
    // 카드 Count만 알면 되겠네?
    private List<CardData> deckListCards = new List<CardData>();
    private List<DeckData> deckList = new List<DeckData>();

    public List<DeckData> GetDeckData() => deckList;
    public int GetCurrentCard() => currentCard;
    public int GetMaxCard() => maxCard;


    private void OnEnable()
    {
        cardNumber.text = $"{currentCard}/{maxCard}\n장";
        var eventManager = Locator<EventManager>.Get();
        deckList = new List<DeckData>();
        eventManager.Subscription(ChannelInfo.InputDeck, HandleEvent);
        eventManager.Subscription(ChannelInfo.OutputDeck, HandleEvent);
    }

    private void OnDisable()
    {
        var eventManager = Locator<EventManager>.Get();
        eventManager.Unsubscription(ChannelInfo.InputDeck, HandleEvent);
        eventManager.Unsubscription(ChannelInfo.OutputDeck, HandleEvent);
        deckList = null;
        ClearAllDeckData();
        cardNumber.text = $"{currentCard}/{maxCard}\n장";
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

                //InputDeckCard(cardDeck);
                InputDeckCard2(cardDeck);
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

                //OutputDeckCard(destoryObject);
                OutputDeckCard2(destoryObject);
                break;
        }
    }

    private void ClearAllDeckData()
    {
        for(int i = content.transform.childCount - 1; i >= 0; i--)
        {
            GameObject child = content.transform.GetChild(i).gameObject;
            Destroy(child);
        }
        currentCard = 0;
        maxCard = 30;
    }

    #region Dummy

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

        foreach (Transform child in content.transform)
        {
            Card cardComponent = child.GetComponent<Card>();
            if (cardComponent != null)
            {
                CardData childCardData = cardComponent.cardData;

                if (childCardData.cost > newCost)
                    return index;
                else if (childCardData.cost == newCost)
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

        if (index == 0)
        {
            deckListCards.Add(cardData);
            return index;
        }

        int left = 0;
        int right = index - 1;

        while (left <= right)
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

    private int SortIndexVer3(CardData cardData)
    {
        int index = deckListCards.Count;
        DeckData deckData = new DeckData();
        deckData.cardData = cardData;
        deckData.count = 1;

        if (index == 0)
        {
            deckList.Add(deckData);
            return index;
        }

        int left = 0;
        int right = index - 1;

        while (left <= right)
        {
            int mid = (left + right) / 2;
            int compare = CompareCard(cardData, deckList[mid].cardData);

            if (compare < 0)
                right = mid - 1;
            else
                left = mid + 1;
        }

        index = left;
        deckList.Insert(index, deckData);
        return index;
    }


    private void OutputDeckCard(GameObject cardDeck)
    {
        // Output Deck에서 삭제할 때
        var card = cardDeck.GetComponent<Card>();
        var deckCardComponent = cardDeck.GetComponentInChildren<DeckCard>(true);

        if (card == null || deckCardComponent == null)
            return;

        if (deckCardComponent.deckCount == 1)
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

        while (left <= right)
        {
            int mid = (left + right) / 2;
            int compare = CompareCard(cardData, deckListCards[mid]);

            if (compare == 0)
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

    #endregion

    private void InputDeckCard2(GameObject cardDeck)
    {
        Card card = cardDeck.GetComponent<Card>();
        if (card != null)
        {
            if (card.cardData == null)
            {
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

        int targetIndex = FindDeckList(newCardData);
        
        if(targetIndex != -1)
        {
            DeckData deckData = deckList[targetIndex];

            Destroy(cardDeck);
            if(deckData.count >= deckCardMaxCount)
            {
                Debug.Log("이 카드는 덱에 더 넣을 수 없습니다.");
                return;
            }
            deckData.count++;

            Transform child = content.transform.GetChild(targetIndex);
            var deckCardComponent = child.GetComponentInChildren<DeckCard>(true);

            if(deckCardComponent != null)
            {
                deckCardComponent.CurrentDeckCount(deckData.count);
            }
        }
        else
        {
            int index = SortInsert(newCardData);
            cardDeck.transform.SetParent(content.transform);
            cardDeck.transform.SetSiblingIndex(index);

            var deckCardComponent = cardDeck.GetComponentInChildren<DeckCard>(true);
            if(deckCardComponent != null)
            {
                deckCardComponent.CurrentDeckCount(1);
            }
        }

        currentCard++;
        cardNumber.text = $"{currentCard}/{maxCard}\n장";
    }

    private int FindDeckList(CardData newCardData)
    {
        int left = 0;
        int right = deckList.Count - 1;

        while(left <= right)
        {
            int mid = (left + right) / 2;
            int compare = CompareCard(newCardData, deckList[mid].cardData);

            if (compare == 0)
                return mid;

            if (compare < 0) right = mid - 1;
            else left = mid + 1;
        }

        return -1;
    }

    private int SortInsert(CardData cardData)
    {
        DeckData newDeckData = new DeckData();
        newDeckData.cardData = cardData;
        newDeckData.count = 1;

        int left = 0;
        int right = deckList.Count - 1;

        while(left <= right)
        {
            int mid = (left + right) / 2;
            int compare = CompareCard(cardData, deckList[mid].cardData);

            if (compare < 0) 
                right = mid - 1;
            else 
                left = mid + 1;
        }

        deckList.Insert(left, newDeckData);
        return left;
    }

    

    private int CompareCard(CardData newCard, CardData oldCard)
    {
        if (newCard.cost != oldCard.cost)
            return newCard.cost.CompareTo(oldCard.cost);

        return string.Compare(newCard.cardName, oldCard.cardName, System.StringComparison.Ordinal); 
    }

    private void OutputDeckCard2(GameObject cardDeck)
    {
        var card = cardDeck.GetComponent<Card>();

        if (card == null)
            return;

        int index = FindDeckList(card.cardData);

        if(index == -1)
        {
            Debug.Log("삭제하려는 Data가 없습니다");
            return;
        }

        DeckData deckData = deckList[index];

        if(deckData.count <= 1)
        {
            deckList.RemoveAt(index);
            Destroy(cardDeck);
        }
        else
        {
            deckData.count--;
            var deckCardComponent = cardDeck.GetComponentInChildren<DeckCard>(true);
            if (deckCardComponent != null)
            {
                deckCardComponent.CurrentDeckCount(deckData.count);
            }
        }

        currentCard--;
        cardNumber.text = $"{currentCard}/{maxCard}\n장";
    }


    public async UniTask RecontructDeck(DeckInformation deckInfo)
    {
        var resourceManager = Locator<ResourceManager>.Get();
        var dataManager = Locator<DataManager>.Get();
        var cardTable = dataManager.GetCardData();

        GameObject cardOrigin = await resourceManager.Get<GameObject>("Card");
        Card cardPrefab = cardOrigin.GetComponent<Card>();

        var factory = Locator<Factory>.Get();
        currentCard = deckInfo.currentCard;
        maxCard = deckInfo.maxCard;

        List<UniTask> tasks = new List<UniTask>();
        deckList = deckInfo.deckData;
        foreach (var data in deckInfo.deckData)
        {
            Card cardInstance = factory.Create(cardPrefab, content.transform);
            tasks.Add(cardInstance.CardSetting(cardTable[data.cardData.cardId]));
            cardInstance.deckCardScript.CurrentDeckCount(data.count);
        }

        await UniTask.WhenAll(tasks);

        var eventManager = Locator<EventManager>.Get();
        var heroDataTable = dataManager.GetHeroData();
        var heroData = heroDataTable[deckInfo.heroIndex];

        string[] heros = { heroData.heroDeckName, "중립" };
        FilterParameter parameter = new FilterParameter(FilterType.Search, _job: heros);
        eventManager.Notify(ChannelInfo.Filter, parameter);

        eventManager.Notify(ChannelInfo.SelectingDeck, true);
    }
}
