using UnityEngine;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

public class BattleDeckList : MonoBehaviour, IChannel
{
    // 음.. 테스트를 해보자.
    public GameObject deckCardPrefab;
    private List<GameObject> deck;

    private int deckMaxCount = 30;
    private int currentIndex = 0;
    private List<CardData> battleDeck = new List<CardData>();

    private const int fatCardWidth = 40;
    private const int middleCardWidth = 20;
    private const int slimCardWidth = 10;
    private const int lastCardWidth = 3;

    // 여기서 Test Deck을 ScirtaleObject를 뽑자.

    private async void Start()
    {
        await Cysharp.Threading.Tasks.UniTask.WaitUntil(() => GameManager.isReadyGameManager);

        // 그러면 여기서 DeckData 설정을 하면 굳이 설정안해도 된다.
        InitalizeCardDummySetting();
        ResettingCardSetting(deck.Count);
    }

    // 여기서 가져온 Object가 가지고 있던 Sprite이나 다양한 것들로 바꿔야 할 것 같아보인다.
    private void OnEnable()
    {
        var eventManager = Locator<EventManager>.Get();
        eventManager.Subscription(ChannelInfo.DrawBattleCard, HandleEvent);
    }

    private void OnDisable()
    {
        var eventManager = Locator<EventManager>.Get();
        eventManager.Unsubscription(ChannelInfo.DrawBattleCard, HandleEvent);
    }

    public void HandleEvent(ChannelInfo channel, object information = null)
    {
        switch(channel)
        {
            case ChannelInfo.DrawBattleCard:
                // card Prefab이 들어온다 한들
                // 이 곳에서 다양한 Resource를 받아야 한다.
                GameObject card = information as GameObject;
                if(card != null)
                {
                    // Card에 Resource 정보를 넣는다.
                    // 탈진 카드 Resource 설정도 이곳이다.
                    if(currentIndex >= 0)
                    {
                        Debug.Log("Card Real Resource");
                        var battleComponent = card.GetComponent<BattleCard>();
                        battleComponent.InitalizeCardSetup(battleDeck[currentIndex], this.GetCancellationTokenOnDestroy()).Forget();
                    }
                    else
                    {
                        Debug.Log("Card Image Resource");
                        // 여기는 탈진 카드 ResourceSetting을 할 것이다.
                    }


                    DeckSetting();
                }
                break;
        }
    }

    private void DeckSetting()
    {
        // current 
        if (currentIndex < 0) {
            Debug.Log("Dont Drawing Card");
            return;
        }

        // currentIndex에 따라서 ResettingCardSetting(int index)를 실행해야 하는데 매번 확인하면 안되고 일정 index에 도달했으면 그 때 deck이 바뀐다.
        // 근데 매번 확인하는 것보다 일정 index에 딱 한번 도달했으면 그 때 Deck 두께가 바뀌었으면 좋겠다.
        switch(currentIndex)
        {
            case fatCardWidth:
                ResettingCardSetting(fatCardWidth);
                break;
            case middleCardWidth:
                ResettingCardSetting(middleCardWidth);
                break;
            case slimCardWidth:
                ResettingCardSetting(slimCardWidth);
                break;
            case lastCardWidth:
                ResettingCardSetting(lastCardWidth);
                break;
            default:
                break;
        }

        currentIndex--;
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
        BattleCardTransform battleCardInfo = new BattleCardTransform();
        var card = deck[maxCount - 1];
        var cardRect = card.GetComponent<RectTransform>();
        battleCardInfo.position = cardRect.position;
        battleCardInfo.rotation = cardRect.rotation;

        var eventManager = Locator<EventManager>.Get();
        eventManager.Notify(ChannelInfo.BattleDeckListPosition, battleCardInfo);
    }


    private void InitalizeCardDummySetting()
    {
        // 이 deckData는 어떻게 사용해야 하지?? DeckData 섞기도 해야한다.
        var battleManager = Locator<BattleManager>.Get();
        var deckData = battleManager.GetDeckData();

        PrepareBattleDeckShffle(deckData);

        deck = new List<GameObject>();

        int count = 30;

        /*
        if (battleDeck == null)
            count = 30;
        else
            count = battleDeck.Count;
        */

        currentIndex = count - 1;

        for(int i = 0; i < count; i++)
        {
            var deckCard = Instantiate(deckCardPrefab, this.transform);
            deck.Add(deckCard);
        }
    }


    private void PrepareBattleDeckShffle(List<DeckData> deckData)
    {
        if (deckData == null)
        {
            Debug.Log("None Deck Data Information");
            return;
        }

        battleDeck.Clear();

        // deckData를 battleDeckList에 넣는 작업을 수행한다.
        foreach(var deckItem in deckData)
        {
            for(int i = 0; i < deckItem.count; i++)
            {
                battleDeck.Add(deckItem.cardData);
            }
        }

        /// 랜덤
        // 랜덤에 대해 조금 자세하게 알아볼 필요가 있을 것 같다..
        // 일단은 이렇게 해두자.
        for(int i = battleDeck.Count - 1; i > 0; i--)
        {
            int randomIndex = UnityEngine.Random.Range(0, i + 1);
            CardData temp = battleDeck[i];
            battleDeck[i] = battleDeck[randomIndex];
            battleDeck[randomIndex] = temp;
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

        if (index == 0)
            return;

        DeckListSetup(index);
    }
}


public struct BattleCardTransform
{
    public Vector3 position;
    public Quaternion rotation;
}