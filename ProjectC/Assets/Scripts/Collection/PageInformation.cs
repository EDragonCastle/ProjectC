using UnityEngine;
using Cysharp.Threading.Tasks;

public class PageInformation : MonoBehaviour, IChannel
{
    public bool isSelectingDeckList = false;

    public int page;

    public Card[] cards;

    private void OnEnable()
    {
        var eventManager = Locator<EventManager>.Get();
        eventManager.Subscription(ChannelInfo.SelectingDeck, HandleEvent);
    }

    private void OnDisable()
    {
        var eventManager = Locator<EventManager>.Get();
        eventManager.Unsubscription(ChannelInfo.SelectingDeck, HandleEvent);
        isSelectingDeckList = false;
    }

    public async UniTask ResettingCard()
    {
        UniTask[] tasks = new UniTask[cards.Length];

        // cards에 있는 card setting async methord 들을 병렬로 처리할 방법이 없을까?
        for(int i = 0; i < cards.Length; i++)
        {
            tasks[i] = cards[i].CardSetting(page);
        }

        await UniTask.WhenAll(tasks);
    }

    public void ReleaseCard()
    {
        // cards에 있는 card setting async methord 들을 병렬로 처리할 방법이 없을까?
        for (int i = 0; i < cards.Length; i++)
        {
            cards[i].ReleaseCard(page);
        }
    }

    public void HandleEvent(ChannelInfo channel, object information = null)
    {
        switch(channel)
        {
            case ChannelInfo.SelectingDeck:
                if(information is bool isActive) {
                    isSelectingDeckList = isActive;
                }
                break;
        }
    }

}
