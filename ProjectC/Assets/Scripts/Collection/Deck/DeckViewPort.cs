using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DeckViewPort : MonoBehaviour, IChannel
{
    public GameObject content;
    private int maxCard = 30;
    public TextMeshProUGUI cardNumber;

    private void OnEnable()
    {
        cardNumber.text = $"{content.transform.childCount}/{maxCard}\n장";
        var eventManager = Locator<EventManager>.Get();
        eventManager.Subscription(ChannelInfo.InputDeck, HandleEvent);
        eventManager.Subscription(ChannelInfo.OutputDeck, HandleEvent);
    }

    private void OnDisable()
    {
        var eventManager = Locator<EventManager>.Get();
        eventManager.Unsubscription(ChannelInfo.InputDeck, HandleEvent);
        eventManager.Unsubscription(ChannelInfo.OutputDeck, HandleEvent);
    }

    public void HandleEvent(ChannelInfo channel, object information = null)
    {
        switch(channel)
        {
            case ChannelInfo.InputDeck:
                // information -> Deck 바꾸고
                GameObject cardDeck = information as GameObject;

                if(cardDeck != null)
                {
                    if(content.transform.childCount >= maxCard)
                    {
                        Debug.Log("카드가 가득 차서 넣을 수 없다.");
                        return;
                    }    

                    cardDeck.transform.SetParent(content.transform);
                    LayoutRebuilder.ForceRebuildLayoutImmediate(content.GetComponent<RectTransform>());
                    //cardDeck.transform.SetSiblingIndex(2); 나중에 index 형식으로 넣을 수 있다.

                    cardNumber.text = $"{content.transform.childCount}/{maxCard}\n장";
                }
                break;
            case ChannelInfo.OutputDeck:
                cardNumber.text = $"{content.transform.childCount}/{maxCard}\n장";
                break;
        }
    }

}
