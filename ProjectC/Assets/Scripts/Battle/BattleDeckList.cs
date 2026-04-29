using UnityEngine;
using System.Collections.Generic;

public class BattleDeckList : MonoBehaviour
{
    // 음.. 테스트를 해보자.
    public GameObject deckCardPrefab;
    private List<GameObject> deck;

    private int deckMaxCount = 30;

    private void Start()
    {
        InitalizeCardDummySetting();
        ResettingCardSetting(deckMaxCount);
    }


    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            ResettingCardSetting(10);
        }   
        
        if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            ResettingCardSetting(5);
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            ResettingCardSetting(1);
        }
    }


    private void DeckListSetup(int maxCount)
    {
        // 지금은 Deck Card Prefab에서 카드를 한번 쭉 나열해볼까?
        float midIndex = (maxCount - 1) / 2f;
        for(int i = 0; i < deckMaxCount; i++)
        {
            var deckCard = deck[i];
            var deckCardRect = deckCard.GetComponent<RectTransform>();

            float index = i - midIndex;
            
            deckCardRect.localPosition = new Vector3(0, 0, index);
            deckCardRect.localRotation = Quaternion.Euler(new Vector3(0, -65, 90));
        }

        // Position이랑 Rotation을 어떻게 전달할까?
        BattleCardInformation battleCardInfo = new BattleCardInformation();
        var card = deck[maxCount - 1];
        var cardRect = card.GetComponent<RectTransform>();
        battleCardInfo.position = cardRect.position;
        battleCardInfo.rotation = cardRect.rotation;

        var eventManager = Locator<EventManager>.Get();
        eventManager.Notify(ChannelInfo.BattleDeckListPosition, battleCardInfo);
    }

    private void InitalizeCardDummySetting()
    {
        deck = new List<GameObject>();
        
        for(int i = 0; i < deckMaxCount; i++)
        {
            var deckCard = Instantiate(deckCardPrefab, this.transform);
            deck.Add(deckCard);
        }
    }

    private void ResettingCardSetting(int index)
    {
        for(int i = 0; i < deck.Count; i++)
        {
            if (i < index)
                deck[i].SetActive(true);
            else
                deck[i].SetActive(false);
        }

        DeckListSetup(index);
    }
}


public struct BattleCardInformation
{
    public Vector3 position;
    public Quaternion rotation;
}